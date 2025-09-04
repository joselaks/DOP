using Bibioteca.Clases;
using DOP.Presupuestos.Clases;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.UI.Xaml.TreeGrid.Helpers;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace DOP.Presupuestos.Controles
{
    /// <summary>
    /// Lógica de interacción para UcPlanilla.xaml
    /// </summary>
    public partial class UcPlanilla : UserControl
    {
        public Presupuesto Objeto;
        public UcDosaje Dosaje;
        Nodo anterior = new Nodo();
        private object _originalValue;
        private Stack<Cambios> undoStack;
        private Stack<Cambios> redoStack;
        private CultureInfo cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };




        public UcPlanilla(Presupuesto objeto, UcDosaje dosaje)
        {
            InitializeComponent();
            Dosaje = dosaje;
            Objeto = objeto;
            this.grillaArbol.ItemsSource = Objeto.Arbol;
            this.grillaArbol.ChildPropertyName = "Inferiores";
            //Este evento se ejecuta cada vez que haya un recalculo en el arbol del presupuesto
            Objeto.RecalculoFinalizado += Presupuesto_RecalculoFinalizado;
            this.grillaArbol.Loaded += GrillaArbol_Loaded;
            this.grillaArbol.RowDragDropController.Drop += RowDragDropController_Drop;
            this.grillaArbol.RowDragDropController.DragStart += RowDragDropController_DragStart;
            this.grillaArbol.QueryCoveredRange += OnQueryCoveredRange;
        }

        private void Presupuesto_RecalculoFinalizado(object sender, EventArgs e)
        {

            //Objeto.sinCero();

            decimal totGeneral1 = Objeto.Arbol.Sum(i => i.Importe1);
            decimal totMateriales1 = Objeto.Arbol.Sum(i => i.Materiales1);
            decimal totManoDeObra1 = Objeto.Arbol.Sum(i => i.ManodeObra1);
            decimal totEquipos1 = Objeto.Arbol.Sum(i => i.Equipos1);
            decimal totSubcontratos1 = Objeto.Arbol.Sum(i => i.Subcontratos1);
            decimal totOtros1 = Objeto.Arbol.Sum(i => i.Otros1);

            // Asignar el valor explícitamente al HeaderText
            var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
            colImporte1.HeaderText = $"{totGeneral1.ToString("N2", cultura)}";
            colMateriales1.HeaderText = $"{totMateriales1.ToString("N2", cultura)}";
            colManoDeObra1.HeaderText = $"{totManoDeObra1.ToString("N2", cultura)}";
            colEquipos1.HeaderText = $"{totEquipos1.ToString("N2", cultura)}";
            colSubcontratos1.HeaderText = $"{totSubcontratos1.ToString("N2", cultura)}";
            colOtros1.HeaderText = $"{totOtros1.ToString("N2", cultura)}";

        }

        private UserControl GetParentUserControl(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is UserControl))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as UserControl;
        }

        private void RowDragDropController_DragStart(object? sender, TreeGridRowDragStartEventArgs e)
        {
            DragDropContext.DragSourceUserControl = GetParentUserControl(this.grillaArbol);
        }

        private void RowDragDropController_Drop(object? sender, TreeGridRowDropEventArgs e)
        {
            e.Handled = true;
            Nodo nodoMovido = null;
            Nodo nodoReceptor = null;
            bool esDragDePlanilla = DragDropContext.DragSourceUserControl is UcPlanilla;
            bool esDragDeMaestro = DragDropContext.DragSourceUserControl is UcMaestro;

            if (e.DraggingNodes != null && e.DraggingNodes.Count > 0)
            {
                if (esDragDePlanilla)
                {
                    nodoMovido = e.DraggingNodes[0].Item as Nodo;
                }
                else if (esDragDeMaestro)
                {
                    var nodoOriginal = e.DraggingNodes[0].Item as Nodo;
                    nodoMovido = Objeto.clonar(nodoOriginal, true);
                }
            }

            if (e.TargetNode != null)
            {
                nodoReceptor = e.TargetNode.Item as Nodo;
                var tipoReceptor = nodoReceptor?.Tipo;
                var tipoMovido = nodoMovido?.Tipo;

                switch ((tipoReceptor, tipoMovido))
                {
                    case ("T", "R"):
                        MessageBox.Show("No se puede mover un rubro dentro de una tarea");
                        break;

                    case ("R", "R"):
                        if (esDragDePlanilla)
                        {
                            var nodoPadreOriginal = Objeto.FindParentNode(Objeto.Arbol, nodoMovido, null);
                            if (nodoPadreOriginal != null)
                                nodoPadreOriginal.Inferiores.Remove(nodoMovido);
                            else
                                Objeto.Arbol.Remove(nodoMovido);
                        }
                        var padreDestinoRR = Objeto.FindParentNode(Objeto.Arbol, nodoReceptor, null);
                        var coleccionDestinoRR = padreDestinoRR != null ? padreDestinoRR.Inferiores : Objeto.Arbol;
                        int idxRR = coleccionDestinoRR.IndexOf(nodoReceptor);
                        int insertIdxRR = e.DropPosition == Syncfusion.UI.Xaml.TreeGrid.DropPosition.DropAbove ? idxRR : idxRR + 1;
                        coleccionDestinoRR.Insert(insertIdxRR, nodoMovido);
                        break;

                    case ("R", "T"):
                        if (esDragDePlanilla)
                        {
                            var nodoPadreOriginal = Objeto.FindParentNode(Objeto.Arbol, nodoMovido, null);
                            if (nodoPadreOriginal != null)
                                nodoPadreOriginal.Inferiores.Remove(nodoMovido);
                            else
                                Objeto.Arbol.Remove(nodoMovido);
                        }
                        nodoReceptor.Inferiores.Add(nodoMovido);
                        break;

                    case ("T", "T"):
                        if (esDragDePlanilla)
                        {
                            var nodoPadreOriginal = Objeto.FindParentNode(Objeto.Arbol, nodoMovido, null);
                            if (nodoPadreOriginal != null)
                                nodoPadreOriginal.Inferiores.Remove(nodoMovido);
                            else
                                Objeto.Arbol.Remove(nodoMovido);
                        }
                        var padreDestinoTT = Objeto.FindParentNode(Objeto.Arbol, nodoReceptor, null);
                        var coleccionDestinoTT = padreDestinoTT != null ? padreDestinoTT.Inferiores : Objeto.Arbol;
                        int idxTT = coleccionDestinoTT.IndexOf(nodoReceptor);
                        int insertIdxTT = e.DropPosition == Syncfusion.UI.Xaml.TreeGrid.DropPosition.DropAbove ? idxTT : idxTT + 1;
                        coleccionDestinoTT.Insert(insertIdxTT, nodoMovido);
                        break;

                    default:
                        if (nodoMovido != null)
                        {
                            if (esDragDePlanilla)
                            {
                                var nodoPadreOriginal = Objeto.FindParentNode(Objeto.Arbol, nodoMovido, null);
                                if (nodoPadreOriginal != null)
                                    nodoPadreOriginal.Inferiores.Remove(nodoMovido);
                                else
                                    Objeto.Arbol.Remove(nodoMovido);
                            }
                            Objeto.Arbol.Add(nodoMovido);
                        }
                        break;
                }
            }
            else
            {
                // Si no hay nodo receptor, se agrega a la raíz
                if (nodoMovido != null)
                {
                    if (esDragDePlanilla)
                    {
                        var nodoPadreOriginal = Objeto.FindParentNode(Objeto.Arbol, nodoMovido, null);
                        if (nodoPadreOriginal != null)
                            nodoPadreOriginal.Inferiores.Remove(nodoMovido);
                        else
                            Objeto.Arbol.Remove(nodoMovido);
                    }
                    Objeto.Arbol.Add(nodoMovido);
                }
            }

            DragDropContext.DragSourceUserControl = null;
        }


        // Esto me lo pasó Syncfusion para solucionar lo de las celdas de la grilla en caso de Rubro
        private void OnQueryCoveredRange(object? sender, TreeGridQueryCoveredRangeEventArgs e)
        {
            var record = e.Record as Nodo;
            if (record != null && record.Tipo == "R")
            {
                //Customize here based on your requirement
                e.Range = new TreeGridCoveredCellInfo(2, 5, e.RowColumnIndex.RowIndex);
                e.Handled = true;
            }
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is DropDownMenuItem boton)
            {
                switch (boton.Name)
                {
                    case "Rubro":
                        // Lógica para agregar Rubro
                        var (nuevoNodo, mensaje) = Objeto.agregaNodo("R", null);
                        break;
                    case "Tarea":
                        // Lógica para agregar Tarea
                        if (this.grillaArbol.SelectedItem == null)
                        {
                            MessageBox.Show("debe seleccionar un rubro para la tarea");
                        }
                        else
                        {
                            Bibioteca.Clases.Nodo sele = this.grillaArbol.SelectedItem as Bibioteca.Clases.Nodo; //obtine contenido
                            if (sele.Tipo != "R")
                            {
                                MessageBox.Show("debe seleccionar un rubro para la tarea");
                            }
                            else
                            {
                                Objeto.agregaNodo("T", sele);


                                // Obtener la vista del árbol
                                var view = grillaArbol.View;
                                if (view != null && sele != null)
                                {
                                    var node = FindNodeByData(view.Nodes, sele);
                                    if (node != null && !node.IsExpanded)
                                    {
                                        grillaArbol.ExpandNode(node);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        // Otro caso
                        break;
                }
            }
        }

        private TreeNode? FindNodeByData(IEnumerable<TreeNode> nodes, object data)
        {
            foreach (var node in nodes)
            {
                if (node.Item == data) // Cambia aquí 'Item' si es necesario
                    return node;
                var found = FindNodeByData(node.ChildNodes, data);
                if (found != null)
                    return found;
            }
            return null;
        }


        // Le hemos agregado un evento Loaded al TreeGrid para aplicar el filtro una vez que la vista se haya cargado.
        private void GrillaArbol_Loaded(object sender, RoutedEventArgs e)
        {
            int inicio = 0;

            colImporte1.HeaderText = $"{inicio.ToString("N2", cultura)}";
            colMateriales1.HeaderText = $"{inicio.ToString("N2", cultura)}";
            colManoDeObra1.HeaderText = $"{inicio.ToString("N2", cultura)}";
            colEquipos1.HeaderText = $"{inicio.ToString("N2", cultura)}";
            colSubcontratos1.HeaderText = $"{inicio.ToString("N2", cultura)}";
            colOtros1.HeaderText = $"{inicio.ToString("N2", cultura)}";

            if (this.grillaArbol.View != null)
            {
                this.grillaArbol.View.Filter = FiltrarPorTipo;
                this.grillaArbol.View.Refresh();
                ExpandeRubro();
            }

            //
            Objeto.RecalculoCompleto();
        }


        // Método para filtrar los nodos que se mostrarán en el TreeGrid.
        public bool FiltrarPorTipo(object item)
        {
            if (item is Nodo nodo)
            {
                return nodo.Tipo == "R" || nodo.Tipo == "T";
            }
            return false;
        }

        public void ExpandeRubro()
        {
            if (grillaArbol.View == null)
                return;

            foreach (var node in grillaArbol.View.Nodes)
            {
                ExpandeRubroRecursivo(node);
            }
        }

        private void ExpandeRubroRecursivo(Syncfusion.UI.Xaml.TreeGrid.TreeNode node)
        {
            if (node == null)
                return;

            if (node.Item is Nodo nodo && nodo.Tipo == "R")
            {
                grillaArbol.ExpandNode(node);
            }

            if (node.HasChildNodes)
            {
                foreach (var child in node.ChildNodes)
                {
                    ExpandeRubroRecursivo(child);
                }
            }
        }


        //Cada vez que se edita la grilla, se clona el objeto original antes de editarlo.
        private void grillaArbol_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
        {
            var column = grillaArbol.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var record = grillaArbol.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
            // Clonar el objeto record y asignarlo a anterior
            anterior = Objeto.clonar(record, true);

            if (column == "ID")
            {
                _originalValue = record.ID;
            }

        }

        private void grillaArbol_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            var column = grillaArbol.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var editado = grillaArbol.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
            edicion(editado, column);
            var undoRegistro = new Cambios
            {
                TipoCambio = "Tipeo",
                antesCambio = anterior,
                despuesCambio = Objeto.clonar(editado, true),
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
            Objeto.RecalculoCompleto();
        }

        private void grillaArbol_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedItems = grillaArbol.SelectedItems;
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

        private void grillaArbol_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (grillaArbol.SelectedItem is Nodo nodoSeleccionado && Dosaje != null && nodoSeleccionado.Tipo == "T")
            {
                Dosaje.MostrarInferiores(nodoSeleccionado);
            }

        }

        private void expandir_Click(object sender, RoutedEventArgs e)
        {
            if (grillaArbol.View != null)
            {
                var boton = sender as DropDownMenuItem;
                if (boton != null)
                {
                    if (boton.Name == "Expandir")
                    {
                        ExpandeRubro();
                    }
                    else if (boton.Name == "Contraer")
                    {
                        grillaArbol.CollapseAllNodes();
                    }
                }
            }
        }


        private void Renumerar_Click(object sender, RoutedEventArgs e)
        {
            Objeto.NumeraItems(Objeto.Arbol, "");
        }

        private void chkArbol_Checked(object sender, RoutedEventArgs e)
        {
            if (grillaArbol.View != null)
            {
                // Quitar el filtro
                grillaArbol.View.Filter = null;
                grillaArbol.View.Refresh();
                ExpandeRubro();
            }
        }


        private void chkArbol_Unchecked(object sender, RoutedEventArgs e)
        {

            if (grillaArbol.View != null)
            {
                // Activar el filtro
                grillaArbol.View.Filter = FiltrarPorTipo;
                grillaArbol.View.Refresh();
                ExpandeRubro();
            }
        }



        }
        public class Cambios
        {
            public string TipoCambio { get; set; }
            public Nodo antesCambio { get; set; }
            public Nodo despuesCambio { get; set; }
            public string PropiedadCambiada { get; set; }
            public object OldValue { get; set; }
            public object NewValue { get; set; }
            public Nodo NodoPadre { get; set; }
            public Nodo NodoMovido { get; set; }
            public Nodo NodoPadreNuevo { get; set; }
            public Nodo NodoPadreAnterior { get; set; }
            public int Posicion { get; set; }
        }
    }

