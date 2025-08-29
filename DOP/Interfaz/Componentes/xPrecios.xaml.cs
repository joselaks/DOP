using DataObra.Interfaz.Ventanas;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Componentes
{
    /// <summary>
    /// Lógica de interacción para xPrecios.xaml
    /// </summary>
    public partial class xPrecios : UserControl
    {
        private WiEscritorio escritorio;

        public xPrecios(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nPrecios", "Normal");
        }
    }
}
