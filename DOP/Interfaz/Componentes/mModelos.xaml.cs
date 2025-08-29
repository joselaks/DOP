using DataObra.Interfaz.Ventanas;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Componentes
{
    public partial class mModelos : UserControl
    {
        private WiEscritorio escritorio;

        public mModelos(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
        }

        private void Maximizar(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nModelos", "Maximizado");
        }
    }
}
