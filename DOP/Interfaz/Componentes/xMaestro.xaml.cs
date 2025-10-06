using DataObra.Interfaz.Ventanas;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Componentes
{
    /// <summary>
    /// Lógica de interacción para xMaestro.xaml
    /// </summary>
    public partial class xMaestro : UserControl
    {
        private WiEscritorio escritorio;

        public xMaestro(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nMaestro", "Normal", "M");
        }
    }
}
