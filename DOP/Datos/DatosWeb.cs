using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace DOP.Datos
    {
    public static class DatosWeb
        {
        private static readonly HttpClient httpClient;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        public static ObservableCollection<string> LogEntries { get; private set; } = new ObservableCollection<string>();

        static DatosWeb()
            {
            // En cualquier parte de tu aplicación
            httpClient = ((App)Application.Current).HttpClient;

            }

        private static async Task<(bool Success, string Message, T Data)> ExecuteRequestAsync<T>(Func<Task<HttpResponseMessage>> httpRequest, string logMessage)
            {
            DateTime sendTime = DateTime.Now;
            LogEntries.Add($"{sendTime:yyyy-MM-dd HH:mm:ss.fff} - Enviado - {logMessage}");

            try
                {
                var response = await httpRequest();
                DateTime receiveTime = DateTime.Now;
                TimeSpan duration = receiveTime - sendTime;

                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    {
                    var data = JsonSerializer.Deserialize<T>(responseString, jsonSerializerOptions);
                    LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - {logMessage} - Éxito - Duración: {duration.TotalSeconds:F3} s");
                    return (true, "Operación exitosa.", data);
                    }
                else
                    {
                    var errorResponse = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, jsonSerializerOptions);
                    // Cambia aquí: usa Message o Mensaje según cuál no sea nula
                    string errorMessage = !string.IsNullOrEmpty(errorResponse?.Message)
                        ? $"Error: {errorResponse.Message}"
                        : !string.IsNullOrEmpty(errorResponse?.Mensaje)
                            ? $"Error: {errorResponse.Mensaje}"
                            : "Error desconocido";
                    LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - {logMessage} - {errorMessage} - Duración: {duration.TotalSeconds:F3} s");
                    return (false, errorMessage, default);
                    }
                }
            catch (HttpRequestException httpEx)
                {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                string errorMessage = $"Error HTTP: {httpEx.Message}";
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - {logMessage} - {errorMessage} - Duración: {duration.TotalSeconds:F3} s");
                return (false, errorMessage, default);
                }
            catch (Exception ex)
                {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                string errorMessage = $"Error: {ex.Message}";
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - {logMessage} - {errorMessage} - Duración: {duration.TotalSeconds:F3} s");
                return (false, errorMessage, default);
                }
            }

        public static async Task<bool> RegistrarSalidaUsuarioAsync(int usuarioId, string macaddress)
            {
            string url = $"{App.BaseUrl}usuarios/logout?usuarioId={usuarioId}&macaddress={macaddress}";
            var response = await httpClient.PostAsync(url, null);
            return response.IsSuccessStatusCode;
            }

        public static async Task<(bool Success, string Message, CredencialesUsuarioDTO Usuario)> ValidarUsuarioAsync(string email, string pass, string macaddress)
            {
            string url = $"{App.BaseUrl}usuarios/validacion?email={email}&pass={pass}&macaddress={macaddress}";
            var result = await ExecuteRequestAsync<CredencialesUsuarioDTO>(() => httpClient.GetAsync(url), "Validar Usuario");

            if (result.Success && result.Data?.Token != null)
                {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Data.Token);
                }

            return (result.Success, result.Message, result.Data);
            }

        public static async Task<(bool Success, string Message, int PresupuestoID)> ProcesarPresupuestoAsync(ProcesaPresupuestoRequest request)
            {
            string url = $"{App.BaseUrl}presupuestos/procesar";
            var json = JsonSerializer.Serialize(request, jsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
                {
                var response = await httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    {
                    var result = JsonSerializer.Deserialize<ProcesarPresupuestoResult>(responseString, jsonSerializerOptions);
                    return (result.Success, result.Message, result.PresupuestoID);
                    }
                else
                    {
                    var error = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, jsonSerializerOptions);
                    string errorMessage = !string.IsNullOrEmpty(error?.Message)
                        ? error.Message
                        : !string.IsNullOrEmpty(error?.Mensaje)
                            ? error.Mensaje
                            : "Error desconocido";
                    return (false, errorMessage, 0);
                    }
                }
            catch (Exception ex)
                {
                return (false, $"Error: {ex.Message}", 0);
                }
            }

        public static async Task<(bool Success, string Message, List<PresupuestoDTO> Presupuestos)> ObtenerPresupuestosUsuarioAsync()
            {
            string url = $"{App.BaseUrl}presupuestos/usuario/{App.IdUsuario}";
            var (success, message, data) = await ExecuteRequestAsync<List<PresupuestoDTO>>(
                () => httpClient.GetAsync(url),
                $"Obtener presupuestos del usuario {App.IdUsuario}"
            );
            return (success, message, data ?? new List<PresupuestoDTO>());
            }


        public static async Task<(bool Success, string Message, List<ConceptoDTO> Conceptos, List<RelacionDTO> Relaciones)> ObtenerConceptosYRelacionesAsync(int presupuestoID)
            {
            string url = $"{App.BaseUrl}presupuestos/{presupuestoID}";
            var (success, message, data) = await ExecuteRequestAsync<ConceptosRelacionesResult>(
                () => httpClient.GetAsync(url),
                $"Obtener conceptos y relaciones del presupuesto {presupuestoID}"
            );

            if (success && data != null)
                return (true, "Operación exitosa.", data.Conceptos ?? new List<ConceptoDTO>(), data.Relaciones ?? new List<RelacionDTO>());
            else
                return (false, message, new List<ConceptoDTO>(), new List<RelacionDTO>());
            }


        public static async Task<(bool Success, string Message)> BorrarPresupuestoAsync(int presupuestoID)
            {
            string url = $"{App.BaseUrl}presupuestos/{presupuestoID}";
            try
                {
                var response = await httpClient.DeleteAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    {
                    var result = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, jsonSerializerOptions);
                    string message = !string.IsNullOrEmpty(result?.Message)
                        ? result.Message
                        : !string.IsNullOrEmpty(result?.Mensaje)
                            ? result.Mensaje
                            : "Presupuesto eliminado exitosamente.";
                    return (true, message);
                    }
                else
                    {
                    var error = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, jsonSerializerOptions);
                    string errorMessage = !string.IsNullOrEmpty(error?.Message)
                        ? error.Message
                        : !string.IsNullOrEmpty(error?.Mensaje)
                            ? error.Mensaje
                            : "Error desconocido al eliminar el presupuesto.";
                    return (false, errorMessage);
                    }
                }
            catch (Exception ex)
                {
                return (false, $"Error: {ex.Message}");
                }
            }


        // Obtener listas de artículos por usuario
        public static async Task<(bool Success, string Message, List<ArticulosListaDTO> Listas)> ObtenerListasArticulosPorUsuarioAsync(int usuarioID)
            {
            string url = $"{App.BaseUrl}insumos/articulos/listas/{usuarioID}";
            var (success, message, data) = await ExecuteRequestAsync<List<ArticulosListaDTO>>(
                () => httpClient.GetAsync(url),
                $"Obtener listas de artículos del usuario {usuarioID}"
            );
            return (success, message, data ?? new List<ArticulosListaDTO>());
            }

        // Obtener artículos de una lista específica por ListaID
        public static async Task<(bool Success, string Message, List<ArticuloDTO> Articulos)> ObtenerArticulosPorListaIDAsync(short listaID)
            {
            string url = $"{App.BaseUrl}insumos/articulos/lista/{listaID}";
            var (success, message, data) = await ExecuteRequestAsync<List<ArticuloDTO>>(
                () => httpClient.GetAsync(url),
                $"Obtener artículos de la lista {listaID}"
            );
            return (success, message, data ?? new List<ArticuloDTO>());
            }

        public static async Task<(bool Success, string Message, int ListaID)> CrearNuevaListaArticulosAsync(ArticulosListaDTO dto)
            {
            string url = $"{App.BaseUrl}insumos/articulos/lista/nueva";
            var json = JsonSerializer.Serialize(dto, jsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var (success, message, result) = await ExecuteRequestAsync<CrearListaArticulosResult>(
                () => httpClient.PostAsync(url, content),
                "Crear nueva lista de artículos"
            );

            // Si la respuesta es exitosa, usa el DTO; si no, el mensaje de error ya está en 'message'
            return (result?.Success ?? false, result?.Message ?? message, result?.ListaID ?? 0);
            }



        public static async Task<(bool Success, string Message)> ProcesarArticulosPorListaAsync(int listaID, List<ArticuloDTO> articulos)
            {
            string url = $"{App.BaseUrl}insumos/articulos/lista/procesar";
            var request = new ProcesarArticulosPorListaRequest { ListaID = listaID, Articulos = articulos };
            var json = JsonSerializer.Serialize(request, jsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var (success, message, result) = await ExecuteRequestAsync<ResultadoOperacion>(
                () => httpClient.PostAsync(url, content),
                "Procesar artículos por lista"
            );

            return (success, message);
            }

        public static async Task<(bool Success, string Message)> EliminarListaArticulosAsync(int listaID)
            {
            string url = $"{App.BaseUrl}insumos/articulos/lista/{listaID}";
            var (success, message, _) = await ExecuteRequestAsync<object>(
                () => httpClient.DeleteAsync(url),
                $"Eliminar lista de artículos {listaID}"
            );
            return (success, message);
            }

        public static async Task<(bool Success, string Message, List<ArticuloBusquedaDTO> Articulos)> BuscarArticulosAsync(int usuarioID, string tipoID, string descripBusqueda)
            {
            string url = $"{App.BaseUrl}insumos/articulos/busqueda?usuarioID={usuarioID}&tipoID={tipoID}&descripBusqueda={Uri.EscapeDataString(descripBusqueda)}";
            var (success, message, data) = await ExecuteRequestAsync<List<ArticuloBusquedaDTO>>(
                () => httpClient.GetAsync(url),
                $"Buscar artículos usuarioID={usuarioID}, tipoID={tipoID}, descripBusqueda={descripBusqueda}"
            );
            return (success, message, data ?? new List<ArticuloBusquedaDTO>());
            }

        public static async Task<(bool Success, string Message)> EditarArticulosPorListaAsync(int listaID, List<ArticuloDTO> articulos)
            {
            string url = $"{App.BaseUrl}insumos/articulos/lista/editar";
            var request = new ProcesarArticulosPorListaRequest { ListaID = listaID, Articulos = articulos };
            var json = JsonSerializer.Serialize(request, jsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var (success, message, result) = await ExecuteRequestAsync<ResultadoOperacion>(
                () => httpClient.PostAsync(url, content),
                "Editar artículos por lista"
            );

            return (success, message);
            }

        //Documentos

        public static async Task<(bool Success, string Message, List<GastoDTO> Gastos)> ObtenerGastosPorUsuarioAsync(int usuarioID)
            {
            string url = $"{App.BaseUrl}documentos/gastos/usuario/{usuarioID}";
            var (success, message, data) = await ExecuteRequestAsync<List<GastoDTO>>(
                () => httpClient.GetAsync(url),
                $"Obtener gastos del usuario {usuarioID}"
            );

            return (success, message, data ?? new List<GastoDTO>());
            }

        public static async Task<(bool Success, string Message, Biblioteca.DTO.ProcesarGastoResult Result)> ProcesarGastoAsync(Biblioteca.DTO.ProcesarGastoRequest request)
{
    string url = $"{App.BaseUrl}documentos/gastos/procesar";
    var json = JsonSerializer.Serialize(request, jsonSerializerOptions);
    var contentFactory = new Func<Task<HttpResponseMessage>>(() => httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")));

    var (success, message, data) = await ExecuteRequestAsync<Biblioteca.DTO.ProcesarGastoResult>(contentFactory, "Procesar Gasto");

    if (data == null)
        data = new Biblioteca.DTO.ProcesarGastoResult { DocumentoID = 0, PresupuestoIDs = new List<int>(), Resumenes = new List<Biblioteca.DTO.PresupuestoResumen>() };

    return (success, message, data);
}

        public static async Task<(bool Success, string Message, List<GastoDetalleDTO> Detalles)> ObtenerDetalleGastoAsync(int gastoID, bool esCobro = false)
            {
            var url = $"{App.BaseUrl}documentos/gastos/{gastoID}/detalle";
            if (esCobro)
                url += "?esCobro=1";

            try
                {
                var response = await httpClient.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    {
                    // Intentar parsear objeto de error estructurado; si no, devolver body crudo
                    try
                        {
                        var err = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, jsonSerializerOptions);
                        var msg = !string.IsNullOrEmpty(err?.Message) ? err.Message : (!string.IsNullOrEmpty(err?.Mensaje) ? err.Mensaje : responseString);
                        return (false, $"HTTP {(int)response.StatusCode}: {msg}", new List<GastoDetalleDTO>());
                        }
                    catch
                        {
                        return (false, $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}. Body: {responseString}", new List<GastoDetalleDTO>());
                        }
                    }

                try
                    {
                    var data = JsonSerializer.Deserialize<List<GastoDetalleDTO>>(responseString, jsonSerializerOptions);
                    return (true, string.Empty, data ?? new List<GastoDetalleDTO>());
                    }
                catch (JsonException jex)
                    {
                    // Devuelve el cuerpo crudo para depuración si no es JSON válido
                    return (false, $"Respuesta inválida JSON: {jex.Message}. Body: {responseString}", new List<GastoDetalleDTO>());
                    }
                }
            catch (HttpRequestException hex)
                {
                return (false, $"Error HTTP: {hex.Message}", new List<GastoDetalleDTO>());
                }
            catch (Exception ex)
                {
                return (false, $"Error: {ex.Message}", new List<GastoDetalleDTO>());
                }
            }

public static async Task<(bool Success, string Message, Biblioteca.DTO.ProcesarGastoResult Result)> BorrarGastoAsync(int gastoID, List<int>? presupuestos = null)
            {
            // Construir URL con query params repetidos: ?presupuestos=1&presupuestos=2...
            string url = $"{App.BaseUrl}documentos/gastos/{gastoID}";
            if (presupuestos != null && presupuestos.Any())
                {
                var qs = string.Join("&", presupuestos.Distinct().Select(p => $"presupuestos={p}"));
                url = $"{url}?{qs}";
                }

            try
                {
                var response = await httpClient.DeleteAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    {
                    // Leer mensaje si viene
                    string message = "Gasto eliminado correctamente.";
                    try
                        {
                        using var doc = JsonDocument.Parse(responseString);
                        if (doc.RootElement.TryGetProperty("Message", out var m) && m.ValueKind == JsonValueKind.String)
                            message = m.GetString() ?? message;
                        }
                    catch { /* ignorar parseo de Message */ }

                    // Intentar deserializar ProcesarGastoResult (DocumentoID, PresupuestoIDs, Resumenes)
                    try
                        {
                        var result = JsonSerializer.Deserialize<Biblioteca.DTO.ProcesarGastoResult>(responseString, jsonSerializerOptions)
                                     ?? new Biblioteca.DTO.ProcesarGastoResult
                                         {
                                         DocumentoID = 0,
                                         PresupuestoIDs = new List<int>(),
                                         Resumenes = new List<Biblioteca.DTO.PresupuestoResumen>()
                                         };

                        return (true, message, result);
                        }
                    catch (JsonException)
                        {
                        // Respuesta exitosa pero no con el formato esperado: devolver objeto vacío pero éxito
                        var empty = new Biblioteca.DTO.ProcesarGastoResult
                            {
                            DocumentoID = 0,
                            PresupuestoIDs = new List<int>(),
                            Resumenes = new List<Biblioteca.DTO.PresupuestoResumen>()
                            };
                        return (true, message, empty);
                        }
                    }
                else
                    {
                    // Error: intentar parsear mensaje de error estructurado
                    try
                        {
                        var error = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, jsonSerializerOptions);
                        string errorMessage = !string.IsNullOrEmpty(error?.Message)
                            ? error.Message
                            : !string.IsNullOrEmpty(error?.Mensaje)
                                ? error.Mensaje
                                : "Error desconocido al eliminar el gasto.";
                        return (false, errorMessage, new Biblioteca.DTO.ProcesarGastoResult
                            {
                            DocumentoID = 0,
                            PresupuestoIDs = new List<int>(),
                            Resumenes = new List<Biblioteca.DTO.PresupuestoResumen>()
                            });
                        }
                    catch
                        {
                        return (false, $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}", new Biblioteca.DTO.ProcesarGastoResult
                            {
                            DocumentoID = 0,
                            PresupuestoIDs = new List<int>(),
                            Resumenes = new List<Biblioteca.DTO.PresupuestoResumen>()
                            });
                        }
                    }
                }
            catch (Exception ex)
                {
                return (false, $"Error: {ex.Message}", new Biblioteca.DTO.ProcesarGastoResult
                    {
                    DocumentoID = 0,
                    PresupuestoIDs = new List<int>(),
                    Resumenes = new List<Biblioteca.DTO.PresupuestoResumen>()
                    });
                }
            }

        public static async Task<(bool Success, string Message, List<ConceptoDTO> Conceptos, List<RelacionDTO> Relaciones, List<GastoDetalleDTO> Detalles)>
    ObtenerConceptosRelacionesYDetallesAsync(int presupuestoID)
            {
            string url = $"{App.BaseUrl}control/presupuestos/{presupuestoID}/conceptos-relaciones-detalles";

            var (success, message, data) = await ExecuteRequestAsync<ConceptosRelacionesDetallesResponse>(
                () => httpClient.GetAsync(url),
                $"Obtener conceptos, relaciones y detalles del presupuesto {presupuestoID}"
            );

            // Normalizar listas vacías
            var conceptos = data?.Conceptos ?? new List<ConceptoDTO>();
            var relaciones = data?.Relaciones ?? new List<RelacionDTO>();
            var detalles = data?.Detalles ?? new List<GastoDetalleDTO>();

            // Calcular Importe en cliente según tu regla
            foreach (var d in detalles)
                {
                d.Importe = Math.Round(d.Cantidad * d.PrecioUnitario, 2, MidpointRounding.AwayFromZero);
                }

            return (success, message, conceptos, relaciones, detalles);
            }


        }

    public class ProcesarArticulosPorListaRequest
        {
        public int ListaID { get; set; }
        public List<ArticuloDTO> Articulos { get; set; }
        }

    public class ResultadoOperacion
        {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Mensaje { get; set; } // <-- Agregado para soportar ambos nombres
        }


    public class ProcesarPresupuestoResult
        {
        public bool Success { get; set; }
        public int PresupuestoID { get; set; }
        public string Message { get; set; }
        }

    public class CrearListaArticulosResult
        {
        public bool Success { get; set; }
        public int ListaID { get; set; }
        public string Message { get; set; }
        }

    public class ProcesarGastoRequest
        {
        public GastoDTO Gasto { get; set; }
        public List<GastoDetalleDTO> Detalles { get; set; } = new List<GastoDetalleDTO>();
        public List<int> PresupuestosAfectados { get; set; } = new List<int>();
        }

    public class ProcesarGastoResult
        {
        public int DocumentoID { get; set; }
        public List<int> PresupuestoIDs { get; set; } = new List<int>();
        public List<PresupuestoResumen> Resumenes { get; set; } = new List<PresupuestoResumen>();
        }

    public class PresupuestoResumen
        {
        public int PresupuestoID { get; set; }
        public string Moneda { get; set; } = string.Empty;
        public decimal TotalGasto { get; set; }
        public decimal TotalCobro { get; set; }
        }
    // DTO interno para deserializar la respuesta del endpoint /control
    public class ConceptosRelacionesDetallesResponse
        {
        [JsonPropertyName("Conceptos")]
        public List<ConceptoDTO>? Conceptos { get; set; }

        [JsonPropertyName("Relaciones")]
        public List<RelacionDTO>? Relaciones { get; set; }

        [JsonPropertyName("Detalles")]
        public List<GastoDetalleDTO>? Detalles { get; set; }
        }

    }
