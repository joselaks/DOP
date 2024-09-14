using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace WebEjemplos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string url = "https://webservicedataobra.azurewebsites.net/documentos";
        public HttpClient httpClient;
        public JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        public MainWindow()
        {
            InitializeComponent();
            ServiceCollection serviceCollection = new ServiceCollection();
            Configure(serviceCollection);
            var servicios = serviceCollection.BuildServiceProvider();
            var httpClientFactory = servicios.GetRequiredService<IHttpClientFactory>();
            httpClient = httpClientFactory.CreateClient();
        }

        private async void Agregar_Click(object sender, RoutedEventArgs e)
        {
                var documento = new Documento() { Tipo = 2, Numero = 100, Descripcion = "Prueba1" };
                var respuesta = await httpClient.PostAsJsonAsync(url, documento);
                if (respuesta.IsSuccessStatusCode)
                {
                    var cuerpo = await respuesta.Content.ReadAsStringAsync();
                    MessageBox.Show("El id es " + cuerpo);
                }
        }

        private async void Listar_Click(object sender, RoutedEventArgs e)
        {
                var respuesta = await httpClient.GetAsync(url);
                var respestaString = await respuesta.Content.ReadAsStringAsync();
                var listadoDocumentos = JsonSerializer.Deserialize<List<Documento>>(respestaString,
                    new JsonSerializerOptions() {PropertyNameCaseInsensitive=true});

        }

        private async void Modificar_Click(object sender, RoutedEventArgs e)
        {
                var documento = new Documento() { Id = 2, Tipo = 5, Numero = 500, Descripcion = "Modificado" };
                await httpClient.PutAsJsonAsync($"{url}/{documento.Id}", documento);
        }

        private async void Borrar_Click(object sender, RoutedEventArgs e)
        {
                var documento = new Documento() { Id = 5, Tipo = 5, Numero = 500, Descripcion = "Modificado" };
                await httpClient.DeleteAsync($"{url}/{documento.Id}");
        }

        public static void Configure(ServiceCollection services)
        { 
            services.AddHttpClient();
        }


    }

    public class Documento
    {
        public int Id { get; set; }
        public int Tipo { get; set; }
        public int Numero { get; set; }
        public string? Descripcion { get; set; }
    }
}