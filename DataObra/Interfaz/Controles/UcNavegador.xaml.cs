using Biblioteca;
using DataObra.Datos;
using DataObra.Interfaz.Controles.SubControles;
using DataObra.Sistema;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows.Tools.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
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
            CargarGrilla();
        }

        private async void CargarGrilla()
        {
            var DocumentosUsuario = await DatosWeb.ObtenerDocumentosPorCuentaIDAsync(App.IdCuenta);
            // Lo que se obtiene es una lista de DocumentoDTO. Habria que convertirlos en Documentos.


            this.GrillaDocumentos.ItemsSource = DocumentosUsuario.Documentos;
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

        private async void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (GrillaDocumentos.SelectedItem is Documento documentoSeleccionado)
            {
                if (documentoSeleccionado.TipoID == 10) // es un presupuesto
                {
                    
                    var encabezado = documentoSeleccionado;
                    UserControl presup = new DataObra.Presupuestos.UcPresupuesto(encabezado);
                    DataObra.Interfaz.Ventanas.WiDocumento ventanaPres = new DataObra.Interfaz.Ventanas.WiDocumento("Presupuesto", presup);
                    var mainWindow = Window.GetWindow(this);
                    // Aplicar efecto de desenfoque a la ventana principal
                    mainWindow.Effect = new BlurEffect { Radius = 3 };
                    // Mostrar la ventana de manera modal
                    ventanaPres.ShowDialog();
                    // Quitar el efecto de desenfoque después de cerrar la ventana modal
                    mainWindow.Effect = null;
                }
                else
                {
                    
                    Documentos.MaxDocumento Docu = new Documentos.MaxDocumento(documentoSeleccionado);
                    DataObra.Interfaz.Ventanas.WiDocumento ventanaDocu = new DataObra.Interfaz.Ventanas.WiDocumento(documentoSeleccionado.TipoID.ToString(), Docu);
                    ventanaDocu.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un documento para editar.");
            }
        }

        private async void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (GrillaDocumentos.SelectedItem is Documento documentoSeleccionado)
            {
                //var respuesta = await ConsultasAPI.DeleteDocumentoAsync((int)documentoSeleccionado.ID);
                //if (respuesta.Success)
                //{
                //    MessageBox.Show("Documento eliminado exitosamente.");
                //    CargarGrilla(); // Actualizar la grilla después de eliminar el documento
                //}
                //else
                //{
                //    MessageBox.Show($"Error al eliminar el documento: {respuesta.Message}");
                //}
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un documento para eliminar.");
            }
        }

        private void NuevoPresupuesto_Click(object sender, RoutedEventArgs e)
        {
            UserControl presup = new DataObra.Presupuestos.UcPresupuesto(null);
            DataObra.Interfaz.Ventanas.WiDocumento ventanaPres = new DataObra.Interfaz.Ventanas.WiDocumento("Presupuesto", presup);
            var mainWindow = Window.GetWindow(this);

            // Aplicar efecto de desenfoque a la ventana principal
            mainWindow.Effect = new BlurEffect { Radius = 3 };

            // Mostrar la ventana de manera modal
            ventanaPres.ShowDialog();

            // Quitar el efecto de desenfoque después de cerrar la ventana modal
            mainWindow.Effect = null;
        }

        private void NuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            Biblioteca.Documento objetoFactura = new Biblioteca.Documento();
            Documentos.MaxDocumento Docu = new Documentos.MaxDocumento(objetoFactura);
            DataObra.Interfaz.Ventanas.WiDocumento ventanaDocu = new DataObra.Interfaz.Ventanas.WiDocumento("Factura", Docu);

            ventanaDocu.ShowDialog();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;

            if (toggleButton != null)
            {
                toggleButton.BorderBrush = new SolidColorBrush(Colors.Red);
                toggleButton.BorderThickness = new Thickness(2);
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;

            if (toggleButton != null)
            {
                toggleButton.BorderBrush = new SolidColorBrush(Colors.Transparent);
                toggleButton.BorderThickness = new Thickness(1);
            }
        }

        private void actualizaGrilla_Click(object sender, RoutedEventArgs e)
        {
            CargarGrilla();
        }

        private void GrillaDocumentos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (GrillaDocumentos.SelectedItem is Documento documentoSeleccionado)
            {
                var mainWindow = Window.GetWindow(this);
                // Aplicar efecto de desenfoque a la ventana principal
                mainWindow.Effect = new BlurEffect { Radius = 3 };

                DataObra.Interfaz.Ventanas.WiDocumento ventanaDoc;

                if (documentoSeleccionado.TipoID == 10) // es un presupuesto
                {
                    UserControl presup = new DataObra.Presupuestos.UcPresupuesto(null);
                    ventanaDoc = new DataObra.Interfaz.Ventanas.WiDocumento("Presupuesto", presup);

                }
                else
                {
                    // Agregar manejo para cada tipo de documento
                    Biblioteca.Documento objetoFactura = new Biblioteca.Documento();
                    Documentos.MaxDocumento Docu = new Documentos.MaxDocumento(objetoFactura);
                    ventanaDoc = new DataObra.Interfaz.Ventanas.WiDocumento("Factura", Docu);

                }
                // Mostrar la ventana de manera modal
                ventanaDoc.ShowDialog();

                // Quitar el efecto de desenfoque después de cerrar la ventana modal
                mainWindow.Effect = null;

            }
            else
            {
                MessageBox.Show("Por favor, seleccione un documento para editar.");
            }
        }
    }
}











