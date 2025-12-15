using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DataObra.Presupuestos.Controles;
using DataObra.Presupuestos.Controles.SubControles;
using DOP;
using DOP.Presupuestos.Clases;
using DOP.Presupuestos.Controles;
using Microsoft.Win32;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using Syncfusion.UI.Xaml.Grid;

namespace DataObra.Documentos.Ventanas
    {
    /// <summary>
    /// Lógica de interacción para WiGasto.xaml
    /// </summary>
    public partial class WiGasto : RibbonWindow
        {
        // Colección que alimenta gridDetalle
        public Gasto objeto;
        public ObservableCollection<GastoDTO> gastos = new();
        // Guardar la referencia del encabezado original pasado al constructor
        private GastoDTO? originalEncabezado;

        public WiGasto(ObservableCollection<GastoDTO> _gastos, GastoDTO encabezado, List<GastoDetalleDTO> detalle, bool tipoGasto)
            {
            SfSkinManager.ApplyThemeAsDefaultStyle = true;
            SfSkinManager.SetTheme(this, new Theme("FluentLight"));

            InitializeComponent();
            objeto = new Gasto(encabezado, detalle, tipoGasto);
            this.DataContext = objeto;
            gastos = _gastos;

            // Guardar la referencia del encabezado original para reemplazo en la colección
            originalEncabezado = encabezado;

            if (grillaEncabezado != null)
                grillaEncabezado.DataContext = objeto.encabezado;

            gridDetalle.ItemsSource = objeto.detalleGrabar;
            this.gridDetalle.RecordDeleting += GridDetalle_RecordDeleting;

            RecalcularImportesInicial();
            }

        // Sobrecarga: abrir por ID en modo solo lectura (opcionalmente como Cobro)
        public WiGasto(int gastoID, bool esCobro = false)
            {
            SfSkinManager.ApplyThemeAsDefaultStyle = true;
            SfSkinManager.SetTheme(this, new Theme("FluentLight"));

            InitializeComponent();

            // Forzar modo solo lectura en grillas
            if (gridDetalle != null)
                {
                gridDetalle.AllowEditing = false;
                gridDetalle.AllowDeleting = false;
                }
            if (grillaEncabezado != null)
                grillaEncabezado.IsEnabled = false;

            // Inicializar colección local vacía
            gastos = new ObservableCollection<GastoDTO>();

            // Cargar datos desde el servidor
            _ = CargarGastoSoloLecturaAsync(gastoID, esCobro);
            }

        private async Task CargarGastoSoloLecturaAsync(int gastoID, bool esCobro)
            {
            try
                {
                var (success, message, encabezado, detalles) = await DOP.Datos.DatosWeb.ObtenerGastoAsync(gastoID, esCobro);

                if (!success || encabezado == null)
                    {
                    MessageBox.Show(string.IsNullOrWhiteSpace(message) ? "No se encontró el documento solicitado." : message,
                                    "Consultar gasto", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.Close();
                    return;
                    }

                // Guardar referencia del encabezado original
                originalEncabezado = encabezado;

                // tipoGasto se asume como !esCobro (gasto=true, cobro=false)
                bool tipoGasto = !esCobro;

                // Construir el objeto de trabajo y enlazar
                objeto = new Gasto(encabezado, detalles ?? new List<GastoDetalleDTO>(), tipoGasto);
                this.DataContext = objeto;

                if (grillaEncabezado != null)
                    grillaEncabezado.DataContext = objeto.encabezado;

                if (gridDetalle != null)
                    {
                    gridDetalle.ItemsSource = objeto.detalleGrabar;
                    this.gridDetalle.RecordDeleting += GridDetalle_RecordDeleting;
                    }

                // Recalcular importes locales (por si el backend no los envía calculados)
                RecalcularImportesInicial();
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error al cargar el gasto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                }
            }


        private void RecalcularImportesInicial()
            {
            if (objeto?.detalleGrabar == null)
                return;

            foreach (var det in objeto.detalleGrabar)
                {
                det.Importe = det.Cantidad * det.PrecioUnitario;
                }

            objeto.encabezado.Importe = objeto.detalleGrabar.Sum(d => d?.Importe ?? 0);

            if (grillaEncabezado != null)
                {
                grillaEncabezado.DataContext = null;
                grillaEncabezado.DataContext = objeto.encabezado;
                }
            }

        private void GridDetalle_RecordDeleting(object? sender, RecordDeletingEventArgs e)
            {
            try
                {
                var eliminados = e.Items?.OfType<GastoDetalleDTO>().ToList() ?? new List<GastoDetalleDTO>();
                if (eliminados.Count == 0)
                    return;

                // Recalcular total excluyendo los que se eliminan (todavía están en la lista en este evento)
                var setEliminados = new HashSet<GastoDetalleDTO>(eliminados);
                var nuevoTotal = objeto.detalleGrabar
                                        .Where(d => d != null && !setEliminados.Contains(d))
                                        .Sum(d => d!.Importe);

                objeto.encabezado.Importe = nuevoTotal;

                // Refrescar encabezado
                if (grillaEncabezado != null)
                    {
                    grillaEncabezado.DataContext = null;
                    grillaEncabezado.DataContext = objeto.encabezado;
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error al recalcular total tras borrar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private void AgregarRegistro_Click(object sender, RoutedEventArgs e)
            {
            // Crear un nuevo detalle con valores por defecto coherentes con el encabezado
            var nuevo = new GastoDetalleDTO
                {
                ID = 0,
                // TipoID de detalle por defecto; si manejas tipos ('M','S', etc.), ajusta aquí
                TipoID = '0',
                UsuarioID = objeto.encabezado.UsuarioID,
                CuentaID = objeto.encabezado.CuentaID,
                Moneda = objeto.encabezado.Moneda,
                Fecha = objeto.encabezado.FechaDoc,
                FactorConcepto = 1.0000m,

                Cantidad = 0,
                PrecioUnitario = 0,
                Importe = 0,
                Descrip = string.Empty,
                Unidad = string.Empty
                };

            // Asegurar la lista y añadir
            if (objeto.detalleGrabar == null)
                objeto.detalleGrabar = new List<GastoDetalleDTO>();

            objeto.detalleGrabar.Add(nuevo);

            // Re-enlazar ItemsSource para que la grilla actualice (detalleGrabar es List<T>)
            gridDetalle.ItemsSource = null;
            gridDetalle.ItemsSource = objeto.detalleGrabar;

            // Seleccionar el nuevo registro para que el usuario pueda editarlo inmediatamente
            gridDetalle.SelectedItem = nuevo;
            gridDetalle.Focus();
            }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
            {
            // Ejecuta el guardado asincrónico y cierra/indica resultado
            if (await GuardarGastoAsync())
                {
                MessageBox.Show("Gasto guardado correctamente.", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);
                try
                    {
                    this.DialogResult = true;
                    }
                catch (InvalidOperationException)
                    {
                    // No fue abierto como diálogo; seguir
                    }
                this.Close();
                }
            }

        private void BtnSalirSinGuardar_Click(object sender, RoutedEventArgs e)
            {
            try
                {
                this.DialogResult = false;
                }
            catch (InvalidOperationException)
                {
                }
            this.Close();
            }

        private async Task<bool> GuardarGastoAsync()
            {
            // Actualizar totales y marcas temporales
            try
                {
                // Actualizar importe total desde detalleGrabar
                if (objeto?.detalleGrabar != null)
                    {
                    objeto.encabezado.Importe = objeto.detalleGrabar.Sum(d => d?.Importe ?? 0);
                    }

                objeto.encabezado.FechaEditado = DateTime.Now;

                // Asegurar UsuarioID si existe App.IdUsuario
                try
                    {
                    objeto.encabezado.UsuarioID = App.IdUsuario;
                    }
                catch
                    {
                    // Si no existe App.IdUsuario no hacemos nada
                    }

                // Empaquetar request usando la lógica de Gasto (compara detalleLeer y detalleGrabar)
                Biblioteca.DTO.ProcesarGastoRequest oGrabar = objeto.EmpaquetarGasto();

                // Llamada al servicio web para procesar el gasto
                var procesarTaskResult = await DOP.Datos.DatosWeb.ProcesarGastoAsync(oGrabar);
                var success = procesarTaskResult.Success;
                var message = procesarTaskResult.Message;
                var procesaResult = procesarTaskResult.Result;

                if (success)
                    {
                    // Actualiza listas para próxima ejecución (sincronizar versiones)
                    objeto.detalleLeer = objeto.detalleGrabar.Select(x => x).ToList();

                    // Si fue nuevo, asignar ID y FechaCreado si la API devolvió DocumentoID
                    if ((objeto.encabezado.ID == 0) && (procesaResult?.DocumentoID ?? 0) > 0)
                        {
                        objeto.encabezado.FechaCreado = DateTime.Today;
                        objeto.encabezado.ID = procesaResult.DocumentoID;
                        }

                    // Reemplazar en la colección 'gastos' la instancia original por la instancia actualizada
                    if (gastos != null)
                        {
                        int idx = -1;
                        // Primero intentar por referencia (la instancia que se pasó al constructor)
                        if (originalEncabezado != null)
                            idx = gastos.IndexOf(originalEncabezado);

                        // Si no está por referencia, intentar localizar por ID
                        if (idx == -1 && objeto.encabezado.ID != 0)
                            idx = gastos.ToList().FindIndex(g => g.ID == objeto.encabezado.ID);

                        if (idx >= 0)
                            {
                            gastos[idx] = objeto.encabezado;
                            }
                        else
                            {
                            gastos.Add(objeto.encabezado);
                            }
                        }

                    var escritorio = Application.Current.Windows
                                    .OfType<DataObra.Interfaz.Ventanas.WiEscritorio>()
                                    .FirstOrDefault();

                    if (escritorio != null && procesaResult != null)
                        {
                        // Ejecutar en el hilo de UI del escritorio
                        escritorio.Dispatcher.Invoke(() => escritorio.UpdatePresupuestosFromGastoResult(procesaResult));
                        }

                    return true;
                    }
                else
                    {
                    var msg = string.IsNullOrEmpty(message) ? "Error desconocido al guardar el gasto." : message;
                    MessageBox.Show($"Error al guardar el gasto: {msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Excepción al guardar el gasto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
                }
            }

        private void gridDetalle_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellBeginEditEventArgs e)
            {

            }

        private void gridDetalle_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
            {
            try
                {
                int colIndex = e.RowColumnIndex.ColumnIndex;
                if (colIndex < 0 || colIndex >= gridDetalle.Columns.Count)
                    return;

                var mappingName = gridDetalle.Columns[colIndex].MappingName;
                if (mappingName != "Cantidad" && mappingName != "PrecioUnitario")
                    return;

                // Obtener el item actual (la fila en la que se estaba editando)
                var item = gridDetalle.CurrentItem as GastoDetalleDTO
                           ?? gridDetalle.SelectedItem as GastoDetalleDTO;
                if (item == null)
                    return;

                // Recalcular importe
                item.Importe = item.Cantidad * item.PrecioUnitario;

                // Recalcular total encabezado
                if (objeto?.detalleGrabar != null)
                    {
                    objeto.encabezado.Importe = objeto.detalleGrabar.Sum(d => d?.Importe ?? 0);

                    // Forzar refresco del encabezado si no se actualiza solo
                    if (grillaEncabezado != null)
                        {
                        grillaEncabezado.DataContext = null;
                        grillaEncabezado.DataContext = objeto.encabezado;
                        }
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error al recalcular importe: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }