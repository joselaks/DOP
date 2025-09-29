using DataObra.Interfaz.Ventanas;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Componentes
{
    public partial class nPrecios : UserControl
    {
        private WiEscritorio escritorio;

        public nPrecios(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nPrecios", "Maximizado", "M");
        }
    }
}
