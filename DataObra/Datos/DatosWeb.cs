﻿using System;
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


        public static async Task<(bool Success, string Message, int? Id)> CrearDocumentoAsync(DocumentoDTO documento)
        {
            string url = $"{App.BaseUrl}documentos/";
            var content = new StringContent(JsonSerializer.Serialize(documento), Encoding.UTF8, "application/json");
            var result = await ExecuteRequestAsync<CrearDocumentoResponse>(() => httpClient.PostAsync(url, content), "Crear Documento");

            return (result.Success, result.Message, result.Data?.Id);
        }

        public static async Task<(bool Success, string Message)> EliminarDocumentoAsync(int id)
        {
            string url = $"{App.BaseUrl}documentos/{id}";
            var result = await ExecuteRequestAsync<ResultadoOperacion>(() => httpClient.DeleteAsync(url), "Eliminar Documento");

            return (result.Success, result.Message);
        }

        public static async Task<(bool Success, string Message)> ActualizarDocumentoAsync(DocumentoDTO documento)
        {
            string url = $"{App.BaseUrl}documentos/";
            var content = new StringContent(JsonSerializer.Serialize(documento), Encoding.UTF8, "application/json");
            var result = await ExecuteRequestAsync<ResultadoOperacion>(() => httpClient.PutAsync(url, content), "Actualizar Documento");

            return (result.Success, result.Message);
        }

        public static async Task<(bool Success, string Message, List<DocumentoDTO> Documentos)> ObtenerDocumentosPorCuentaIDAsync(int cuentaID)
        {
            string url = $"{App.BaseUrl}documentos/cuenta/{cuentaID}";
            var result = await ExecuteRequestAsync<List<DocumentoDTO>>(() => httpClient.GetAsync(url), "Obtener Documentos por CuentaID");

            return (result.Success, result.Message, result.Data);
        }

        public static async Task<(bool Success, string Message)> ProcesarListaDetalleDocumentoAsync(List<DocumentoDetDTO> listaDetalleDocumento)
        {
            string url = $"{App.BaseUrl}documentosdet/procesar";
            var content = new StringContent(JsonSerializer.Serialize(listaDetalleDocumento), Encoding.UTF8, "application/json");
            var result = await ExecuteRequestAsync<ResultadoOperacion>(() => httpClient.PostAsync(url, content), "Procesar Lista Detalle Documento");

            return (result.Success, result.Message);
        }

        public static async Task<(bool Success, string Message, List<DocumentoDetDTO> Documentos)> ObtenerDocumentosDetPorCampoAsync(int id, string fieldName, short cuentaID)
        {
            string url = $"{App.BaseUrl}documentosdet/{fieldName}/{id}/{cuentaID}";
            var result = await ExecuteRequestAsync<List<DocumentoDetDTO>>(() => httpClient.GetAsync(url), "Obtener Documentos Det por Campo");

            return (result.Success, result.Message, result.Data);
        }

        public static async Task<(bool Success, string Message, List<Movimiento> Movimientos)> ObtenerMovimientosPorCampoAsync(int id, string fieldName, short cuentaID)
        {
            string url = $"{App.BaseUrl}movimientos/{fieldName}/{id}/{cuentaID}";
            var result = await ExecuteRequestAsync<List<Movimiento>>(() => httpClient.GetAsync(url), "Obtener Movimientos por Campo");

            return (result.Success, result.Message, result.Data);
        }

        public static async Task<(bool Success, string Message)> ProcesarMovimientosAsync(List<MovimientoDTO> listaMovimientos)
        {
            string url = $"{App.BaseUrl}movimientos/procesar";
            var content = new StringContent(JsonSerializer.Serialize(listaMovimientos), Encoding.UTF8, "application/json");
            var result = await ExecuteRequestAsync<ResultadoOperacion>(() => httpClient.PostAsync(url, content), "Procesar Movimientos");

            return (result.Success, result.Message);
        }

        public static async Task<(bool Success, string Message, List<Impuesto> Impuestos)> ObtenerImpuestosPorCampoAsync(int id, string fieldName, short cuentaID)
        {
            string url = $"{App.BaseUrl}impuestos/{fieldName}/{id}/{cuentaID}";
            var result = await ExecuteRequestAsync<List<Impuesto>>(() => httpClient.GetAsync(url), "Obtener Impuestos por Campo");

            return (result.Success, result.Message, result.Data);
        }

        public static async Task<(bool Success, string Message)> ProcesarImpuestosAsync(List<ImpuestoDTO> listaImpuestos)
        {
            string url = $"{App.BaseUrl}impuestos/procesar";
            var content = new StringContent(JsonSerializer.Serialize(listaImpuestos), Encoding.UTF8, "application/json");
            var result = await ExecuteRequestAsync<ResultadoOperacion>(() => httpClient.PostAsync(url, content), "Procesar Impuestos");

            return (result.Success, result.Message);
        }

        public static async Task<(bool Success, string Message, int? Id)> InsertarAgrupadorAsync(AgrupadorDTO agrupador)
        {
            string url = $"{App.BaseUrl}agrupadores/";
            var content = new StringContent(JsonSerializer.Serialize(agrupador), Encoding.UTF8, "application/json");
            var result = await ExecuteRequestAsync<CrearDocumentoResponse>(() => httpClient.PostAsync(url, content), "Insertar Agrupador");

            return (result.Success, result.Message, result.Data?.Id);
        }

        public static async Task<(bool Success, string Message, List<AgrupadorDTO> Agrupadores)> ObtenerAgrupadoresPorCuentaIDAsync(int cuentaID)
        {
            string url = $"{App.BaseUrl}agrupadores/cuenta/{cuentaID}";
            var result = await ExecuteRequestAsync<List<AgrupadorDTO>>(() => httpClient.GetAsync(url), "Obtener Agrupadores por CuentaID");
            return (result.Success, result.Message, result.Data);
        }

        public static async Task<(bool Success, string Message)> ActualizarAgrupadorAsync(AgrupadorDTO agrupador)
        {
            string url = $"{App.BaseUrl}agrupadores/";
            var content = new StringContent(JsonSerializer.Serialize(agrupador), Encoding.UTF8, "application/json");
            var result = await ExecuteRequestAsync<ResultadoOperacion>(() => httpClient.PutAsync(url, content), "Actualizar Agrupador");
            return (result.Success, result.Message);
        }

        public static async Task<(bool Success, string Message)> EliminarAgrupadorAsync(int id)
        {
            string url = $"{App.BaseUrl}agrupadores/{id}";
            var result = await ExecuteRequestAsync<ResultadoOperacion>(() => httpClient.DeleteAsync(url), "Eliminar Agrupador");
            return (result.Success, result.Message);
        }

        public static async Task<(bool Success, string Message, (List<ConceptoDTO> Conceptos, List<RelacionDTO> Relaciones))> ObtenerRegistrosPorPresupuestoIDAsync(int presupuestoID)
        {
            string url = $"{App.BaseUrl}presupuestos/{presupuestoID}";
            var result = await ExecuteRequestAsync<PresupuestoResponse>(() => httpClient.GetAsync(url), "Obtener Registros por PresupuestoID");

            return (result.Success, result.Message, (result.Data?.Conceptos, result.Data?.Relaciones));
        }

        public static async Task<(bool Success, string Message)> ProcesarArbolPresupuestoAsync(ProcesaPresupuestoDTO request)
        {
            string url = $"{App.BaseUrl}presupuestos/procesar";
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var result = await ExecuteRequestAsync<ResultadoOperacion>(() => httpClient.PostAsync(url, content), "Procesar Árbol de Presupuesto");

            return (result.Success, result.Message);
        }

        public static async Task<(bool Success, string Message)> EliminarPresupuestoAsync(int presupuestoID, bool verifica)
        {
            string url = $"{App.BaseUrl}presupuestos/{presupuestoID}?verifica={verifica.ToString().ToLower()}";
            var result = await ExecuteRequestAsync<ResultadoOperacion>(() => httpClient.DeleteAsync(url), "Eliminar Presupuesto");

            return (result.Success, result.Message);
        }

        public class ResultadoOperacion
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string Mensaje { get; set; } // <-- Agregado para soportar ambos nombres
        }

        // Clase para deserializar la respuesta de CrearDocumentoAsync
        private class CrearDocumentoResponse
        {
            public int Id { get; set; }
            public string Message { get; set; }
        }

        private class PresupuestoResponse
        {
            public List<ConceptoDTO> Conceptos { get; set; }
            public List<RelacionDTO> Relaciones { get; set; }
        }
    }
}

