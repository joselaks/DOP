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
using Biblioteca; // <-- agregado para ControlPresupuesto

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
            Loaded += WiControlPres_Loaded;

            // opcional: seleccionar el primero si existe
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
            UpdateSwitch();
            }

        private void UpdateSwitch()
            {
            // Protección contra llamadas tempranas durante InitializeComponent donde los controles pueden ser null
            if (RbTareas is null || gridTareas is null || gridInsumos is null)
                return;

            if (RbTareas.IsChecked == true)
                {
                gridTareas.Visibility = Visibility.Visible;
                gridInsumos.Visibility = Visibility.Collapsed;
                }
            else
                {
                gridTareas.Visibility = Visibility.Collapsed;
                gridInsumos.Visibility = Visibility.Visible;
                }
            }

        private void grillaArbol_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
            {

            }

        private void grillaArbol_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
            {

            }

        private void grillaArbol_KeyDown(object sender, KeyEventArgs e)
            {

            }

        private void grillaArbol_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
            {

            }
        }
    }