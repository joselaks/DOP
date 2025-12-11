using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DOP.Datos;
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


        public WiControl1(PresupuestoDTO presupuestosRef)
            {
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
