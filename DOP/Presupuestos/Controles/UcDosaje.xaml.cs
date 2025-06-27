using Bibioteca.Clases;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeGrid;
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
        Nodo anterior = new Nodo();
        public Presupuesto Objeto;
        private object _originalValue;


        public UcDosaje(Presupuesto presup)
            {
            InitializeComponent();
            Objeto = presup;
            this.grillaDetalle.ChildPropertyName = "Inferiores";

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
            var column = grillaDetalle.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var record = grillaDetalle.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
            // Clonar el objeto record y asignarlo a anterior
            anterior = Objeto.clonar(record);

            if (column == "ID")
                {
                _originalValue = record.ID;
                }
            }

        private void grillaDetalle_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
            {
            var column = grillaDetalle.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var editado = grillaDetalle.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
            edicion(editado, column);
            var undoRegistro = new Cambios
                {
                TipoCambio = "Tipeo",
                antesCambio = anterior,
                despuesCambio = Objeto.clonar(editado),
                PropiedadCambiada = column,
                OldValue = _originalValue,
                NewValue = editado.GetType().GetProperty(column).GetValue(editado)
                };
            }

        private void edicion(Nodo? editado, string column)
            {
            switch (column)
                {
                case "ID":
                    Objeto.cambiaCodigo(Objeto.Arbol, editado.ID, _originalValue.ToString());
                    break;
                case "Cantidad":
                    CambioAuxiliar dato = new CambioAuxiliar();
                    dato.IdInferior = editado.ID;
                    dato.IdSuperior = Objeto.FindParentNode(Objeto.Arbol, editado, null).ID;
                    dato.Cantidad = editado.Cantidad;
                    Objeto.cambioCantidadAuxiliar(Objeto.Arbol, dato);
                    break;
                default:
                    Objeto.mismoCodigo(Objeto.Arbol, editado);
                    break;
                }
            Objeto.recalculo(Objeto.Arbol, true, 0, true);
            }

        private void grillaDetalle_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
            {

            }

        private void grillaDetalle_KeyDown(object sender, KeyEventArgs e)
            {
            if (e.Key == Key.Delete)
                {
                var selectedItems = grillaDetalle.SelectedItems;
                var itemsToRemove = new List<Nodo>();
                foreach (var item in selectedItems)
                    {
                    itemsToRemove.Add(item as Nodo);
                    }
                foreach (var item in itemsToRemove)
                    {
                    var result = RemoveItemRecursively(Objeto.Arbol, item);

                    }
                }
            }

        private (bool, Nodo, int) RemoveItemRecursively(ObservableCollection<Nodo> collection, Nodo itemToRemove, Nodo parent = null)
            {
            int index = collection.IndexOf(itemToRemove);
            if (index != -1)
                {
                collection.RemoveAt(index);
                return (true, parent, index);
                }

            foreach (var item in collection)
                {
                if (item.HasItems)
                    {
                    var result = RemoveItemRecursively(item.Inferiores, itemToRemove, item);
                    if (result.Item1)
                        {
                        return result;
                        }
                    }
                }

            return (false, null, -1);
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
                        AgregarNodo("D");
                        break;
                    case "Equipo":
                        AgregarNodo("E");
                        break;
                    case "Subcontrato":
                        AgregarNodo("S");
                        break;
                    case "Otro":
                        AgregarNodo("O");
                        break;
                    case "Auxiliar":
                        AgregarNodo("A");
                        break;

                    }
                }

            }

        private void AgregarNodo(string tipo)
            {
            Nodo sele = NodoAnalizado;

            // Si hay un nodo seleccionado en la grilla y es de tipo "A", usarlo como sele
            if (grillaDetalle.SelectedItem is Nodo seleccionado && seleccionado.Tipo == "A")
                {
                sele = seleccionado;
                }

            if (sele == null || (tipo != "A" && sele.Tipo != "T" && sele.Tipo != "A"))
                {
                MessageBox.Show("Debe seleccionar una tarea o auxiliar para agregar el nuevo nodo.");
                return;
                }

            (Nodo nuevoNodo, string mensaje) = Presup.agregaNodo(tipo, sele);
            Presup.recalculo(Presup.Arbol, true, 0, true);
            }
        }
    }
