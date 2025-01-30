using DataObra.Interfaz.Controles.SubControles;
using DataObra.Sistema;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows.Tools.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace DataObra.Interfaz.Controles
{
    public partial class UcNavegador : UserControl
    {
        string Rol;
       

        public UcNavegador(string rol)
        {
            InitializeComponent();
            Rol = rol;
            configuraRol(Rol);
           
        }

        private void configuraRol(string rol)
        {
            switch (rol)
            {
                case "Presupuestos":
                    break;
                case "Compras":
                    break;
                default:
                    break;

            }
           
        }



        private void GrillaDocumentos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void NuevoPresupuesto_Click(object sender, RoutedEventArgs e)
        {
            UserControl presup = new DataObra.Presupuestos.UcPresupuesto(null);
            DataObra.Interfaz.Ventanas.WiDocumento ventanaPres = new DataObra.Interfaz.Ventanas.WiDocumento("Presupuesto", presup);
            var mainWindow = Window.GetWindow(this);

            // Aplicar efecto de desenfoque a la ventana principal
            mainWindow.Effect = new BlurEffect { Radius = 3 };

            //// Calcular el nuevo tamaño y posición de la ventana modal
            //ventana.Width = mainWindow.ActualWidth - 20;
            //ventana.Height = mainWindow.ActualHeight - 20;
            //ventana.Left = mainWindow.Left + 10;
            //ventana.Top = mainWindow.Top + 10;

            // Mostrar la ventana de manera modal
            ventanaPres.ShowDialog();

            // Quitar el efecto de desenfoque después de cerrar la ventana modal
            mainWindow.Effect = null;

        }

        private void NuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            UserControl docu = new DataObra.Documentos.UcDocumento();
            DataObra.Interfaz.Ventanas.WiDocumento ventanaDocu = new DataObra.Interfaz.Ventanas.WiDocumento("Factura", docu);

            ventanaDocu.ShowDialog();

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Checked");
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}








