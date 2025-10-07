using DOP.Presupuestos.Controles; // Importa el namespace del control
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

            // Inicializa el control UcMaestro y lo agrega al contenedor
            var ucMaestro = new UcMaestro();
            ContenedorUcMaestro.Children.Add(ucMaestro);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nMaestro", "Normal", "M");
        }
    }
}
