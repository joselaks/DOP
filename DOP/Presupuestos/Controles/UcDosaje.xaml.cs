using Bibioteca.Clases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DOP.Presupuestos.Controles
{
    /// <summary>
    /// Lógica de interacción para UcDosaje.xaml
    /// </summary>
    public partial class UcDosaje : UserControl
    {
        Nodo NodoAnalizado;
        Presupuesto Presup;


        public UcDosaje(Presupuesto presup) 
        {
            InitializeComponent();
            Presup = presup;
            
            }


        public void MostrarInferiores(Nodo analizado)
        {
            NodoAnalizado = analizado;
            grillaDetalle.ItemsSource = NodoAnalizado.Inferiores;
            nombreTarea.Text = NodoAnalizado.Descripcion;
            // Asignar el valor explícitamente al HeaderText
            var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
            colImporte1.HeaderText = $"{NodoAnalizado.PU1.ToString("N2", cultura)}";

            }

        private void grillaDetalle_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
            {

            }

        private void grillaDetalle_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
            {

            }

        private void grillaDetalle_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
            {

            }

        private void grillaDetalle_KeyDown(object sender, KeyEventArgs e)
            {

            }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
            {
            if (sender is MenuItem menuItem)
                {
                switch (menuItem.Header)
                    {
                    case "Material":
                        AgregarNodo("M");
                        break;
                    case "Mano de obra":
                        // Lógica para eliminar
                        break;
                    }
                }

            }

        private void AgregarNodo(string tipo)
            {
            Nodo sele = NodoAnalizado;
            if (sele == null || (tipo != "A" && sele.Tipo != "T"))
                {
                MessageBox.Show("Debe seleccionar una tarea o auxiliar para agregar el nuevo nodo.");
                return;
                }

            (Nodo nuevoNodo, string mensaje) = Presup.agregaNodo(tipo, sele);
            Presup.recalculo(Presup.Arbol, true, 0, true);

            }
        }
}
