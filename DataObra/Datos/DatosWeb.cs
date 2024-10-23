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

namespace DataObra.Datos
{
    public class DatosWeb
    {
        public HttpClient httpClient;
        public JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        public DatosWeb()
        {
            // En cualquier parte de tu aplicación
            httpClient = ((App)Application.Current).HttpClient;

        }

        // Procedimiento para validar usuario. Una vez verificado graba el Token en el httpClient para las próximas consultas
        public async Task<(bool Success, string Message, CredencialesUsuario Usuario)> ValidarUsuarioAsync(string email, string pass)
        {
            string url = $"https://localhost:7255/usuarios/validacion?email={email}&pass={pass}";
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var usuario = JsonSerializer.Deserialize<CredencialesUsuario>(responseString, jsonSerializerOptions);
                if (usuario.Token != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);
                    return (true, "Usuario validado exitosamente.", usuario);

                }
                return (true, "Usuario inexistene.", usuario);
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

        // Procedimiento para obtener todos los documentos de una cuenta
        public async Task<(bool Success, string Message, List<Documento> Documentos)> GetDocumentosPorCuentaIDAsync(short cuentaID)
        {
            //var url = $"https://localhost:7255/documentos/cuenta/{cuentaID}";
            var url = $"{App.BaseUrl}documentos/cuenta/{cuentaID}";

            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var documentos = JsonSerializer.Deserialize<List<Documento>>(responseString, jsonSerializerOptions);
                return (true, "Documentos obtenidos exitosamente.", documentos);
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

        // Procedimiento que inserta un nuevo documento en la base de datos
        public async Task<(bool Success, string Message)> PostDocumentoAsync(Documento documento)
        {
            var url = "https://localhost:7255/documentos/";
            var content = new StringContent(JsonSerializer.Serialize(documento), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var nuevoDocumentoID = JsonSerializer.Deserialize<int>(responseString, jsonSerializerOptions);

                return (true, $"Documento insertado con ID: {nuevoDocumentoID}");
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

        // Procedimiento que modifica un docuemnto existente en la base de datos
        public async Task<(bool Success, string Message)> PutDocumentoAsync(Documento documento)
        {
            var url = $"https://localhost:7255/documentos/{documento.ID}";
            var content = new StringContent(JsonSerializer.Serialize(documento), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
                return (true, "Documento actualizado exitosamente.");
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

        // Procedimiento que borra un documento de la base de datos
        public async Task<(bool Success, string Message)> DeleteDocumentoAsync(int id)
        {
            var url = $"https://localhost:7255/documentos/{id}";

            try
            {
                var response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                return (true, "Documento eliminado exitosamente.");
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

        // Procedimiento que obtiene un documento con su ID
        public async Task<(bool Success, string Message, Documento Documento)> ObtenerDocumentoPorIDAsync(int id)
        {
            var url = $"https://localhost:7255/documentos/id/{id}";

            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var documento = JsonSerializer.Deserialize<Documento>(responseString, jsonSerializerOptions);
                return (true, "Documento obtenido exitosamente.", documento);
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
    }
}

