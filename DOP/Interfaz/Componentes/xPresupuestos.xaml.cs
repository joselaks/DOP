using Biblioteca.DTO;
using DataObra.Interfaz.Ventanas;
using DataObra.Presupuestos.Ventanas;
using DocumentFormat.OpenXml.Drawing.Charts;
using DOP;
using DOP.Datos;
using DOP.Presupuestos.Ventanas;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataObra.Interfaz.Componentes
{
    /// <summary>
    /// Lógica de interacción para xPresupuestos.xaml
    /// </summary>
    public partial class xPresupuestos : UserControl
    {
        private WiEscritorio escritorio;

        public xPresupuestos(WiEscritorio _escritorio)
        {
            InitializeComponent();
            escritorio = _escritorio;
            this.Loaded += XPresupuestos_Loaded;
            }

        private async void XPresupuestos_Loaded(object sender, RoutedEventArgs e)
            {
            
                GrillaPresupuestos.ItemsSource = escritorio._presupuestos;
                
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            escritorio.CambioEstado("nPresupuestos", "Normal", "O");
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
            {
            var win = new WiPres(null, null, null, escritorio._presupuestos);
            win.ShowDialog();
            }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
            {
            if (GrillaPresupuestos.SelectedItem is PresupuestoDTO seleccionado && seleccionado.ID.HasValue)
                {
                // Obtener conceptos y relaciones antes de abrir la ventana
                var (ok, msg, conceptos, relaciones) = await DatosWeb.ObtenerConceptosYRelacionesAsync(seleccionado.ID.Value);
                if (ok)
                    {
                    // Aquí puedes pasar conceptos y relaciones a la ventana WiPresupuesto si lo necesitas
                    var copia = PresupuestoDTO.CopiarPresupuestoDTO(seleccionado);
                    var wiPresupuesto = new WiPres(copia, conceptos, relaciones, escritorio._presupuestos);
                    wiPresupuesto.Owner = escritorio;
                    wiPresupuesto.ShowDialog();
                    // Si necesitas usar conceptos y relaciones después, puedes hacerlo aquí
                    }
                else
                    {
                    MessageBox.Show($"No se pudieron obtener los datos del presupuesto.\n{msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            else
                {
                MessageBox.Show("Seleccione un presupuesto para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        private async void btnBorrar_Click(object sender, RoutedEventArgs e)
            {
            if (GrillaPresupuestos.SelectedItem is PresupuestoDTO eliminar && eliminar.ID.HasValue)
                {
                var result = MessageBox.Show("¿Está seguro que desea eliminar el presupuesto seleccionado?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    {
                    var (success, message) = await DatosWeb.BorrarPresupuestoAsync(eliminar.ID.Value);
                    if (success)
                        {
                        // Quitar de la colección y refrescar la grilla
                        escritorio._presupuestos.Remove(eliminar);
                        GrillaPresupuestos.ItemsSource = null;
                        GrillaPresupuestos.ItemsSource = escritorio._presupuestos;
                        MessageBox.Show(message, "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    else
                        {
                        MessageBox.Show($"No se pudo eliminar el presupuesto.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            else
                {
                MessageBox.Show("Seleccione un presupuesto para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }

        private void GrillaPresupuestos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {

            }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
            {
            if (sender is MenuItem menuItem)
                {
                // Puedes usar Name o Header según cómo esté definido tu menú
                switch (menuItem.Header?.ToString())
                    {
                    case "Nuevo":
                        var win = new WiPres(null, null, null, escritorio._presupuestos);
                        win.ShowDialog();
                        break;
                    case "Editar":
                        if (GrillaPresupuestos.SelectedItem is PresupuestoDTO seleccionado && seleccionado.ID.HasValue)
                            {
                            var (ok, msg, conceptos, relaciones) = await DatosWeb.ObtenerConceptosYRelacionesAsync(seleccionado.ID.Value);
                            if (ok)
                                {
                                var copia = PresupuestoDTO.CopiarPresupuestoDTO(seleccionado); // <-- aquí el cambio
                                var wiPresupuesto = new WiPres(copia, conceptos, relaciones, escritorio._presupuestos);
                                wiPresupuesto.Owner = escritorio;
                                wiPresupuesto.ShowDialog();
                                }
                            else
                                {
                                MessageBox.Show($"No se pudieron obtener los datos del presupuesto.\n{msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        else
                            {
                            MessageBox.Show("Seleccione un presupuesto para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        break;

                    case "Borrar":
                        if (GrillaPresupuestos.SelectedItem is PresupuestoDTO eliminar && eliminar.ID.HasValue)
                            {
                            var result = MessageBox.Show("¿Está seguro que desea eliminar el presupuesto seleccionado?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                                {
                                var (success, message) = await DatosWeb.BorrarPresupuestoAsync(eliminar.ID.Value);
                                if (success)
                                    {
                                    // Quitar de la colección y refrescar la grilla
                                    escritorio._presupuestos.Remove(eliminar);
                                    GrillaPresupuestos.ItemsSource = null;
                                    GrillaPresupuestos.ItemsSource = escritorio._presupuestos;
                                    MessageBox.Show(message, "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                else
                                    {
                                    MessageBox.Show($"No se pudo eliminar el presupuesto.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                        else
                            {
                            MessageBox.Show("Seleccione un presupuesto para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        break;

                    default:
                        MessageBox.Show("Opción de menú no implementada.", "Menú", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                }
            }

        private void btnControl_Click(object sender, RoutedEventArgs e)
            {
            var win = new WiInicioControlPres(escritorio._presupuestos);
            win.Owner = escritorio;
            // Se muestra modal y, por la XAML, se centra respecto al Owner
            win.ShowDialog();
            }
        }
    }
