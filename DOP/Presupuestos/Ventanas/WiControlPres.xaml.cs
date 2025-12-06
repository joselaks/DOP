using Bibioteca.Clases;
using Biblioteca.DTO;
using DataObra.Presupuestos.Controles;
using DataObra.Presupuestos.Controles.SubControles;
using DocumentFormat.OpenXml.Office2010.Excel;
using DOP.Datos;
using DOP.Presupuestos.Controles;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Biblioteca;
using Syncfusion.UI.Xaml.TreeGrid; // <-- agregado para ControlPresupuesto

namespace DataObra.Presupuestos.Ventanas
    {
    /// <summary>
    /// Lógica de interacción para WiControlPres.xaml
    /// </summary>
    public partial class WiControlPres : RibbonWindow, INotifyPropertyChanged
        {

        public Presupuesto Objeto;

        private List<GastoDetalleDTO> _detallesGastos = new();
        private PresupuestoDTO _encabezado = new();

        // Instancia del generador de árbol para Control de Presupuesto
        private ControlPresupuesto _control = new ControlPresupuesto();

        public WiControlPres(PresupuestoDTO presupuestosRef)
            {
            InitializeComponent();
            _encabezado = presupuestosRef;
            grillaArbol.ChildPropertyName = "Inferiores";
            this.grillaArbol.QueryCoveredRange += OnQueryCoveredRange;
            Loaded += WiControlPres_Loaded;

            // opcional: seleccionar el primero si existe
            }

        private void OnQueryCoveredRange(object? sender, TreeGridQueryCoveredRangeEventArgs e)
            {
            var record = e.Record as Nodo;
            if (record != null && record.Tipo == "R")
                {
                //Customize here based on your requirement
                e.Range = new TreeGridCoveredCellInfo(2, 7, e.RowColumnIndex.RowIndex);
                e.Handled = true;
                }

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
            _detallesGastos = resultado.Detalles;
            _control.Construir(resultado.Conceptos, resultado.Relaciones, resultado.Detalles);

            // Hacer visible el resultado en la ventana
            this.DataContext = _control;                 // para bindings generales (p.ej. {Binding Arbol})
            if (grillaArbol != null)
                grillaArbol.ItemsSource = _control.Arbol; // TreeGrid/TreeView del XAML, si usa ItemsSource
            if (grillaListados != null)
                grillaListados.ItemsSource = _control.Insumos; // TreeGrid/TreeView del XAML, si usa ItemsSource

            }

        private void SaleExcel_Click(object sender, RoutedEventArgs e)
            {

            }

        private void BrnGuardar_Click(object sender, RoutedEventArgs e)
            {

            }

        private void BtnGuardarSalir_Click(object sender, RoutedEventArgs e)
            {

            }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
            {

            }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Switch_Checked(object sender, RoutedEventArgs e)
        {
            // Evitar ejecutar antes de estar cargado completamente
            if (!IsLoaded) return;

            // Validar Grid y columnas
            if (ppal == null || ppal.ColumnDefinitions == null || ppal.ColumnDefinitions.Count < 3)
                return;

            var rb = sender as RadioButton;
            if (rb == null || rb.IsChecked != true)
                return;

            // Usar el Name del RadioButton para evitar dependencias de campos nulos
            switch (rb.Name)
            {
                case "RbTareas":
                    // Tareas: * | 3 | 0
                    ppal.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    ppal.ColumnDefinitions[1].Width = new GridLength(3, GridUnitType.Pixel);
                    ppal.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Pixel);
                    break;

                case "RbInsumos":
                    // Insumos: 0 | 3 | *
                    ppal.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Pixel);
                    ppal.ColumnDefinitions[1].Width = new GridLength(3, GridUnitType.Pixel);
                    ppal.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                    break;

                case "RbAmbos":
                    // Ambos: * | 3 | *
                    ppal.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    ppal.ColumnDefinitions[1].Width = new GridLength(3, GridUnitType.Pixel);
                    ppal.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                    break;
            }
        }


        private void grillaArbol_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
            {

            }

        private void grillaArbol_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
            {
            _control.RecalculoCompleto();
            }

        private void grillaArbol_KeyDown(object sender, KeyEventArgs e)
            {

            }

        private void grillaArbol_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
            {

            }
        }
    }