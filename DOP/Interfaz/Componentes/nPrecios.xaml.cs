using DataObra.Interfaz.Ventanas;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Lógica de interacción para nPrecios.xaml
    /// </summary>
    public partial class nPrecios : UserControl
    {
        private WiEscritorio escritorio;

        public nPrecios(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
        }
    }
}
