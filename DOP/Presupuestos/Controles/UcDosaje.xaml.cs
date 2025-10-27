using Bibioteca.Clases;
using Biblioteca;
using DOP.Presupuestos.Clases;
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
        Nodo anterior = new Nodo();
        public Presupuesto Objeto;
        private object _originalValue;
        private CultureInfo cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };



        public UcDosaje(Presupuesto presup)
            {
            InitializeComponent();
            Objeto = presup;
            this.grillaDetalle.ChildPropertyName = "Inferiores";
            //Este evento se ejecuta cada vez que haya un recalculo en el arbol del presupuesto
            Objeto.RecalculoFinalizado += Presupuesto_RecalculoFinalizado;
            this.grillaDetalle.RowDragDropController.Drop += RowDragDropController_Drop;
            this.grillaDetalle.RowDragDropController.DragStart += RowDragDropController_DragStart;
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
            DragDropContext.DragSourceUserControl = GetParentUserControl(this.grillaDetalle);

            }

        private void RowDragDropController_Drop(object? sender, TreeGridRowDropEventArgs e)
            {
            e.Handled = true;
            Nodo nodoMovido = null;
            Nodo nodoReceptor = null;
            Nodo nodoPadreOriginal = null;
            int itemIndex = -1;
            bool esDragDeMaestro = DragDropContext.DragSourceUserControl is UcMaestro;
            bool esDragDeDosaje = DragDropContext.DragSourceUserControl is UcDosaje;
            bool esDragDeListado = DragDropContext.DragSourceUserControl is UcListado;

            // Solo permitir tipos válidos
            var tiposPermitidos = new[] { "M", "D", "E", "S", "O", "A" };

            if (e.DraggingNodes != null && e.DraggingNodes.Count > 0)
                {

                if (e.DraggingNodes != null && e.DraggingNodes.Count > 0)
                    {
                    if (esDragDeDosaje)
                        {
                        nodoMovido = e.DraggingNodes[0].Item as Nodo;
                        nodoPadreOriginal = Objeto.FindParentNode(Objeto.Arbol, nodoMovido, null);
                        if (nodoPadreOriginal != null)
                            {
                            itemIndex = nodoPadreOriginal.Inferiores.IndexOf(nodoMovido);
                            }
                        else
                            {
                            itemIndex = Objeto.Arbol.IndexOf(nodoMovido);
                            }
                        }
                    else if (esDragDeMaestro || esDragDeListado)
                        {
                        var nodoOriginal = e.DraggingNodes[0].Item as Nodo;
                        nodoMovido = Objeto.clonar(nodoOriginal, true);
                        }

                    }

                }

            // Validar tipo permitido
            if (nodoMovido == null || !tiposPermitidos.Contains(nodoMovido.Tipo))
                {
                MessageBox.Show("Solo se pueden arrastrar nodos de tipo Material, Mano de obra, Equipo, Subcontrato, Otro o Auxiliar.", "Tipo no permitido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
                }

            if (e.TargetNode != null)
                {
                nodoReceptor = e.TargetNode.Item as Nodo;
                var tipoReceptor = nodoReceptor?.Tipo;

                // Si el receptor es "A", agregar como hijo
                if (tipoReceptor == "A")
                    {
                    nodoReceptor.Inferiores.Add(nodoMovido);

                    }
                else
                    {
                    // Si el receptor NO es "A", agregar a la raíz
                    NodoAnalizado.Inferiores.Add(nodoMovido);
                    }
                }
            else
                {
                // Si no hay nodo receptor, agregar a la raíz
                NodoAnalizado.Inferiores.Add(nodoMovido);
                }
            if (nodoPadreOriginal != null)
                nodoPadreOriginal.Inferiores.Remove(nodoMovido);

            // Crear un registro de cambio y agregarlo a undoStack
            if (esDragDeDosaje)
                {
                // Movimiento dentro de la misma planilla de dosaje
                // Determinar el padre de destino y la posición real de inserción
                Nodo padreDestino = null;
                int posicionDestino = -1;

                if (e.TargetNode != null)
                    {
                    if (nodoReceptor != null && nodoReceptor.Tipo == "A")
                        {
                        padreDestino = nodoReceptor;
                        posicionDestino = nodoReceptor.Inferiores.IndexOf(nodoMovido);
                        }
                    else
                        {
                        padreDestino = NodoAnalizado;
                        posicionDestino = NodoAnalizado.Inferiores.IndexOf(nodoMovido);
                        }
                    }
                else
                    {
                    padreDestino = NodoAnalizado;
                    posicionDestino = NodoAnalizado.Inferiores.IndexOf(nodoMovido);
                    }

                var undoRegistro = new Cambios
                    {
                    TipoCambio = "Mover",
                    NodoMovido = nodoMovido,
                    NodoPadreAnterior = nodoPadreOriginal,
                    NodoPadreNuevo = padreDestino,
                    Posicion = posicionDestino
                    };
                Objeto.undoStack.Push(undoRegistro);
                }
            else if (esDragDeMaestro || esDragDeListado)
                {
                // Alta desde maestro o listado
                Nodo padreDestino = null;
                int posicionDestino = -1;

                if (e.TargetNode != null && nodoReceptor != null && nodoReceptor.Tipo == "A")
                    {
                    padreDestino = nodoReceptor;
                    posicionDestino = nodoReceptor.Inferiores.IndexOf(nodoMovido);
                    }
                else
                    {
                    padreDestino = NodoAnalizado;
                    posicionDestino = NodoAnalizado.Inferiores.IndexOf(nodoMovido);
                    }

                var undoRegistro = new Cambios
                    {
                    TipoCambio = "Nuevo",
                    despuesCambio = nodoMovido,
                    NodoPadre = padreDestino,
                    Posicion = posicionDestino
                    };
                Objeto.undoStack.Push(undoRegistro);
                }
            Objeto.redoStack.Clear(); // Limpiar la pila de rehacer cuando se realiza una nueva operación




            DragDropContext.DragSourceUserControl = null;
            Objeto.RecalculoCompleto();
            }

        private void Presupuesto_RecalculoFinalizado(object sender, EventArgs e)
            {
            if (NodoAnalizado == null)
                return;

            // Asignar el valor explícitamente al HeaderText
            colImporte1.HeaderText = $"{NodoAnalizado.PU1.ToString("N2", cultura)}";
            colImporte2.HeaderText = $"{NodoAnalizado.PU2.ToString("N2", cultura)}";
            }


        public void MostrarInferiores(Nodo analizado)
            {
            this.grillaBase.Visibility = Visibility.Visible;

            NodoAnalizado = analizado;
            grillaDetalle.ItemsSource = NodoAnalizado.Inferiores;
            nombreTarea.Text = NodoAnalizado.Descripcion;
            colImporte1.HeaderText = $"{NodoAnalizado.PU1.ToString("N2", cultura)}";

            }

        // Sobrecarga para buscar por ID
        public void MostrarInferiores(string id)
            {
            var nodo = BuscarNodoPorId(Objeto.Arbol, id);
            if (nodo != null)
                {
                MostrarInferiores(nodo);
                this.grillaBase.Visibility = Visibility.Visible;
                }
            else
                {
                this.grillaBase.Visibility = Visibility.Collapsed;
                }
            }

        // Método auxiliar para búsqueda recursiva
        private Nodo BuscarNodoPorId(IEnumerable<Nodo> nodos, string id)
            {
            foreach (var nodo in nodos)
                {
                if (nodo.ID == id)
                    return nodo;
                if (nodo.Inferiores != null && nodo.Inferiores.Count > 0)
                    {
                    var encontrado = BuscarNodoPorId(nodo.Inferiores, id);
                    if (encontrado != null)
                        return encontrado;
                    }
                }
            return null;
            }


        private void grillaDetalle_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
            {
            var column = grillaDetalle.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var record = grillaDetalle.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
            // Clonar el objeto record y asignarlo a anterior
            anterior = Objeto.clonar(record, true);

            if (column == "ID")
                {
                _originalValue = record.ID;
                }
            }

        private void grillaDetalle_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
            {
            var column = grillaDetalle.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var editado = grillaDetalle.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
            Objeto.edicion(editado, column, _originalValue != null ? _originalValue.ToString() : null);
            var undoRegistro = new Cambios
                {
                TipoCambio = "Tipeo",
                antesCambio = anterior,
                despuesCambio = Objeto.clonar(editado, true),
                PropiedadCambiada = column,
                OldValue = _originalValue,
                NewValue = editado.GetType().GetProperty(column).GetValue(editado)
                };
            Objeto.undoStack.Push(undoRegistro);
            Objeto.redoStack.Clear(); // Limpiar la pila de rehacer cuando se realiza una nueva operación


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
                    if (result.Item1)
                        {
                        var undoRegistro = new Cambios
                            {
                            TipoCambio = "Borrado",
                            despuesCambio = item,
                            NodoPadre = result.Item2,
                            Posicion = result.Item3
                            };
                        Objeto.undoStack.Push(undoRegistro);

                        }
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

            (Nodo nuevoNodo, string mensaje) = Objeto.agregaNodo(tipo, sele);
            var undoRegistro = new Cambios
                {
                TipoCambio = "Nuevo",
                despuesCambio = nuevoNodo,
                NodoPadre = sele,
                Posicion = sele.Inferiores.IndexOf(nuevoNodo)
                };

            Objeto.undoStack.Push(undoRegistro);
            Objeto.redoStack.Clear(); // Limpiar la pila de rehacer cuando se realiza una nueva operación
            Objeto.RecalculoCompleto();
            }
        }
    }
