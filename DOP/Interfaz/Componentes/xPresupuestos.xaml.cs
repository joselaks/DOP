using DataObra.Interfaz.Ventanas;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Componentes
{
    /// <summary>
    /// Lógica de interacción para xPresupuestos.xaml
    /// </summary>
    public partial class xPresupuestos : UserControl
    {
        private WiEscritorio escritorio;

        public xPresupuestos(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nPresupuestos", "Normal");
        }
    }
}
