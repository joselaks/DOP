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
using System.Text.RegularExpressions;
using System;

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
        Random random = new Random();

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
            try
            {
                IndicadorEspera.Visibility = Visibility.Visible;
                var documento = new Documento() { Tipo = random.Next(1, 11), Numero = random.Next(500,2400), Descripcion = GenerarTextoAleatorio(insumosConstruccion) };
                var respuesta = await httpClient.PostAsJsonAsync(url, documento);
                
                if (respuesta.IsSuccessStatusCode)
                {
                    var cuerpo = await respuesta.Content.ReadAsStringAsync();
                    var primerEntero = Regex.Match(cuerpo, @"\d+").Value;
                    MessageBox.Show("El ID es: " + primerEntero + "\n\nDel documento: \n" + cuerpo);
                    Listar_Click(null, null); // Actualiza listado
                }
                else
                {
                    MessageBox.Show("Error al agregar el documento");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}");
            }
            finally
            {
                IndicadorEspera.Visibility = Visibility.Collapsed;
            }
        }


        private async void Listar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IndicadorEspera.Visibility = Visibility.Visible;
                var respuesta = await httpClient.GetAsync(url);
                if (respuesta.IsSuccessStatusCode)
                {
                    var respestaString = await respuesta.Content.ReadAsStringAsync();
                    var listadoDocumentos = JsonSerializer.Deserialize<List<Documento>>(respestaString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    dataGrid.ItemsSource = listadoDocumentos;
                }
                else
                {
                    MessageBox.Show("Error al listar los documentos");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}");
            }
            finally
            {
                IndicadorEspera.Visibility = Visibility.Collapsed;
            }
        }


        private async void Modificar_Click(Documento documento)
        {
            try
            {
                IndicadorEspera.Visibility = Visibility.Visible;
                documento.Descripcion = GenerarTextoAleatorio(insumosConstruccion);
                await httpClient.PutAsJsonAsync($"{url}/{documento.Id}", documento);
                MessageBox.Show("Documento modificado exitosamente");
                Listar_Click(null, null); // Actualiza listado
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}");
            }
            finally
            {
                IndicadorEspera.Visibility = Visibility.Collapsed;
            }
        }

        private async void Borrar_Click(Documento documento)
        {
            try
            {
                IndicadorEspera.Visibility = Visibility.Visible;
                await httpClient.DeleteAsync($"{url}/{documento.Id}");
                MessageBox.Show("Documento borrado exitosamente");
                Listar_Click(null, null); // Actualiza listado
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}");
            }
            finally
            {
                IndicadorEspera.Visibility = Visibility.Collapsed;
            }
        }

        private async void Obtener_Click(object sender, RoutedEventArgs e)
        {
            int idDoc = 1;
            var respuesta = await httpClient.GetAsync($"{url}/{idDoc}");
            var respestaString = await respuesta.Content.ReadAsStringAsync();
            var Documentos = JsonSerializer.Deserialize<Documento>(respestaString,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public static void Configure(ServiceCollection services)
        { 
            services.AddHttpClient();
        }

        #region Complementos

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dataGrid.SelectedItem is Documento documentoSeleccionado)
            {
                Borrar_Click(documentoSeleccionado);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem is Documento documentoSeleccionado)
            {
                Modificar_Click(documentoSeleccionado);
            }
        }

        public static string GenerarTextoAleatorio(string[] palabras)
        {
            Random random = new Random();
            var palabrasAleatorias = palabras.OrderBy(x => random.Next()).Take(2);
            return string.Join(" ", palabrasAleatorias);
        }

        private static readonly string[] insumosConstruccion = {
            "cemento", "arena", "grava", "ladrillo", "bloque", "madera", "acero", "vidrio", "yeso", "pintura",
            "clavo", "tornillo", "tuerca", "arandela", "cable", "tubería", "válvula", "grifo", "enchufe", "interruptor",
            "cemento armado", "hormigón", "mortero", "cal", "teja", "azulejo", "cerámica", "mármol", "granito", "pizarra",
            "aislante", "impermeabilizante", "sellador", "adhesivo", "silicona", "masilla", "barniz", "lacado", "tapajuntas", "rejilla",
            "andamio", "escalera", "carretilla", "taladro", "martillo", "sierra", "cincel", "nivel", "metro", "escuadra"
        };

        #endregion
    }

    public class Documento
    {
        public int Id { get; set; }
        public int Tipo { get; set; }
        public int Numero { get; set; }
        public string? Descripcion { get; set; }
    }



}