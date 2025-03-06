using DataObra.Datos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DataObra.Datos
{
    public class HttpQueueManager
    {
        private readonly BlockingCollection<QueueItem> _queue = new BlockingCollection<QueueItem>();
        public HttpClient HttpClient { get; }
        public ObservableCollection<LogEntry> Logs { get; } = new ObservableCollection<LogEntry>();
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        public event Func<QueueItem, Task<bool>> RequestRetryConfirmation;
     

        public HttpQueueManager(HttpClient httpClient)
        {
            HttpClient = httpClient;
            Task.Run(() => ProcessQueue(_cancellationTokenSource.Token));
        }

        public void Enqueue(QueueItem item)
        {
            _queue.Add(item);
            LogRequest(item, true, null, "Enqueued");
        }

        private async Task ProcessQueue(CancellationToken cancellationToken)
        {
            foreach (var item in _queue.GetConsumingEnumerable(cancellationToken))
            {
                bool success = false;
                int retryCount = 0;
                const int maxRetries = 1;

                while (!success && retryCount < maxRetries)
                {
                    LogRequest(item, true, null, "Started");
                    try
                    {
                        var response = await SendRequest(item);
                        response.EnsureSuccessStatusCode();
                        LogRequest(item, true, null, "Completed");
                        item.ResponseTaskCompletionSource.SetResult(response);
                        success = true;
                    }
                    catch (Exception ex) when (ex is HttpRequestException httpEx && (int)httpEx.StatusCode == 500)
                    {
                        retryCount++;
                        LogRequest(item, false, ex.Message, "Error");

                        if (retryCount >= maxRetries)
                        {
                            var shouldRetry = RequestRetryConfirmation != null ? await RequestRetryConfirmation(item) : false;

                            if (!shouldRetry)
                            {
                                LogRequest(item, false, ex.Message, "Failed");
                                item.ResponseTaskCompletionSource.SetException(ex);
                                break;
                            }
                            else
                            {
                                retryCount = 0; // Reset the retry count if the user wants to continue
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        LogRequest(item, false, ex.Message, "Error");

                        if (retryCount >= maxRetries)
                        {
                            var shouldRetry = RequestRetryConfirmation != null ? await RequestRetryConfirmation(item) : false;

                            if (!shouldRetry)
                            {
                                LogRequest(item, false, ex.Message, "Failed");
                                item.ResponseTaskCompletionSource.SetException(ex);
                                break;
                            }
                            else
                            {
                                retryCount = 0; // Reset the retry count if the user wants to continue
                            }
                        }
                    }
                }
            }
        }

        private async Task<HttpResponseMessage> SendRequest(QueueItem item)
        {
            HttpResponseMessage response;
            if (item.Method == HttpMethod.Get || item.Method == HttpMethod.Delete)
            {
                var query = item.Parameters != null ? "?" + string.Join("&", item.Parameters.Select(p => $"{p.Key}={p.Value}")) : string.Empty;
                response = await HttpClient.SendAsync(new HttpRequestMessage(item.Method, item.Url + query));
            }
            else
            {
                var json = JsonSerializer.Serialize(item.Data, jsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await HttpClient.SendAsync(new HttpRequestMessage(item.Method, item.Url) { Content = content });
            }

            if (!response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, jsonSerializerOptions);
                throw new HttpRequestException(resultado?.Message ?? "Error desconocido del servidor");
            }

            return response;
        }


        private void LogRequest(QueueItem item, bool success, string errorMessage, string status)
        {
            _dispatcher.Invoke(() =>
            {
                Logs.Add(new LogEntry
                {
                    Id=item.Id,
                    Timestamp = DateTime.Now,
                    Url = item.Url,
                    Method = item.Method,
                    Success = success,
                    ErrorMessage = errorMessage,
                    Status = status
                });
            });
        }

        public void StopProcessing()
        {
            _queue.CompleteAdding();
            _cancellationTokenSource.Cancel();
        }

        public ObservableCollection<LogEntry> GetLogs()
        {
            return Logs;
        }
    }
}

