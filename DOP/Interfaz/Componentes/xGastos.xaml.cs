using DataObra.Documentos.Ventanas;
using DataObra.Interfaz.Ventanas;
using DataObra.Presupuestos.Ventanas;
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
    /// Lógica de interacción para xGastos.xaml
    /// </summary>
    public partial class xGastos : UserControl
        {
        private WiEscritorio escritorio;
        public xGastos(WiEscritorio escritorio)
            {
            InitializeComponent();
            this.escritorio = escritorio;
            }

        private void Button_Click(object sender, RoutedEventArgs e)
            {
            escritorio.CambioEstado("nGastos", "Normal", "O");
            }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
            {
            var win = new WiGasto();
            win.ShowDialog();

            }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
            {

            }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
            {

            }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
            {

            }

        private void GrillaPresupuestos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {

            }
        }
    }
