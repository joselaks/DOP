using Biblioteca.DTO;
using DataObra.Documentos.Ventanas;
using DataObra.Interfaz.Ventanas;
using DataObra.Presupuestos.Ventanas;
using DOP;
using DOP.Datos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<GastoDTO> _gastos = new();
        public xGastos(WiEscritorio escritorio)
            {
            InitializeComponent();
            this.escritorio = escritorio;
            this.Loaded += XGastos_Loaded;


            }

        private async void XGastos_Loaded(object sender, RoutedEventArgs e)
            {
            try
                {
                // Obtener gastos del usuario (usa App.IdUsuario como en otros helpers)
                var (success, message, gastos) = await DatosWeb.ObtenerGastosPorUsuarioAsync(App.IdUsuario);

                if (success)
                    {
                    _gastos.Clear();
                    foreach (var g in gastos)
                        _gastos.Add(g);

                    // Asegura que la vista pueda enlazar a la colección
                    GrillaGastos.ItemsSource = _gastos;
                    }
                else
                    {
                    MessageBox.Show(message ?? "No se pudieron obtener los gastos.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error al obtener gastos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private void Button_Click(object sender, RoutedEventArgs e)
            {
            escritorio.CambioEstado("nGastos", "Normal", "O");
            }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
            {
            var win = new WiGasto(_gastos, null, null);
            win.ShowDialog();

            }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
            {
            // Pattern matching para extraer un ID válido (int no nullable)
            if (GrillaGastos.SelectedItem is GastoDTO { ID: var id } seleccionado && id > 0)
                {
                try
                    {
                    // Obtener detalles del gasto (usa DatosWeb helper)
                    var (success, message, detalles) = await DatosWeb.ObtenerDetalleGastoAsync(id);
                    if (!success)
                        {
                        MessageBox.Show($"No se pudieron obtener los detalles del gasto.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                        }

                    // Abrir ventana de edición (usa el constructor que tengas)
                    var win = new WiGasto(_gastos, seleccionado, detalles);
                    // Si WiGasto tiene un método o propiedad para cargar el gasto y sus detalles, asignarlo aquí:
                    // win.CargarGasto(seleccionado, detalles);
                    win.ShowDialog();
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show($"Error al editar gasto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            else
                {
                MessageBox.Show("Seleccione un gasto válido para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
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

        private void btnControl_Click(object sender, RoutedEventArgs e)
            {

            }
        }
    }
