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
    /// <summary>
    /// Lógica de interacción para UcAgrupador.xaml
    /// </summary>
    public partial class UcAgrupadores : UserControl
    {
        string Rol;

        public UcAgrupadores(string rol)
        {
            InitializeComponent();
            Rol = rol;
            //configuraRol(Rol);
            CargarGrilla();

        }

        private async void CargarGrilla()
        {
            var AgrupadoresUsuario = await ConsultasAPI.ObtenerAgrupadoresPorCuentaID(App.IdCuenta);
            this.GrillaAgrupadores.ItemsSource = AgrupadoresUsuario.agrupadores;
        }

        private void configuraRol(string rol)
        {
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            {
                SubControles.UcAgrupador Docu = new SubControles.UcAgrupador(null);
                DataObra.Interfaz.Ventanas.WiDialogo ventanaDocu = new DataObra.Interfaz.Ventanas.WiDialogo("Agrupador", Docu);

                ventanaDocu.ShowDialog();

            }
        }

        private void EditarAgrupador(object sender, RoutedEventArgs e)
        {
            if (GrillaAgrupadores.SelectedItem is Agrupador agrupadorSeleccionado)
            {
                Agrupador agrupador = agrupadorSeleccionado;

                  SubControles.UcAgrupador Docu = new SubControles.UcAgrupador(agrupador);
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


        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void GrillaAgrupadores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void actualizaGrilla_Click(object sender, RoutedEventArgs e)
        {
            CargarGrilla();
        }

        private void Obra_Click(object sender, RoutedEventArgs e)
        {
            SubControles.UcAgrupador Docu = new SubControles.UcAgrupador(null);
            DataObra.Interfaz.Ventanas.WiDialogo ventanaDocu = new DataObra.Interfaz.Ventanas.WiDialogo("Agrupador", Docu);

            ventanaDocu.ShowDialog();

        }

        private void NuevoProveedor_Click(object sender, RoutedEventArgs e)
        {
            {
                SubControles.UcAgrupador Docu = new SubControles.UcAgrupador(null);
                DataObra.Interfaz.Ventanas.WiDialogo ventanaDocu = new DataObra.Interfaz.Ventanas.WiDialogo("Agrupador", Docu);

                ventanaDocu.ShowDialog();

            }
        }
    }
}
