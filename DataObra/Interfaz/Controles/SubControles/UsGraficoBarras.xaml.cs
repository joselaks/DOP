using DataObra.Presupuestos;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataObra.Interfaz.Controles.SubControles
{
    /// <summary>
    /// Lógica de interacción para UsGraficoBarras.xaml
    /// </summary>
    public partial class UsGraficoBarras : UserControl
    {
        public UsGraficoBarras()
        {
            SfSkinManager.SetTheme(this, new Theme("MaterialLight", new string[] { "SfChart" }));
            InitializeComponent();
            Loaded += GraficoTorta_Loaded;

            // Definir los datos de ejemplo
            var datos = new ObservableCollection<DatoGrafico>
            {
                new DatoGrafico { Tipología = "Vivienda", Importe = 1200 },
                new DatoGrafico { Tipología = "Edificio", Importe = 1500 },
                new DatoGrafico { Tipología = "Galpón", Importe = 1100 },
                new DatoGrafico { Tipología = "Reformas", Importe = 890 },
                new DatoGrafico { Tipología = "Oficinas", Importe = 1340 }
            };

            Datos = datos;

        }

        private void GraficoTorta_Loaded(object sender, RoutedEventArgs e)
        {
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
            series.ItemsSource = Datos;
            series.XBindingPath = "Tipología";
            series.YBindingPath = "Importe";
            series.ShowTooltip = true;
            series.Label = "Valor del m2";

            ////Setting adornment to the chart series
            //series.AdornmentsInfo = new ChartAdornmentInfo() { ShowLabel = true };

            //Adding Series to the Chart Series Collection
            graficoBarras.Series.Add(series);
        }

        // Propiedad para enlazar al gráfico
        public ObservableCollection<DatoGrafico> Datos { get; set; }


        public class DatoGrafico
        {
            public string Tipología { get; set; }
            public double Importe
            {
                get; set;
            }
        }
    }
}
