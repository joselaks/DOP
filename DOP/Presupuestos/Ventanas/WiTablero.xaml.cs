using Biblioteca;
using Biblioteca.DTO;
using DOP.Datos;
using DOP.Interfaz.Ventanas;
using Syncfusion.SfSkinManager;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DOP.Presupuestos.Ventanas
    {
    /// <summary>
    /// Lógica de interacción para WiTablero.xaml
    /// </summary>
    public partial class WiTablero : Window
        {
        private double _previousLeft;
        private double _previousTop;
        private double _previousWidth;
        private double _previousHeight;
        private bool _isCustomMaximized = false;
        private ObservableCollection<PresupuestoDTO> _presupuestos = new();
        private ObservableCollection<PresupuestoDTO> _modelos = new();

        public WiTablero()
            {
            SfSkinManager.SetTheme(this, new Theme("MaterialLight", new string[] { "TabNavigationControl", "TabControlExt" }));
            InitializeComponent();

            //// Detectar la resolución de pantalla principal
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            if (screenWidth <= 1920 && screenHeight <= 1080)
                {
                // Maximizar si la resolución es igual o menor a 1920x1080
                Maximize_Click(null, null);
                }
            else
                {
                // Centrar y establecer tamaño fijo si la resolución es mayor

                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                WindowState = WindowState.Normal;
                Width = 1900;
                Height = 1030;
                }
            Loaded += WiTablero_Loaded;



            }

        private async void WiTablero_Loaded(object sender, RoutedEventArgs e)
            {
            var (success, message, lista) = await DatosWeb.ObtenerPresupuestosUsuarioAsync();
            if (success)
                {
                // Calcular ValorM2 para cada presupuesto
                foreach (var p in lista)
                    {
                    if (p.Superficie.HasValue && p.Superficie.Value > 0)
                        p.ValorM2 = p.PrEjecTotal / p.Superficie.Value;
                    else
                        p.ValorM2 = 0;
                    }

                if (App.tipoUsuario == 2)
                    {
                    _presupuestos = new ObservableCollection<PresupuestoDTO>(lista);
                    }
                else
                    {
                    _presupuestos = new ObservableCollection<PresupuestoDTO>(lista.Where(p => !p.EsModelo));
                    }
                _modelos = new ObservableCollection<PresupuestoDTO>(lista.Where(p => p.EsModelo));

                GrillaPresupuestos.ItemsSource = _presupuestos;
                }
            else
                {
                MessageBox.Show($"No se pudieron cargar los presupuestos.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                GrillaPresupuestos.ItemsSource = null;
                }
            txtUsuario.Text = "Usuario: " + App.NombreUsuario;
            btnBackstage.Visibility = (App.tipoUsuario == 2)
    ? Visibility.Visible
    : Visibility.Collapsed;

            GraficoGraficoBarras();


            }


        #region Comportamiento ventana

        private void Maximize_Click(object sender, RoutedEventArgs e)
            {
            if (_isCustomMaximized)
                {
                // Restaurar el tamaño, la posición y la sombra anteriores
                WindowState = WindowState.Normal;
                Left = _previousLeft;
                Top = _previousTop;
                Width = _previousWidth;
                Height = _previousHeight;
                MainBorder.Margin = new Thickness(10);
                WindowShadow.Opacity = 0.5;
                _isCustomMaximized = false;
                }
            else
                {
                // Almacenar el tamaño y la posición actuales
                _previousLeft = Left;
                _previousTop = Top;
                _previousWidth = Width;
                _previousHeight = Height;

                // Maximizar la ventana y eliminar la sombra
                var screen = System.Windows.SystemParameters.WorkArea;
                Left = screen.Left;
                Top = screen.Top;
                Width = screen.Width;
                Height = screen.Height;
                MainBorder.Margin = new Thickness(0);
                WindowShadow.Opacity = 0;
                _isCustomMaximized = true;
                }
            }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            {
            WindowState = WindowState.Minimized;
            }

        private async void Close_Click(object sender, RoutedEventArgs e)
            {

            Close();
            // Obtener la MAC Address de la primera interfaz activa
            string macaddress = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up &&
                              nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault() ?? "UNKNOWN";

            // Registrar la salida en el log (si el usuario está logueado)
            if (App.IdUsuario > 0)
                {
                await DOP.Datos.DatosWeb.RegistrarSalidaUsuarioAsync(App.IdUsuario, macaddress);
                }
            Application.Current.Shutdown();
            }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
            if (e.ClickCount == 2)
                {
                Maximize_Click(sender, e);
                }
            else
                {
                DragMove();
                }
            }

        #endregion

        #region Grafico

        private void GraficoGraficoBarras()
            {
            // Borra series previas para evitar duplicados al recargar
            graficoBarras.Series.Clear();

            // Construye los datos a partir de la colección _modelos
            var datos = new ObservableCollection<DatoGrafico>(
                _modelos
                    .Where(m => !string.IsNullOrWhiteSpace(m.Descrip))
                    .Select(m => new DatoGrafico
                    {
                        Tipología = m.Descrip,
                        Importe = (double)m.ValorM2
                    })
            );

            // Ejes
            CategoryAxis primaryAxis = new CategoryAxis
            {
                Header = "Tipología",
                FontSize = 14
            };
            graficoBarras.PrimaryAxis = primaryAxis;

            NumericalAxis secondaryAxis = new NumericalAxis
            {
                Header = "Valor del m2 (u$s)",
                FontSize = 14
            };
            graficoBarras.SecondaryAxis = secondaryAxis;

            // Leyenda
            ChartLegend legend = new ChartLegend();
            graficoBarras.Legend = legend;

            // Serie de columnas
            ColumnSeries series = new ColumnSeries
            {
                ItemsSource = datos,
                XBindingPath = "Tipología",
                YBindingPath = "Importe",
                ShowTooltip = true,
                Label = "Valor del m2"
            };

            graficoBarras.Series.Add(series);
            }

        #endregion

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
            {
            if (sender is MenuItem menuItem)
                {
                // Puedes usar Name o Header según cómo esté definido tu menú
                switch (menuItem.Header?.ToString())
                    {
                    case "Nuevo":
                        var ventana = new WiNuevoPres(_presupuestos)
                            {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                            };
                        ventana.ShowDialog();
                        break;

                    case "Editar":
                        if (GrillaPresupuestos.SelectedItem is PresupuestoDTO seleccionado && seleccionado.ID.HasValue)
                            {
                            var (ok, msg, conceptos, relaciones) = await DatosWeb.ObtenerConceptosYRelacionesAsync(seleccionado.ID.Value);
                            if (ok)
                                {
                                var copia = PresupuestoDTO.CopiarPresupuestoDTO(seleccionado); // <-- aquí el cambio
                                var wiPresupuesto = new WiPresupuesto(copia, conceptos, relaciones, _presupuestos);
                                wiPresupuesto.Owner = this;
                                wiPresupuesto.ShowDialog();
                                }
                            else
                                {
                                MessageBox.Show($"No se pudieron obtener los datos del presupuesto.\n{msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        else
                            {
                            MessageBox.Show("Seleccione un presupuesto para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        break;

                    case "Borrar":
                        if (GrillaPresupuestos.SelectedItem is PresupuestoDTO eliminar && eliminar.ID.HasValue)
                            {
                            var result = MessageBox.Show("¿Está seguro que desea eliminar el presupuesto seleccionado?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                                {
                                var (success, message) = await DatosWeb.BorrarPresupuestoAsync(eliminar.ID.Value);
                                if (success)
                                    {
                                    // Quitar de la colección y refrescar la grilla
                                    _presupuestos.Remove(eliminar);
                                    GrillaPresupuestos.ItemsSource = null;
                                    GrillaPresupuestos.ItemsSource = _presupuestos;
                                    MessageBox.Show(message, "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                else
                                    {
                                    MessageBox.Show($"No se pudo eliminar el presupuesto.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                        else
                            {
                            MessageBox.Show("Seleccione un presupuesto para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        break;

                    default:
                        MessageBox.Show("Opción de menú no implementada.", "Menú", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                }
            }



        private void GrillaPresupuestos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {

            }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
            {
            var ventana = new WiNuevoPres(_presupuestos)
                {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
            ventana.ShowDialog();
            }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
            {
            if (GrillaPresupuestos.SelectedItem is PresupuestoDTO seleccionado && seleccionado.ID.HasValue)
                {
                // Obtener conceptos y relaciones antes de abrir la ventana
                var (ok, msg, conceptos, relaciones) = await DatosWeb.ObtenerConceptosYRelacionesAsync(seleccionado.ID.Value);
                if (ok)
                    {
                    // Aquí puedes pasar conceptos y relaciones a la ventana WiPresupuesto si lo necesitas
                    var copia = PresupuestoDTO.CopiarPresupuestoDTO(seleccionado);
                    var wiPresupuesto = new WiPresupuesto(copia, conceptos, relaciones, _presupuestos);
                    wiPresupuesto.Owner = this;
                    wiPresupuesto.ShowDialog();
                    // Si necesitas usar conceptos y relaciones después, puedes hacerlo aquí
                    }
                else
                    {
                    MessageBox.Show($"No se pudieron obtener los datos del presupuesto.\n{msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            else
                {
                MessageBox.Show("Seleccione un presupuesto para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        private async void btnBorrar_Click(object sender, RoutedEventArgs e)
            {
            if (GrillaPresupuestos.SelectedItem is PresupuestoDTO eliminar && eliminar.ID.HasValue)
                {
                var result = MessageBox.Show("¿Está seguro que desea eliminar el presupuesto seleccionado?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    {
                    var (success, message) = await DatosWeb.BorrarPresupuestoAsync(eliminar.ID.Value);
                    if (success)
                        {
                        // Quitar de la colección y refrescar la grilla
                        _presupuestos.Remove(eliminar);
                        GrillaPresupuestos.ItemsSource = null;
                        GrillaPresupuestos.ItemsSource = _presupuestos;
                        MessageBox.Show(message, "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    else
                        {
                        MessageBox.Show($"No se pudo eliminar el presupuesto.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            else
                {
                MessageBox.Show("Seleccione un presupuesto para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }

        private void btnBackstage_Click(object sender, RoutedEventArgs e)
            {

            }
        }


    public class DatoGrafico
        {
        public string Tipología { get; set; }
        public double Importe
            {
            get; set;
            }
        }

    }

