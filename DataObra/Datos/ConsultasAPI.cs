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
using Syncfusion.UI.Xaml.Diagram;
using DataObra.Agrupadores;
using System.Text;
using Bibioteca.Clases;

namespace DataObra.Datos
{
    public static class ConsultasAPI
    {
        private static readonly HttpQueueManager _queueManager;
        private static int evento = 1;

        static ConsultasAPI()
        {
            _queueManager = App.QueueManager; // Obtiene el QueueManager de la clase App
            _queueManager.RequestRetryConfirmation += OnRequestRetryConfirmation;
        }

        // Solicita definición cuando la operación rebotó 3 veces
        private static async Task<bool> OnRequestRetryConfirmation(QueueItem item)
        {
            var result = MessageBox.Show($"La solicitud a {item.Url} ha fallado tres veces. ¿Deseas seguir intentándolo?", "Reintentar Solicitud", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        #region Post

        //Documento Post
        public static async Task<(bool Success, string Message, int? Id)> PostDocumentoAsync(Documento nuevoDoc)
        {
            var item = new QueueItem
            {
                Id = evento,
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
                return (true, "Documento insertado con ID: ", nuevoDocumentoID);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public static async Task<(bool Success, string Message)> ProcesarInfoDocumentoAsync(InfoDocumento documento)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentos/procesar",
                Method = HttpMethod.Post,
                Data = documento
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultado.Success)
                {
                    return (true, "Documento procesado exitosamente.");
                }
                else
                {
                    return (false, resultado.Message);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        // Nuevo método para procesar la lista de detalles de documentos
        public static async Task<(bool Success, string Message)> ProcesarListaDetalleDocumentoAsync(List<DocumentoDet> listaDetalleDocumento)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentosdet/procesar",
                Method = HttpMethod.Post,
                Data = listaDetalleDocumento
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultado.Success)
                {
                    return (true, "Lista de detalles de documentos procesada exitosamente.");
                }
                else
                {
                    return (false, resultado.Message);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }




        //Documento rel Post
        public static async Task<(bool Success, string Message)> PostDocumentoRelAsync(DocumentoRel nuevaRel)
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

        //Agrupador rel Post
        public static async Task<(bool Success, string Message, int? Id)> PostAgrupadorAsync(Agrupador nuevaRel)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}agrupadores",
                Method = HttpMethod.Post,
                Data = nuevaRel
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();
                var nuevoAgrpador = JsonSerializer.Deserialize<int>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (true, "Documento insertado con ID", nuevoAgrpador);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public static async Task<(bool Success, string Message)> ProcesarMovimientosAsync(List<Movimiento> listaMovimientos)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}movimientos/procesar",
                Method = HttpMethod.Post,
                Data = listaMovimientos
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultado.Success)
                {
                    return (true, "Lista de movimientos procesada exitosamente.");
                }
                else
                {
                    return (false, resultado.Message);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }


