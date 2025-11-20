using Bibioteca.Clases;
using Biblioteca.DTO;
using DataObra.Presupuestos.Controles;
using DataObra.Presupuestos.Controles.SubControles;
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

namespace DataObra.Presupuestos.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiControlPres.xaml
    /// </summary>
    public partial class WiControlPres : RibbonWindow, INotifyPropertyChanged
    {

        public Presupuesto Objeto;

        public ObservableCollection<PresupuestoDTO> PresupuestosRef { get; }

        private PresupuestoDTO _selectedPresupuesto;
        public PresupuestoDTO SelectedPresupuesto
        {
            get => _selectedPresupuesto;
            set
            {
                if (_selectedPresupuesto != value)
                {
                    _selectedPresupuesto = value;
                    OnPropertyChanged(nameof(SelectedPresupuesto));
                    // Aquí puede colocarse lógica a ejecutar al cambiar la selección, por ejemplo cargar el presupuesto seleccionado.
                }
            }
        }

        public WiControlPres(PresupuestoDTO presupuestosRef)
        {
            InitializeComponent();


            // opcional: seleccionar el primero si existe
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
