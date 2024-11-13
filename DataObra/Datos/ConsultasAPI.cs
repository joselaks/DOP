using Biblioteca;
using DataObra.Datos;
using DataObra;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Security.Policy;
using System.Net;

namespace DataObra.Datos
{
    public partial class ConsultasAPI
    {
        private readonly HttpQueueManager _queueManager;

        public ConsultasAPI()
        {
            _queueManager = App.QueueManager; // Obtiene el QueueManager de la clase App
            _queueManager.RequestRetryConfirmation += OnRequestRetryConfirmation;
        }

        private void OnAddGetRequestClick(object sender, RoutedEventArgs e)
        {
            var item = new QueueItem
            {
                Url = "https://jsonplaceholder.typicode.com/posts",
                Method = HttpMethod.Get,
                Parameters = new Dictionary<string, string>
                {
                    { "userId", "1" }
                }
            };
            _queueManager.Enqueue(item);
        }

        private void OnShowLogsClick(object sender, RoutedEventArgs e)
        {
            //LogListBox.ItemsSource = _queueManager.GetLogs();
        }

        private async Task<bool> OnRequestRetryConfirmation(QueueItem item)
        {
            var result = MessageBox.Show($"La solicitud a {item.Url} ha fallado tres veces. ¿Deseas seguir intentándolo?", "Reintentar Solicitud", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        // Validar usuario
        public async Task<(bool Success, string Message, Usuario? Usuario)> ValidarUsuarioAsync(string email, string pass)
        {
            var item = new QueueItem
            {
                Url = $"{App.BaseUrl}usuarios/validacion",
                Method = HttpMethod.Get,
                Parameters = new Dictionary<string, string>
                {
                    { "email", email },
                    { "pass", pass }
                }
            };
            _queueManager.Enqueue(item);

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();
                var usuario = JsonSerializer.Deserialize<CredencialesUsuario>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (usuario.Token != null)
                {
                    _queueManager.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", usuario.Token);
                    return (true, "Usuario validado exitosamente.", usuario.DatosUsuario);
                }
                else
                {
                    return (false, "No pudo validarse el usuario", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        //Documento Post
        public async Task<(bool Success, string Message, int? Id)> PostDocumentoAsync(Documento nuevoDoc)
        {
            var item = new QueueItem
            {
                Url = $"{App.BaseUrl}documentos/",
                Method = HttpMethod.Post,
                Data = nuevoDoc
            };
            _queueManager.Enqueue(item);

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();
                var nuevoDocumentoID = JsonSerializer.Deserialize<int>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (true, "Documento insertado con ID", nuevoDocumentoID);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }



        }



        // Documentos Get por ID
        public async Task<(bool Success, string Message, List<Documento> Documentos)> GetDocumentosPorCuentaIDAsync(short cuentaID)
        {
            var item = new QueueItem
            {
                Url = $"{App.BaseUrl}documentos/cuenta/{cuentaID}",
                Method = HttpMethod.Get
            };

            _queueManager.Enqueue(item);

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();
                var documentos = JsonSerializer.Deserialize<List<Documento>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (true, "Documentos obtenidos exitosamente.", documentos);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }


        public async Task<(bool Success, string Message)> DeleteDocumentoAsync(int id)
        {
            var item = new QueueItem
            {
                Url = $"{App.BaseUrl}documentos/{id}",
                Method = HttpMethod.Delete
            };
            _queueManager.Enqueue(item);

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return (resultado.Success, resultado.Message);
            }
            catch (HttpRequestException httpEx)
            {
                return (false, $"Error HTTP: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

       






        private void DisplayData(List<Documento> documentos)
        {
            // Lógica para mostrar los datos en la UI
            foreach (var documento in documentos)
            {
                // Añade cada documento a un control visual, por ejemplo un ListBox
                //myListBox.Items.Add(documento.Title); // Suponiendo que Documento tiene una propiedad Title
            }
        }

        //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        //{
        //    base.OnClosing(e);
        //    _queueManager.StopProcessing(); // Detiene el procesamiento cuando se cierra la ventana
        //}
    }

    public class ResultadoOperacion
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