        public static async Task<(bool Success, string Message)> ProcesarImpuestosAsync(List<Impuesto> listaImpuestos)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}impuestos/procesar",
                Method = HttpMethod.Post,
                Data = listaImpuestos
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultado.Success)
                {
                    return (true, "Lista de impuestos procesada exitosamente.");
                }
                else
                {
                    return (false, resultado.Message);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }




        #endregion

        // Usuario Get y validación
        public static async Task<(bool Success, string Message, Usuario? Usuario)> ValidarUsuarioAsync(string email, string pass)
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
                    App.IdUsuario = usuario.DatosUsuario.ID;
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

        //// Documentos Get por Cuenta
        //public static async Task<(bool Success, string Message, List<Documento> Documentos)> GetDocumentosPorCuentaIDAsync(int ID)
        //{
        //    var item = new QueueItem
        //    {
        //        Id = evento,
        //        Url = $"{App.BaseUrl}documentos/{ID}",
        //        Method = HttpMethod.Get
        //    };

        //    _queueManager.Enqueue(item);
        //    evento++;

        //    try
        //    {
        //        var response = await item.ResponseTaskCompletionSource.Task;
        //        var responseString = await response.Content.ReadAsStringAsync();
        //        var documentos = JsonSerializer.Deserialize<List<Documento>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //        return (true, "Documentos obtenidos exitosamente.", documentos);
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, $"Error: {ex.Message}", null);
        //    }
        //}

        // DocumentosRel Get por superiorID
        public static async Task<(bool Success, string Message, List<DocumentoRel> DocumentosRel)> GetDocumentosRelPorSupIDAsync(int supID)
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
        public static async Task<(bool Success, string Message)> DeleteDocumentoAsync(int id)
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

        // DocumentoRel Delete
        public static async Task<(bool Success, string Message)> DeleteDocumentoRelAsync(int supID, int infID)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentos/rel/{supID}/{infID}",
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
        public static async Task<(bool Success, string Message, Documento? doc)> ObtenerDocumentoPorID(int id)
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
                    return (false, "No se obtuvo el documento", resultado);
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
        public static async Task<(bool Success, string Message, List<Documento> docs)> ObtenerDocumentosPorCuentaID(int id)
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

        // Obtener agrupadores por cuentaID
        public static async Task<(bool Success, string Message, List<Agrupador> agrupadores)> ObtenerAgrupadoresPorCuentaID(int id)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}agrupadores/cuenta/{id}",
                Method = HttpMethod.Get
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<List<Agrupador>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (resultado != null)
                {
                    return (true, "Se obtuvieron los", resultado);
                }
                else
                {
                    return (false, "No se obtuvieron documentos", resultado);
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

        // Borrar agrupador
        public static async Task<(bool Success, string Message)> BorrarAgrupador(int id)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}agrupadores/{id}",
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

        //Documento Put
        public static async Task<(bool Success, string Message)> PutDocumentoAsync(Documento modDoc)
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

        //Agrupador Put
        public static async Task<(bool Success, string Message)> PutAgrupadorAsync(Agrupador agrupador)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}agrupadores/",
                Method = HttpMethod.Put,
                Data = agrupador
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


        //DocumentoDet Post
        public static async Task<(bool Success, string Message, int? id)> PostDocumentoDetAsync(DocumentoDet nuevoDocDet)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentosdet",
                Method = HttpMethod.Post,
                Data = nuevoDocDet
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();
                var nuevoDocumentoID = JsonSerializer.Deserialize<int>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (true, "Documento insertado con ID: ", nuevoDocumentoID);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        // DocumentoDet Get por ID y campo
        public static async Task<(bool Success, string Message, List<DocumentoDet> DocumentosDet)> GetDocumentosDetPorCampoAsync(int id, string fieldName, short cuentaID)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentosdet/{fieldName}/{id}/{cuentaID}",
                Method = HttpMethod.Get
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();
                var documentosDet = JsonSerializer.Deserialize<List<DocumentoDet>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (documentosDet.Count() >= 1)
                {
                    return (true, "Detalles obtenidos exitosamente.", documentosDet);
                }
                else
                {
                    return (false, "No hay detalles ", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }



        // DocumentoDet Put incluye borrado cuando no esta vinculado a ningún documento
        public static async Task<(bool Success, string Message)> PutDocumentoDetAsync(DocumentoDet documentoDet)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}documentosdet",
                Method = HttpMethod.Put,
                Data = documentoDet
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (resultado.Success, resultado.Message);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> ProcesarArbolPresupuestoAsync(ProcesarArbolPresupuestoRequest request)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}pre/procesar",
                Method = HttpMethod.Post,
                Data = request
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultado.Success)
                {
                    return (true, "Árbol de presupuesto procesado exitosamente.");
                }
                else
                {
                    return (false, resultado.Message);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message, List<Impuesto> Impuestos)> GetImpuestosPorCampoAsync(int id, string fieldName, short cuentaID)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}impuestos/{fieldName}/{id}/{cuentaID}",
                Method = HttpMethod.Get
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();
                var impuestos = JsonSerializer.Deserialize<List<Impuesto>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (impuestos.Count() >= 1)
                {
                    return (true, "Impuestos obtenidos exitosamente.", impuestos);
                }
                else
                {
                    return (false, "No hay impuestos.", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }


        public static async Task<(bool Success, string Message, List<Movimiento> Movimientos)> GetMovimientosPorCampoAsync(int id, string fieldName, short cuentaID)
        {
            var item = new QueueItem
            {
                Id = evento,
                Url = $"{App.BaseUrl}movimientos/{fieldName}/{id}/{cuentaID}",
                Method = HttpMethod.Get
            };
            _queueManager.Enqueue(item);
            evento++;

            try
            {
                var response = await item.ResponseTaskCompletionSource.Task;
                var responseString = await response.Content.ReadAsStringAsync();
                var movimientos = JsonSerializer.Deserialize<List<Movimiento>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (movimientos.Count() >= 1)
                {
                    return (true, "Movimientos obtenidos exitosamente.", movimientos);
                }
                else
                {
                    return (false, "No hay movimientos.", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }


    }

    public class ResultadoOperacion
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

