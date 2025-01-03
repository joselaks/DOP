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
using System.Windows.Shapes;

namespace DataObra.Interfaz.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiInicio.xaml
    /// </summary>
    public partial class WiInicio : Window
    {
        public WiInicio()
        {
            InitializeComponent();
            this.Loaded += WiInicio_Loaded;
        }

        private void WiInicio_Loaded(object sender, RoutedEventArgs e)
        {
            // Crear y mostrar la ventana de login
            WiLogin login = new WiLogin
            {
                Owner = this, // Establecer la ventana WiInicio como la propietaria de la ventana de login
                WindowStartupLocation = WindowStartupLocation.CenterOwner // Centrar la ventana de login en WiInicio
            };
            login.ShowDialog();
        }
    }
}


