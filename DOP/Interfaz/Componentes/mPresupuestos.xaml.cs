using DataObra.Interfaz.Ventanas;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Componentes
{
    public partial class mPresupuestos : UserControl
    {
        private WiEscritorio escritorio;

        public mPresupuestos(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
        }

        private void Maximizar(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nPresupuestos", "Maximizado", "O");
        }
    }
}
