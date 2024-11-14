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
        int evento = 1;

        public ConsultasAPI()
        {
            _queueManager = App.QueueManager; // Obtiene el QueueManager de la clase App
            _queueManager.RequestRetryConfirmation += OnRequestRetryConfirmation;
        }

        // Solicita definición cuando la operación rebotó 3 veces
        private async Task<bool> OnRequestRetryConfirmation(QueueItem item)
        {
            var result = MessageBox.Show($"La solicitud a {item.Url} ha fallado tres veces. ¿Deseas seguir intentándolo?", "Reintentar Solicitud", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        // Usuario Get y validación
        public async Task<(bool Success, string Message, Usuario? Usuario)> ValidarUsuarioAsync(string email, string pass)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}usuarios/validacion",
                Method = HttpMethod.Get,
                Parameters = new Dictionary<string, string>
                {
                    { "email", email },
                    { "pass", pass }
                }
            };
            _queueManager.Enqueue(item);
            evento++;

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
                Id= evento,
                Url = $"{App.BaseUrl}documentos/",
                Method = HttpMethod.Post,
                Data = nuevoDoc
            };
            _queueManager.Enqueue(item);
            evento++;

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

        //Documento rel Post
        public async Task<(bool Success, string Message)> PostDocumentoRelAsync(DocumentoRel nuevaRel)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentos/rel",
                Method = HttpMethod.Post,
                Data = nuevaRel
            };
            _queueManager.Enqueue(item);
            evento++;

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

        // Documentos Get por Cuenta
        public async Task<(bool Success, string Message, List<Documento> Documentos)> GetDocumentosPorCuentaIDAsync(int ID)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentos/{ID}",
                Method = HttpMethod.Get
            };

            _queueManager.Enqueue(item);
            evento++;

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

        // DocumentosRel Get por superiorID
        public async Task<(bool Success, string Message, List<DocumentoRel> DocumentosRel)> GetDocumentosRelPorSupIDAsync(int supID)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentos/rel/{supID}",
                Method = HttpMethod.Get
            };

            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();
                var documentos = JsonSerializer.Deserialize<List<DocumentoRel>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (true, "Documentos relacionados obtenidos exitosamente.", documentos);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }






        // Documento Delete
        public async Task<(bool Success, string Message)> DeleteDocumentoAsync(int id)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentos/{id}",
                Method = HttpMethod.Delete
            };
            _queueManager.Enqueue(item);
            evento++;

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


        // Documento Get por ID
        public async Task<(bool Success, string Message, Documento? doc)> ObtenerDocumentoPorID(int id)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentos/id/{id}",
                Method = HttpMethod.Get
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<Documento>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (resultado != null)
                {
                    return (true, "Se obtuvo el documento", resultado);
                }
                else
                {
                    return(false, "No se obtuvo el documento", resultado);
                }
               
            }
            catch (HttpRequestException httpEx)
            {
                return (false, $"Error HTTP: {httpEx.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }


        // Documento Get por cuentaID
        public async Task<(bool Success, string Message, List<Documento> docs)> ObtenerDocumentosPorCuentaID(int id)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentos/cuenta/{id}",
                Method = HttpMethod.Get
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<List<Documento>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (resultado != null)
                {
                    return (true, "Se obtuvieron los", resultado);
                }
                else
                {
                    return (false, "No se obtuvoieron documentos", resultado);
                }

            }
            catch (HttpRequestException httpEx)
            {
                return (false, $"Error HTTP: {httpEx.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        //Documento Put
        public async Task<(bool Success, string Message)> PutDocumentoAsync(Documento modDoc)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentos/",
                Method = HttpMethod.Put,
                Data = modDoc
            };
            _queueManager.Enqueue(item);
            evento++;

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

        //private void DisplayData(List<Documento> documentos)
        //{
        //    // Lógica para mostrar los datos en la UI
        //    foreach (var documento in documentos)
        //    {
        //        // Añade cada documento a un control visual, por ejemplo un ListBox
        //        //myListBox.Items.Add(documento.Title); // Suponiendo que Documento tiene una propiedad Title
        //    }
        //}

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
