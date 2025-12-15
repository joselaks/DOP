using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DataObra.Documentos.Ventanas;
using DOP.Datos;
using Syncfusion.SfSkinManager;
using Syncfusion.SfSkinManager;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using static Biblioteca.Control1;

namespace DataObra.Presupuestos.Ventanas
    {
    /// <summary>
    /// Lógica de interacción para WiControl1.xaml
    /// </summary>
    public partial class WiControl1 : RibbonWindow, INotifyPropertyChanged
        {
        private PresupuestoDTO _encabezado = new();
        private Control1 _control = new Control1();
        private bool _tieneCambiosPendientes = false;


        public WiControl1(PresupuestoDTO presupuestosRef)
            {
            SfSkinManager.ApplyThemeAsDefaultStyle = true;
            SfSkinManager.SetTheme(this, new Theme("FluentLight"));
            InitializeComponent();
            _encabezado = presupuestosRef;


            Loaded += WiControlPres_Loaded;
            }

        private async void WiControlPres_Loaded(object sender, RoutedEventArgs e)
            {

            int presupuestoID = _encabezado?.ID ?? 0;
            if (presupuestoID <= 0)
                {
                MessageBox.Show("El presupuesto no tiene ID válido.", "Control Presupuesto", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }

            var resultado = await DatosWeb.ObtenerConceptosRelacionesYDetallesAsync(presupuestoID);

            if (!resultado.Success)
                {
                MessageBox.Show($"No se pudieron obtener los datos: {resultado.Message}", "Control Presupuesto", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                }

            // Construir árbol y mostrar en la UI
            _control = new Control1();
            _control.ConstruirConceptosConGastos(resultado.Conceptos, resultado.Detalles);
            var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };

            colImportePrevisto.HeaderText = $"{_control.TotalPreviso.ToString("N2", cultura)}";
            colImporteReal.HeaderText = $"{_control.TotalReal.ToString("N2", cultura)}";
            colImporteSaldo.HeaderText = $"{_control.TotalSaldo.ToString("N2", cultura)}";



            this.DataContext = _control;                 // para bindings generales (p.ej. {Binding Arbol})
            if (grillaListados != null)
                grillaListados.ItemsSource = _control.ConceptosConGastosPropios; // TreeGrid/TreeView del XAML, si usa ItemsSource

            grillaGastos.RowDragDropController.DragStart += RowDragDropController_DragStart;
            grillaListados.RowDragDropController.Drop += RowDragDropController_Drop;


            }

        private void VerDocumento_Click(object sender, RoutedEventArgs e)
            {
            // NUEVO: bloquear si hay cambios pendientes
            if (_tieneCambiosPendientes)
                {
                MessageBox.Show(
                    "Hay cambios de reasignación pendientes de grabar. Guárdalos o descártalos antes de ver el documento.",
                    "Cambios pendientes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
                }

            var item = grillaGastos.SelectedItem;
            if (item == null)
                {
                MessageBox.Show("No hay ningún registro seleccionado.", "Ver documento", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
                }

            int gastoID = 0;
            if (item is Control1.GastoPropio gp)
                {
                gastoID = gp.GastoID ?? 0;
                }
            else
                {
                var propGastoID = item.GetType().GetProperty("GastoID");
                if (propGastoID != null)
                    {
                    var val = propGastoID.GetValue(item);
                    gastoID = val is int i ? i : (val as int?) ?? 0;
                    }
                }

            if (gastoID <= 0)
                {
                MessageBox.Show("El registro seleccionado no tiene un GastoID válido.", "Ver documento", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }

            var win = new WiGasto(gastoID, esCobro: false)
                {
                Owner = this
                };
            win.ShowDialog();
            }

        private void RowDragDropController_DragStart(object? sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridRowDragStartEventArgs e)
            {
            }

        private void RowDragDropController_Drop(object? sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridRowDropEventArgs e)
            {
            GastoPropio nodoMovido = null;
            // Origen: GastoPropio arrastrado
            if (e.DraggingNodes != null && e.DraggingNodes.Count > 0)
                {
                nodoMovido = e.DraggingNodes[0].Item as GastoPropio;
                }
            else
                {
                return;
                }
            // Destino: ConceptoConGastosPropio sobre el que se suelta
            var targetConcepto = e.TargetNode?.Item as Control1.ConceptoConGastosPropio;
            if (targetConcepto == null) return;

            // Buscar el concepto origen (el que tenía el gasto antes)
            var conceptoOrigen = _control.ConceptosConGastosPropios
                .FirstOrDefault(c => c.Gastos.Contains(nodoMovido));
            if (conceptoOrigen == null) return;

            // Eliminar de la lista original
            conceptoOrigen.Gastos.Remove(nodoMovido);

            // Actualizar el InsumoID y agregar al destino
            nodoMovido.InsumoID = targetConcepto.ConceptoID;
            targetConcepto.Gastos.Add(nodoMovido);

            // Recalcular totales si es necesario
            _control.Recalculo();

            // Notificar cambios en la UI
            OnPropertyChanged(nameof(_control.ConceptosConGastosPropios));
            grillaGastos.ItemsSource = targetConcepto.Gastos;

            // --- Seleccionar y enfocar el registro de destino ---
            if (grillaListados != null)
                {
                grillaListados.SelectedItem = targetConcepto;

                // Obtener el índice de la fila del targetConcepto
                int rowIndex = grillaListados.ResolveToRowIndex(targetConcepto);

                // ScrollInView espera un RowColumnIndex
                var rowColIndex = new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(rowIndex, 0);
                grillaListados.ScrollInView(rowColIndex);

                grillaListados.Focus();
                }
            _tieneCambiosPendientes = true;
            }

        private async void BrnGuardar_Click(object sender, RoutedEventArgs e)
            {
            await GuardarReasignacionesAsync();
            }

        private async void BtnGuardarSalir_Click(object sender, RoutedEventArgs e)
            {
            if (await GuardarReasignacionesAsync())
                {
                this.Close();
                }
            }

        //// Método auxiliar para guardar las reasignaciones
        private async Task<bool> GuardarReasignacionesAsync()
            {
            _control.GenerarReasignacionDesdeComparacion();


            var cambios = _control.reAasignacion;

            if (cambios == null || cambios.Count == 0)
                {
                MessageBox.Show("No hay cambios de reasignación para guardar.", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
                }

            var resultado = await DatosWeb.ReasignarGastosAsync(cambios.ToList());

            if (resultado.Success)
                {
                MessageBox.Show("Cambios guardados correctamente.", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);
                _control.LimpiarReasignaciones(); // limpiar después de guardar
                _tieneCambiosPendientes = false;
                return true;
                }
            else
                {
                MessageBox.Show($"Error al guardar cambios: {resultado.Message}", "Guardar", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
                }
            }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
            {

            }

        private void SaleExcel_Click(object sender, RoutedEventArgs e)
            {

            }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void grillaListados_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
            {
            var seleccionado = this.grillaListados.SelectedItem as ConceptoConGastosPropio;
            if (seleccionado != null && seleccionado.Gastos != null)
                {
                grillaGastos.ItemsSource = seleccionado.Gastos;
                }
            else
                {
                grillaGastos.ItemsSource = null;
                }
            }
        }
    }
