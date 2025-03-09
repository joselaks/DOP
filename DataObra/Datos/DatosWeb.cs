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

namespace DataObra.Datos
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

        // Procedimiento para validar usuario. Una vez verificado graba el Token en el httpClient para las próximas consultas
        public static async Task<(bool Success, string Message, CredencialesUsuarioDTO Usuario)> ValidarUsuarioAsync(string email, string pass)
        {
            string url = $"{App.BaseUrl}usuarios/validacion?email={email}&pass={pass}";
            DateTime sendTime = DateTime.Now;
            LogEntries.Add($"{sendTime:yyyy-MM-dd HH:mm:ss.fff} - Enviado - Validar Usuario");

            try
            {
                var response = await httpClient.GetAsync(url);
                DateTime receiveTime = DateTime.Now;
                TimeSpan duration = receiveTime - sendTime;

                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var usuario = JsonSerializer.Deserialize<CredencialesUsuarioDTO>(responseString, jsonSerializerOptions);

                if (usuario.Token != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);
                    LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Validar Usuario - Éxito - Duración: {duration.TotalMilliseconds} ms");
                    return (true, "Usuario validado exitosamente.", usuario);
                }

                LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Validar Usuario - Usuario inexistente - Duración: {duration.TotalMilliseconds} ms");
                return (true, "Usuario inexistente.", usuario);
            }
            catch (HttpRequestException httpEx)
            {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Validar Usuario - Error HTTP: {httpEx.Message} - Duración: {duration.TotalMilliseconds} ms");
                return (false, $"Error HTTP: {httpEx.Message}", null);
            }
            catch (Exception ex)
            {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Validar Usuario - Error: {ex.Message} - Duración: {duration.TotalMilliseconds} ms");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public static async Task<(bool Success, string Message, int? Id)> CrearDocumentoAsync(DocumentoDTO documento)
        {
            string url = $"{App.BaseUrl}documentos/";
            DateTime sendTime = DateTime.Now;
            LogEntries.Add($"{sendTime:yyyy-MM-dd HH:mm:ss.fff} - Enviado - Crear Documento");

            var content = new StringContent(JsonSerializer.Serialize(documento), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(url, content);
                DateTime receiveTime = DateTime.Now;
                TimeSpan duration = receiveTime - sendTime;

                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                // Suponiendo que el responseString es un objeto JSON con una propiedad "Id"
                var responseObject = JsonSerializer.Deserialize<CrearDocumentoResponse>(responseString, jsonSerializerOptions);
                var nuevoDocumentoID = responseObject.Id;

                // Agregar entrada de log con el ID del documento creado
                LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Crear Documento - ID del documento creado: {nuevoDocumentoID} - Duración: {duration.TotalMilliseconds} ms");

                return (true, $"Documento insertado con ID: {nuevoDocumentoID}", nuevoDocumentoID);
            }
            catch (HttpRequestException httpEx)
            {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Crear Documento - Error HTTP: {httpEx.Message} - Duración: {duration.TotalMilliseconds} ms");
                return (false, $"Error HTTP: {httpEx.Message}", null);
            }
            catch (Exception ex)
            {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Crear Documento - Error: {ex.Message} - Duración: {duration.TotalMilliseconds} ms");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public static async Task<(bool Success, string Message)> EliminarDocumentoAsync(int id)
        {
            string url = $"{App.BaseUrl}documentos/{id}";
            DateTime sendTime = DateTime.Now;
            LogEntries.Add($"{sendTime:yyyy-MM-dd HH:mm:ss.fff} - Enviado - Eliminar Documento");

            try
            {
                var response = await httpClient.DeleteAsync(url);
                DateTime receiveTime = DateTime.Now;
                TimeSpan duration = receiveTime - sendTime;

                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, jsonSerializerOptions);

                // Agregar entrada de log con el resultado de la operación
                LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Eliminar Documento - {resultado.Message} - Duración: {duration.TotalMilliseconds} ms");

                return (resultado.Success, resultado.Message);
            }
            catch (HttpRequestException httpEx)
            {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Eliminar Documento - Error HTTP: {httpEx.Message} - Duración: {duration.TotalMilliseconds} ms");
                return (false, $"Error HTTP: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Eliminar Documento - Error: {ex.Message} - Duración: {duration.TotalMilliseconds} ms");
                return (false, $"Error: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> ActualizarDocumentoAsync(DocumentoDTO documento)
        {
            string url = $"{App.BaseUrl}documentos/";
            DateTime sendTime = DateTime.Now;
            LogEntries.Add($"{sendTime:yyyy-MM-dd HH:mm:ss.fff} - Enviado - Actualizar Documento");

            var content = new StringContent(JsonSerializer.Serialize(documento), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PutAsync(url, content);
                DateTime receiveTime = DateTime.Now;
                TimeSpan duration = receiveTime - sendTime;

                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON
                var resultado = JsonSerializer.Deserialize<ResultadoOperacion>(responseString, jsonSerializerOptions);

                // Agregar entrada de log con el resultado de la operación
                LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Actualizar Documento - {resultado.Message} - Duración: {duration.TotalMilliseconds} ms");

                return (resultado.Success, resultado.Message);
            }
            catch (HttpRequestException httpEx)
            {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Actualizar Documento - Error HTTP: {httpEx.Message} - Duración: {duration.TotalMilliseconds} ms");
                return (false, $"Error HTTP: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Actualizar Documento - Error: {ex.Message} - Duración: {duration.TotalMilliseconds} ms");
                return (false, $"Error: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message, List<DocumentoDTO> Documentos)> ObtenerDocumentosPorCuentaIDAsync(int cuentaID)
        {
            string url = $"{App.BaseUrl}documentos/cuenta/{cuentaID}";
            DateTime sendTime = DateTime.Now;
            LogEntries.Add($"{sendTime:yyyy-MM-dd HH:mm:ss.fff} - Enviado - Obtener Documentos por CuentaID");

            try
            {
                var response = await httpClient.GetAsync(url);
                DateTime receiveTime = DateTime.Now;
                TimeSpan duration = receiveTime - sendTime;

                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var documentos = JsonSerializer.Deserialize<List<DocumentoDTO>>(responseString, jsonSerializerOptions);

                LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Obtener Documentos por CuentaID - Duración: {duration.TotalMilliseconds} ms");
                return (true, "Documentos obtenidos exitosamente.", documentos);
            }
            catch (HttpRequestException httpEx)
            {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Obtener Documentos por CuentaID - Error HTTP: {httpEx.Message} - Duración: {duration.TotalMilliseconds} ms");
                return (false, $"Error HTTP: {httpEx.Message}", null);
            }
            catch (Exception ex)
            {
                DateTime errorTime = DateTime.Now;
                TimeSpan duration = errorTime - sendTime;
                LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - Obtener Documentos por CuentaID - Error: {ex.Message} - Duración: {duration.TotalMilliseconds} ms");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public class ResultadoOperacion
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        // Clase para deserializar la respuesta de CrearDocumentoAsync
        private class CrearDocumentoResponse
        {
            public int Id { get; set; }
            public string Message { get; set; }
        }
    }
}
