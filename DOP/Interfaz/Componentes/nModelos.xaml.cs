using DataObra.Interfaz.Ventanas;
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
using static DOP.Presupuestos.Ventanas.WiTablero;

namespace DataObra.Interfaz.Componentes
{
    /// <summary>
    /// Lógica de interacción para nModelos.xaml
    /// </summary>
    public partial class nModelos : UserControl
    {
        private WiEscritorio escritorio;

        public nModelos(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
            this.Loaded += NModelos_Loaded; 

        }

        private void NModelos_Loaded(object sender, RoutedEventArgs e)
        {
            GraficoGraficoBarras();
        }

        private void GraficoGraficoBarras()
        {
            // Borra series previas para evitar duplicados al recargar
            graficoBarras.Series.Clear();

            // Construye los datos a partir de la colección _modelos
            var datos = new ObservableCollection<DatoGrafico>(
                escritorio._modelos
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nModelos", "Maximizado");
        }
    }
}
