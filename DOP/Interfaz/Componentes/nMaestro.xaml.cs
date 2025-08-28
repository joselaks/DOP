using DataObra.Interfaz.Ventanas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Lógica de interacción para nMaestro.xaml
    /// </summary>
    public partial class nMaestro : UserControl
    {
        private WiEscritorio escritorio;

        public nMaestro(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;   
        }

        private void Maximizar(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nMaestro", "Maximizado");
        }
    }
}
