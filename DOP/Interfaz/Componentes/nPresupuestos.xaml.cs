using DataObra.Interfaz.Ventanas;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Componentes
{
    public partial class nPresupuestos : UserControl
    {
        private WiEscritorio escritorio;

        public nPresupuestos(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nPresupuestos", "Maximizado", "O");
        }
    }
}
