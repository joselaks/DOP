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

        public WiTablero()
            {
            SfSkinManager.SetTheme(this, new Theme("MaterialLight", new string[] { "TabNavigationControl", "TabControlExt" }));
            InitializeComponent();
            GraficoGraficoBarras();

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
                Width = 1800;
                Height = 1000;
                }
            Loaded += WiTablero_Loaded;



            }

        private async void WiTablero_Loaded(object sender, RoutedEventArgs e)
            {
            var (success, message, lista) = await DatosWeb.ObtenerPresupuestosUsuarioAsync();
            if (success)
                {
                _presupuestos = new ObservableCollection<PresupuestoDTO>(lista);
                GrillaPresupuestos.ItemsSource = _presupuestos;
                }
            else
                {
                MessageBox.Show($"No se pudieron cargar los presupuestos.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                GrillaPresupuestos.ItemsSource = null;
                }
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

        private void Close_Click(object sender, RoutedEventArgs e)
            {

            Close();
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
            // Definir los datos de ejemplo
            var datos = new ObservableCollection<DatoGrafico>
            {
                new DatoGrafico { Tipología = "Vivienda", Importe = 1200 },
                new DatoGrafico { Tipología = "Edificio", Importe = 1500 },
                new DatoGrafico { Tipología = "Galpón", Importe = 1100 },
                new DatoGrafico { Tipología = "Reformas", Importe = 890 },
                new DatoGrafico { Tipología = "Oficinas", Importe = 1340 }
            };

            //Adding horizontal axis to the chart 
            CategoryAxis primaryAxis = new CategoryAxis();
            primaryAxis.Header = "Tipología";
            primaryAxis.FontSize = 14;
            graficoBarras.PrimaryAxis = primaryAxis;

            //Adding vertical axis to the chart 
            NumericalAxis secondaryAxis = new NumericalAxis();
            secondaryAxis.Header = "Valor del m2 (u$s)";
            secondaryAxis.FontSize = 14;
            graficoBarras.SecondaryAxis = secondaryAxis;

            //Adding Legends for the chart
            ChartLegend legend = new ChartLegend();
            graficoBarras.Legend = legend;

            //Initializing column series
            ColumnSeries series = new ColumnSeries();
            series.ItemsSource = datos;
            series.XBindingPath = "Tipología";
            series.YBindingPath = "Importe";
            series.ShowTooltip = true;
            series.Label = "Valor del m2";

            ////Setting adornment to the chart series
            //series.AdornmentsInfo = new ChartAdornmentInfo() { ShowLabel = true };

            //Adding Series to the Chart Series Collection
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
                        var ventana = new WiNuevoPres
                            {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                            };
                        ventana.ShowDialog();
                        break;

                    case "Editar":
                        if (GrillaPresupuestos.SelectedItem is PresupuestoDTO seleccionado && seleccionado.ID.HasValue)
                            {
                            // Obtener conceptos y relaciones antes de abrir la ventana
                            var (ok, msg, conceptos, relaciones) = await DatosWeb.ObtenerConceptosYRelacionesAsync(seleccionado.ID.Value);
                            if (ok)
                                {
                                // Aquí puedes pasar conceptos y relaciones a la ventana WiPresupuesto si lo necesitas
                                var wiPresupuesto = new WiPresupuesto(seleccionado,conceptos,relaciones);
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
                        break;

                    case "Eliminar":
                        if (GrillaPresupuestos.SelectedItem is PresupuestoDTO eliminar && eliminar.ID.HasValue)
                            {
                            // Aquí puedes pedir confirmación y luego llamar a tu método de borrado
                            var result = MessageBox.Show("¿Está seguro que desea eliminar el presupuesto seleccionado?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                                {
                                // Llama a tu método de borrado (puedes hacerlo async si lo deseas)
                                // await DatosWeb.BorrarPresupuestoAsync(eliminar.ID.Value);
                                MessageBox.Show("Presupuesto eliminado (implementa el borrado real aquí).");
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

