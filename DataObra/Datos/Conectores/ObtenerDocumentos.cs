using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using DataObra.Documentos;
using Microsoft.Extensions.DependencyInjection;

namespace DataObra.Datos.Conectores
{
    public class ObtenerDocumentos
    {
        public string url = "https://webservicedataobra.azurewebsites.net/documentos";
        public HttpClient httpClient;
        public JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        public ObtenerDocumentos()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            var servicios = serviceCollection.BuildServiceProvider();
            var httpClientFactory = servicios.GetRequiredService<IHttpClientFactory>();
            httpClient = httpClientFactory.CreateClient();

        }

        public async Task<List<Documento>> ObtenerDocsAsync()
        {
            try
            {
                var respuesta = await httpClient.GetAsync(url);

                if (respuesta.IsSuccessStatusCode)
                {
                    var respuestaString = await respuesta.Content.ReadAsStringAsync();
                    var listadoDocumentos = JsonSerializer.Deserialize<List<Documento>>(respuestaString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return listadoDocumentos ?? new List<Documento>();  // Devuelve listado
                }
            }
            catch (Exception ex)
            {
                // Tabla de registro de errores
            }

            return new List<Documento>(); // Devuelve lista vacía
        }

    }
}

//public class DocumentoTest
//{
//    public int Id { get; set; }
//    public int Tipo { get; set; }
//    public int Numero { get; set; }
//    public string? Descripcion { get; set; }
//}
