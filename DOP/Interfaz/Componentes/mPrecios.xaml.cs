using DataObra.Interfaz.Ventanas;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Componentes
{
    public partial class mPrecios : UserControl
    {
        private WiEscritorio escritorio;

        public mPrecios(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
        }

        private void Maximizar(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nPrecios", "Maximizado", "M");
        }
    }
}
