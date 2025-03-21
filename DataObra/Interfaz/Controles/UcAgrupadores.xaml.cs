using Biblioteca.DTO;
using DataObra.Agrupadores;
using DataObra.Datos;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataObra.Interfaz.Controles
{
    public partial class UcAgrupadores : UserControl
    {
        string Rol;

        public UcAgrupadores(string rol)
        {
            InitializeComponent();
            Rol = rol;
            CargarGrilla();
        }

        public async void CargarGrilla()
        {
            var AgrupadoresUsuario = await DatosWeb.ObtenerAgrupadoresPorCuentaIDAsync(App.IdCuenta);

            foreach (var item in AgrupadoresUsuario.Agrupadores)
            {
                switch (item.TipoID)
                {
                    case 'C':
                        item.Tipo = "Cliente";
                        break;
                    case 'P':
                        item.Tipo = "Proveedor";
                        break;
                    case 'E':
                        item.Tipo = "Empleado";
                        break;
                    case 'S':
                        item.Tipo = "SubContratista";
                        break;
                    case 'O':
                        item.Tipo = "Obra";
                        break;
                    case 'A':
                        item.Tipo = "Administración";
                        break;
                    default:
                        item.Tipo = "Otros";
                        break;
                }

                switch (item.UsuarioID)
                {
                    case 0:
                        item.Usuario = "El Usuario";
                        break;
                    case 1:
                        item.Usuario = "José";
                        break;
                    case 2:
                        item.Usuario = "Sebastián";
                        break;
                    default:
                        item.Usuario = "Otro";
                        break;
                }
            }

            // Posiblemente haya que convertir AgrupadorDTO en Agrupador
            this.GrillaAgrupadores.ItemsSource = AgrupadoresUsuario.Agrupadores;

            // this.GrillaAgrupadores.ItemsSource = App.ListaAgrupadores;
            // App.ListaAgrupadores = AgrupadoresUsuario.agrupadores;
            // Los agrupadores se utilizan todo el tiempo y por lo tanto tienen que estar accesibles de todos lados.
        }

        private void NuevoAgrupador(object sender, RoutedEventArgs e)
        {
            {
                SubControles.UcAgrupador Docu = new SubControles.UcAgrupador(null, this);
                DataObra.Interfaz.Ventanas.WiDialogo ventanaDocu = new DataObra.Interfaz.Ventanas.WiDialogo("Agrupador", Docu);

                ventanaDocu.ShowDialog();
            }
        }

        private void EditarAgrupador(object sender, RoutedEventArgs e)
        {
            if (GrillaAgrupadores.SelectedItem is AgrupadorDTO agrupadorSeleccionado)
            {
                AgrupadorDTO agrupador = agrupadorSeleccionado;

                SubControles.UcAgrupador Docu = new SubControles.UcAgrupador(agrupador, this);
                DataObra.Interfaz.Ventanas.WiDialogo ventanaAgru = new DataObra.Interfaz.Ventanas.WiDialogo("Agrupador", Docu);

                var mainWindow = Window.GetWindow(this);
                // Aplicar efecto de desenfoque a la ventana principal
                mainWindow.Effect = new BlurEffect { Radius = 3 };
                // Mostrar la ventana de manera modal
                ventanaAgru.ShowDialog();
                // Quitar el efecto de desenfoque después de cerrar la ventana modal
                mainWindow.Effect = null;
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un Agrupador para editar.");
            }
        }

        private async void BorrarAgrupador(object sender, RoutedEventArgs e)
        {
            if (GrillaAgrupadores.SelectedItem is AgrupadorDTO agrupadorSeleccionado)
            {
                // Llamar al método EliminarAgrupadorAsync
                var (success, message) = await DatosWeb.EliminarAgrupadorAsync(agrupadorSeleccionado.ID);

                // Manejar la respuesta
                if (success)
                {
                    MessageBox.Show($"Agrupador eliminado con éxito. ID: {agrupadorSeleccionado.ID}  Descripción: {agrupadorSeleccionado.Descrip}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarGrilla();
                }
                else
                {
                    MessageBox.Show($"Error al eliminar el agrupador: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un agrupador para eliminar.");
            }
        }

        private void GrillaAgrupadores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void actualizaGrilla_Click(object sender, RoutedEventArgs e)
        {
            CargarGrilla();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        //private void Obra_Click(object sender, RoutedEventArgs e)
        //{
        //    SubControles.UcAgrupador Docu = new SubControles.UcAgrupador(null, this);
        //    DataObra.Interfaz.Ventanas.WiDialogo ventanaDocu = new DataObra.Interfaz.Ventanas.WiDialogo("Agrupador", Docu);

        //    ventanaDocu.ShowDialog();
        //}

        //private void NuevoProveedor_Click(object sender, RoutedEventArgs e)
        //{
        //    {
        //        SubControles.UcAgrupador Docu = new SubControles.UcAgrupador(null, this);
        //        DataObra.Interfaz.Ventanas.WiDialogo ventanaDocu = new DataObra.Interfaz.Ventanas.WiDialogo("Agrupador", Docu);

        //        ventanaDocu.ShowDialog();
        //    }
        //}
    }
}
