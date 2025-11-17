using Biblioteca.DTO;
using DataObra.Documentos.Ventanas;
using DataObra.Interfaz.Ventanas;
using DataObra.Presupuestos.Ventanas;
using DOP;
using DOP.Datos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.IO.Compression;
using Syncfusion.XlsIO;

namespace DataObra.Interfaz.Componentes
    {
    /// <summary>
    /// Lógica de interacción para xGastos.xaml
    /// </summary>
    public partial class xGastos : UserControl
        {
        private WiEscritorio escritorio;
        public ObservableCollection<GastoDTO> _gastos = new();
        public xGastos(WiEscritorio escritorio)
            {
            InitializeComponent();
            this.escritorio = escritorio;
            this.Loaded += XGastos_Loaded;


            }

        private async void XGastos_Loaded(object sender, RoutedEventArgs e)
            {
            try
                {
                // Obtener gastos del usuario (usa App.IdUsuario como en otros helpers)
                var (success, message, gastos) = await DatosWeb.ObtenerGastosPorUsuarioAsync(App.IdUsuario);

                if (success)
                    {
                    _gastos.Clear();
                    foreach (var g in gastos)
                        _gastos.Add(g);

                    // Asegura que la vista pueda enlazar a la colección
                    GrillaGastos.ItemsSource = _gastos;
                    }
                else
                    {
                    MessageBox.Show(message ?? "No se pudieron obtener los gastos.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error al obtener gastos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private void Button_Click(object sender, RoutedEventArgs e)
            {
            escritorio.CambioEstado("nGastos", "Normal", "O");
            }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
            {
            var win = new WiGasto(_gastos, null, null);
            win.ShowDialog();

            }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
            {
            // Pattern matching para extraer un ID válido (int no nullable)
            if (GrillaGastos.SelectedItem is GastoDTO { ID: var id } seleccionado && id > 0)
                {
                try
                    {
                    bool esCobro = false;
                    if (seleccionado.TipoID == 20)
                        {
                        esCobro = true;
                        }


                    // Obtener detalles del gasto/cobro (usa DatosWeb helper con el flag)
                    var (success, message, detalles) = await DatosWeb.ObtenerDetalleGastoAsync(id, esCobro);
                    if (!success)
                        {
                        MessageBox.Show($"No se pudieron obtener los detalles del {(esCobro ? "cobro" : "gasto")}.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                        }

                    // Abrir ventana de edición con los detalles recuperados
                    var win = new WiGasto(_gastos, seleccionado, detalles);
                    win.ShowDialog();
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show($"Error al editar {(seleccionado.TipoID == 20 ? "cobro" : "gasto")}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            else
                {
                MessageBox.Show("Seleccione un gasto válido para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
            {

            }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
            {

            }

        private void GrillaPresupuestos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {

            }

        private async void btnControl_Click(object sender, RoutedEventArgs e)
            {
            string apiKey = "ak_Cg6FNLTUjpGl7eaMSIdr"; // Tu API Key
            string url = $"https://api.alphacast.io/datasets/5902/data?from=2025-01-01&to=2025-02-28&filter=variable:Materiales";
            try
                {
                // Forzar handler que descomprima gzip/deflate automáticamente
                var handler = new HttpClientHandler
                    {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                    };

                using var client = new HttpClient(handler);

                // Autenticación con Basic Auth
                var byteArray = Encoding.ASCII.GetBytes($"{apiKey}:");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                // Aceptar JSON
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    {
                    string prettyJson;
                    try
                        {
                        using var doc = JsonDocument.Parse(json);
                        prettyJson = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
                        }
                    catch (Exception ex)
                        {
                        prettyJson = $"Error al parsear JSON:\n{ex.Message}\n\nContenido crudo:\n{json}";
                        }

                    const int maxDisplayLength = 10000;
                    if (prettyJson.Length > maxDisplayLength)
                        {
                        string parcial = prettyJson.Substring(0, maxDisplayLength) + "\n\n... (truncado)";
                        MessageBox.Show(parcial, "Datos obtenidos (parcial)", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    else
                        {
                        MessageBox.Show(prettyJson, "Datos obtenidos", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                else
                    {
                    MessageBox.Show($"Error al consultar API:\n{response.StatusCode}\n\n{json}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Excepción inesperada:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
