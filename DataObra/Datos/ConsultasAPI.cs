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

        // Método de ejemplo para validar usuario
        public async void ValidarUsuarioAsync(string email, string pass)
        {
            var item = new QueueItem
            {
                Url = $"https://localhost:7255/usuarios/validacion",
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
                    MessageBox.Show("Usuario validado exitosamente.");
                }
                else
                {
                    MessageBox.Show("Usuario inexistente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        //Procedimiento para crear un nuevo documento
        public async Task<(bool Success, string Message, int? Id)> PostDocumentoAsync(int cuentaID, Documento nuevoDoc)
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
                return (true, "Documento insertado con ID",nuevoDocumentoID);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }



        }



        // Procedimiento para obtener todos los documentos de una cuenta
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
}
