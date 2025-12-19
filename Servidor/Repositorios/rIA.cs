using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace Servidor.Repositorios
    {
    public class rIA
        {
        private readonly string _connectionString;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiVersion;
        private readonly bool _isAzure;

        public rIA(string connectionString, IHttpClientFactory httpClientFactory, IConfiguration configuration)
            {
            _connectionString = connectionString;
            _httpClientFactory = httpClientFactory;
            _apiVersion = configuration["OpenAI:ApiVersion"] ?? string.Empty;

            var baseUrl = configuration["OpenAI:BaseUrl"] ?? string.Empty;
            _isAzure = baseUrl.Contains(".azure.com", StringComparison.OrdinalIgnoreCase);
            }

        /// <summary>
        /// Llama al proveedor IA (cliente "openai") y devuelve el desglose de análisis de costo.
        /// </summary>
        public async Task<List<AnalisisCostoItemDto>> AnalizarCostoAsync(string descripcion, CancellationToken ct = default)
            {
            if (string.IsNullOrWhiteSpace(descripcion))
                return new List<AnalisisCostoItemDto>();

            var prompt = $@"
Eres un analista de costos de construcción. Desglosa insumos y mano de obra para la tarea:
""{descripcion}""
Responde en JSON con lista de items con campos:
Id (corto), Tipo (M/D/E/S/O), Descripcion, Unidad, Cantidad (por unidad de obra), PU1 (Precio unitario decimal). 
Obtén los precios unitarios actualizados del mercado en Argentina (diciembre 2025). 
No dejes PU1 en cero: si no hay datos exactos, usa valores de referencia de mercado.

Ejemplo:
[
  {{ ""Id"": ""CEMENTO"", ""Tipo"": ""M"", ""Descripcion"": ""Cemento portland"", ""Unidad"": ""kg/m2"", ""Cantidad"": 3.0, ""PU1"": 0, ""PU2"": 0 }},
  {{ ""Id"": ""ARENA"", ""Tipo"": ""M"", ""Descripcion"": ""Arena fina"", ""Unidad"": ""m3/m2"", ""Cantidad"": 0.012, ""PU1"": 0, ""PU2"": 0 }},
  {{ ""Id"": ""OFICIAL"", ""Tipo"": ""D"", ""Descripcion"": ""Oficial albañil"", ""Unidad"": ""h/m2"", ""Cantidad"": 0.40, ""PU1"": 0, ""PU2"": 0 }},
  {{ ""Id"": ""AYUDANTE"", ""Tipo"": ""D"", ""Descripcion"": ""Ayudante"", ""Unidad"": ""h/m2"", ""Cantidad"": 0.35, ""PU1"": 0, ""PU2"": 0 }}
]";

            var client = _httpClientFactory.CreateClient("openai");

            // Azure requiere api-version; OpenAI estándar no.
            string path = string.IsNullOrWhiteSpace(_apiVersion)
                ? "chat/completions"
                : $"chat/completions?api-version={_apiVersion}";

            // En Azure (deployment-scoped) no es necesario enviar "model"; en OpenAI estándar sí.
            object payload = _isAzure
                ? new
                    {
                    messages = new[]
                    {
                        new { role = "system", content = "Responde solo JSON válido de la lista solicitada. No agregues texto fuera del JSON." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.2
                    }
                : new
                    {
                    model = "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content = "Responde solo JSON válido de la lista solicitada. No agregues texto fuera del JSON." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.2
                    };

            using var req = new HttpRequestMessage(HttpMethod.Post, path)
                {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                };

            using var resp = await client.SendAsync(req, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);

            using var doc = JsonDocument.Parse(json);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "[]";

            var options = new JsonSerializerOptions
                {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

            var items = JsonSerializer.Deserialize<List<AnalisisCostoItemDto>>(content, options) ?? new();

            foreach (var i in items)
                {
                i.Tipo = NormalizarTipo(i.Tipo);
                i.Unidad = i.Unidad?.Trim();
                if (i.PU1 < 0) i.PU1 = 0;
                if (i.PU2 < 0) i.PU2 = 0;
                }

            return items;
            }

        private static string NormalizarTipo(string? tipo)
            {
            if (string.IsNullOrWhiteSpace(tipo)) return "O";
            var t = tipo.Trim().ToUpperInvariant();
            return t switch
                {
                    "MATERIALES" or "M" => "M",
                    "MANO DE OBRA" or "MO" or "D" => "D",
                    "EQUIPOS" or "E" => "E",
                    "SUBCONTRATOS" or "S" => "S",
                    "OTROS" or "O" => "O",
                    _ => "O"
                    };
            }

        public sealed class AnalisisCostoItemDto
            {
            public string Id { get; set; } = "";
            public string Tipo { get; set; } = "O";
            public string Descripcion { get; set; } = "";
            public string Unidad { get; set; } = "";
            public decimal Cantidad { get; set; }
            public decimal PU1 { get; set; }
            public decimal PU2 { get; set; }
            }
        }
    }