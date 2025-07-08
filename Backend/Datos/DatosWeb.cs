using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Biblioteca;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Data;
using System.Windows;
using System.Collections.ObjectModel;
using System.Security.Policy;
using Biblioteca.DTO;

namespace Backend.Datos
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

        public static async Task<(bool Success, string Message, CredencialesUsuarioDTO Usuario)> ValidarUsuarioAsync(string email, string pass)
            {
            string url = $"{App.BaseUrl}usuarios/validacion?email={email}&pass={pass}";
            var result = await ExecuteRequestAsync<CredencialesUsuarioDTO>(() => httpClient.GetAsync(url), "Validar Usuario");

            if (result.Success && result.Data?.Token != null)
                {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Data.Token);
                }

            return (result.Success, result.Message, result.Data);
            }

        // Obtener insumos por usuario
        public static async Task<(bool Success, string Message, List<InsumoDTO> Insumos)> ObtenerInsumosPorUsuarioAsync(int usuarioID)
            {
            string url = $"{App.BaseUrl}insumos/usuario/{usuarioID}";
            var result = await ExecuteRequestAsync<List<InsumoDTO>>(() => httpClient.GetAsync(url), $"Obtener insumos usuario {usuarioID}");
            return (result.Success, result.Message, result.Data);
            }

        // Procesar (insertar/actualizar) un insumo y sus artículos relacionados
        public static async Task<(bool Success, string Message, int? InsumoID)> ProcesarInsumoAsync(InsumoDTO insumo, List<ArticuloRelDTO> articulos)
            {
            string url = $"{App.BaseUrl}insumos/procesar";
            var request = new
                {
                Insumo = insumo,
                Articulos = articulos
                };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var result = await ExecuteRequestAsync<JsonElement>(() => httpClient.PostAsync(url, content), "Procesar insumo");
            int? id = null;
            if (result.Success && result.Data.TryGetProperty("InsumoID", out var idProp))
                id = idProp.GetInt32();
            return (result.Success, result.Message, id);
            }

        // Eliminar un insumo y sus artículos relacionados
        public static async Task<(bool Success, string Message)> EliminarInsumoYArticulosRelAsync(int insumoID)
            {
            string url = $"{App.BaseUrl}insumos/{insumoID}";
            var result = await ExecuteRequestAsync<JsonElement>(() => httpClient.DeleteAsync(url), $"Eliminar insumo {insumoID}");
            return (result.Success, result.Message);
            }

        // Procesar (insertar/actualizar) una lista de artículos y sus artículos
        public static async Task<(bool Success, string Message, int? ListaID)> ProcesarArticulosListaAsync(ArticulosListaDTO lista, List<ArticuloDTO> articulos)
            {
            string url = $"{App.BaseUrl}insumos/articulos/procesar";
            var request = new
                {
                Lista = lista,
                Articulos = articulos
                };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var result = await ExecuteRequestAsync<JsonElement>(() => httpClient.PostAsync(url, content), "Procesar lista de artículos");
            int? id = null;
            if (result.Success && result.Data.TryGetProperty("ListaID", out var idProp))
                id = idProp.GetInt32();
            return (result.Success, result.Message, id);
            }

        // Eliminar una lista de artículos y sus artículos relacionados
        public static async Task<(bool Success, string Message)> EliminarArticulosListaYArticulosAsync(int listaID)
            {
            string url = $"{App.BaseUrl}insumos/articulos/{listaID}";
            var result = await ExecuteRequestAsync<JsonElement>(() => httpClient.DeleteAsync(url), $"Eliminar lista de artículos {listaID}");
            return (result.Success, result.Message);
            }

        // Obtener artículos por insumo
        public static async Task<(bool Success, string Message, List<ArticuloDTO> Articulos)> ObtenerArticulosPorInsumoAsync(int insumoID)
            {
            string url = $"{App.BaseUrl}insumos/articulos/{insumoID}";
            var result = await ExecuteRequestAsync<List<ArticuloDTO>>(
                () => httpClient.GetAsync(url),
                $"Obtener artículos por insumo {insumoID}"
            );
            return (result.Success, result.Message, result.Data);
            }

        }

    public class ResultadoOperacion
        {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Mensaje { get; set; } // <-- Agregado para soportar ambos nombres
        }

    }
