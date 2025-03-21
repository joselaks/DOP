using Biblioteca.DTO;
using DataObra.Agrupadores;
using DataObra.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace DataObra.Interfaz.Controles
{
    public partial class UcAgrupadores : UserControl
    {
        #region Campos y Constructor

        private string Rol;
        private List<AgrupadorDTO> listaAgrupadores = new List<AgrupadorDTO>();

        public UcAgrupadores(string rol)
        {
            InitializeComponent();
            Rol = rol;
            this.Loaded += (s, e) => CrearBotones();
            CargarGrilla();
        }

        #endregion

        #region Diccionarios de Tipos y Usuarios

        private readonly Dictionary<char, string> tiposAgrupadores = new()
        {
            { 'C', "Clientes" },
            { 'P', "Proveedores" },
            { 'E', "Empleados" },
            { 'S', "SubContratistas" },
            { 'O', "Obras" },
            { 'A', "Administración" }
        };

        private readonly Dictionary<int, string> usuarios = new()
        {
            { 0, "El Usuario" },
            { 1, "José" },
            { 2, "Sebastián" }
        };

        #endregion

        #region Métodos de Carga de Datos

        public async Task CargarGrilla()
        {
            try
            {
                var AgrupadoresUsuario = await DatosWeb.ObtenerAgrupadoresPorCuentaIDAsync(App.IdCuenta);

                foreach (var item in AgrupadoresUsuario.Agrupadores)
                {
                    item.Tipo = tiposAgrupadores.GetValueOrDefault(item.TipoID, "Otros");
                    item.Usuario = usuarios.GetValueOrDefault(item.UsuarioID, "Otro");
                }

                listaAgrupadores = AgrupadoresUsuario.Agrupadores;
                Application.Current.Dispatcher.Invoke(() => GrillaAgrupadores.ItemsSource = listaAgrupadores);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los agrupadores: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Métodos de UI

        private void CrearBotones()
        {
            items.Children.Clear();
            foreach (var tipo in tiposAgrupadores)
            {
                ToggleButton toggleButton = new()
                {
                    Width = 100,
                    Height = 35,
                    Margin = new Thickness(5),
                    Content = tipo.Value,
                    Tag = tipo.Key
                };

                toggleButton.Checked += ToggleButton_Checked;
                toggleButton.Unchecked += ToggleButton_Unchecked;
                items.Children.Add(toggleButton);
            }
        }

        private void ActualizarGrilla()
        {
            var tiposSeleccionados = items.Children
                .OfType<ToggleButton>()
                .Where(b => b.IsChecked == true)
                .Select(b => (char)b.Tag)
                .ToList();

            GrillaAgrupadores.ItemsSource = tiposSeleccionados.Count == 0
                ? listaAgrupadores
                : listaAgrupadores.Where(a => tiposSeleccionados.Contains(a.TipoID)).ToList();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) => ActualizarGrilla();
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e) => ActualizarGrilla();
        private void actualizaGrilla_Click(object sender, RoutedEventArgs e) => CargarGrilla();
        private void GrillaAgrupadores_MouseDoubleClick(object sender, MouseButtonEventArgs e) => EditarAgrupador(sender, e);

        #endregion

        #region Métodos de Gestión de Agrupadores

        private void NuevoAgrupador(object sender, RoutedEventArgs e)
        {
            var Docu = new SubControles.UcAgrupador(null, this);
            new DataObra.Interfaz.Ventanas.WiDialogo("Agrupador", Docu).ShowDialog();
        }

        private void EditarAgrupador(object sender, RoutedEventArgs e)
        {
            if (GrillaAgrupadores.SelectedItem is not AgrupadorDTO agrupadorSeleccionado)
            {
                MessageBox.Show("Por favor, seleccione un Agrupador para editar.");
                return;
            }

            var Docu = new SubControles.UcAgrupador(agrupadorSeleccionado, this);
            var ventanaAgru = new DataObra.Interfaz.Ventanas.WiDialogo("Agrupador", Docu);
            var mainWindow = Window.GetWindow(this);

            mainWindow.Effect = new BlurEffect { Radius = 3 };
            ventanaAgru.ShowDialog();
            mainWindow.Effect = null;
        }

        private async void BorrarAgrupador(object sender, RoutedEventArgs e)
        {
            if (GrillaAgrupadores.SelectedItem is not AgrupadorDTO agrupadorSeleccionado)
            {
                MessageBox.Show("Por favor, seleccione un agrupador para eliminar.");
                return;
            }

            var (success, message) = await DatosWeb.EliminarAgrupadorAsync(agrupadorSeleccionado.ID);
            if (success)
            {
                MessageBox.Show($"Agrupador eliminado con éxito. ID: {agrupadorSeleccionado.ID}  Descripción: {agrupadorSeleccionado.Descrip}",
                    "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarGrilla();
            }
            else
            {
                MessageBox.Show($"Error al eliminar el agrupador: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
