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

namespace WebEjemplos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            var url = "https://webservicedataobra.azurewebsites.net/documentos";
            var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        }

        private async void Listar_Click(object sender, RoutedEventArgs e)
        {
            var url = "https://webservicedataobra.azurewebsites.net/documentos";
            using (var httpClient = new HttpClient())
            {
                var respuesta = await httpClient.GetAsync(url);
                var respestaString = await respuesta.Content.ReadAsStringAsync();
                var listadoDocumentos = JsonSerializer.Deserialize<List<Documento>>(respestaString,
                    new JsonSerializerOptions() {PropertyNameCaseInsensitive=true});

            };

        }

        private void Modificar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {

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