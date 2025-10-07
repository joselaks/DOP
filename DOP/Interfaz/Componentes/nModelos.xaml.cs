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

        }


     

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nModelos", "Maximizado", "M");
        }
    }
}
