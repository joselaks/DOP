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
using System.Net.Http;
using System.Net.Http.Headers;
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
using System.Text.Json;
using System.IO.Compression;
using Syncfusion.XlsIO;

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
            // Determinar si se pulsó el botón "Nuevo Gasto" (btnNuevoEgr)
            bool tipoGasto = false;
            if (sender is System.Windows.Controls.Button btn)
                {
                tipoGasto = string.Equals(btn.Name, "btnNuevoEgr", StringComparison.OrdinalIgnoreCase);
                }

            // Abrir la ventana pasando el flag tipoGasto
            var win = new WiGasto(_gastos, null, null, tipoGasto);
            win.ShowDialog();
            }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
            {
            // Pattern matching para extraer un ID válido (int no nullable)
            if (GrillaGastos.SelectedItem is GastoDTO { ID: var id } seleccionado && id > 0)
                {
                try
                    {
                    bool esCobro = false;
                    bool tipoGasto = false;
                    if (seleccionado.TipoID == 20)
                        {
                        esCobro = true;
                        tipoGasto = false;
                        }


                    // Obtener detalles del gasto/cobro (usa DatosWeb helper con el flag)
                    var (success, message, detalles) = await DatosWeb.ObtenerDetalleGastoAsync(id, esCobro);
                    if (!success)
                        {
                        MessageBox.Show($"No se pudieron obtener los detalles del {(esCobro ? "cobro" : "gasto")}.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                        }

                    // Abrir ventana de edición con los detalles recuperados
                    var win = new WiGasto(_gastos, seleccionado, detalles, tipoGasto);
                    win.ShowDialog();
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show($"Error al editar {(seleccionado.TipoID == 20 ? "cobro" : "gasto")}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            else
                {
                MessageBox.Show("Seleccione un gasto válido para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        private async void btnBorrar_Click(object sender, RoutedEventArgs e)
            {
            // Validar selección
            if (!(GrillaGastos.SelectedItem is GastoDTO { ID: var id } seleccionado) || id <= 0)
                {
                MessageBox.Show("Seleccione un gasto válido para borrar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }

            // Confirmación del usuario
            var confirmar = MessageBox.Show($"¿Confirma eliminar el {(seleccionado.TipoID == 20 ? "cobro" : "gasto")} ID {id}?\nEsta acción eliminará también sus detalles.",
                                            "Confirmar borrado", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmar != MessageBoxResult.Yes) return;

            try
                {
                // Llamada al servicio web para borrar
                var (success, message) = await DatosWeb.BorrarGastoAsync(id);
                if (success)
                    {
                    // Eliminar de la colección local y actualizar UI
                    if (_gastos.Contains(seleccionado))
                        _gastos.Remove(seleccionado);

                    MessageBox.Show(message ?? "Gasto eliminado correctamente.", "Eliminar", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                else
                    {
                    MessageBox.Show($"No se pudo eliminar el {(seleccionado.TipoID == 20 ? "cobro" : "gasto")}.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Excepción al eliminar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
            {

            }

        private void GrillaPresupuestos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {

            }

        private async void btnControl_Click(object sender, RoutedEventArgs e)
            {

            }
        }
    }
