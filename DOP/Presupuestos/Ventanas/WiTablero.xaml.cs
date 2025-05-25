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

        public WiTablero()
            {
            SfSkinManager.SetTheme(this, new Theme("MaterialLight", new string[] { "TabNavigationControl", "TabControlExt" }));
            InitializeComponent();
            GraficoGraficoBarras();
            Maximize_Click(null, null);


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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
            {

            }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
            {

            }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
            {

            }

        private void GrillaDocumentos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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

