using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /// Lógica de interacción para WiDialogo.xaml
    /// </summary>
    public partial class WiDialogo : Window
    {
        public WiDialogo(string TipoDoc, UserControl userControl)
        {
            InitializeComponent();
            TituloVentana.Text = TipoDoc;
            espacioPrincipal.Children.Add(userControl);

            // Ajustar el tamaño de espacioPrincipal al tamaño del userControl
            espacioPrincipal.Width = userControl.Width;
            espacioPrincipal.Height = userControl.Height;

            // Ajustar el tamaño de la ventana al tamaño del espacioPrincipal
            this.Width = userControl.Width + 40; // Añadir un margen para el borde de la ventana
            this.Height = userControl.Height + 60; // Añadir un margen para el borde de la ventana y el título
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
