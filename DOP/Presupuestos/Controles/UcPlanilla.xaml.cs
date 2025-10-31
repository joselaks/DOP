using Bibioteca.Clases;
using Biblioteca;
using DOP.Presupuestos.Clases;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
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
        private CultureInfo cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
        private HashSet<string> nodosExpandidosRT = new HashSet<string>();

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
            decimal totGeneral2 = Objeto.Arbol.Sum(i => i.Importe2);
            decimal totGeneral3 = Objeto.Arbol.Sum(i => i.Importe3);
            decimal totMateriales1 = Objeto.Arbol.Sum(i => i.Materiales1);
            decimal totManoDeObra1 = Objeto.Arbol.Sum(i => i.ManodeObra1);
            decimal totEquipos1 = Objeto.Arbol.Sum(i => i.Equipos1);
            decimal totSubcontratos1 = Objeto.Arbol.Sum(i => i.Subcontratos1);
            decimal totOtros1 = Objeto.Arbol.Sum(i => i.Otros1);

            // Asignar el valor explícitamente al HeaderText
            var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
            colImporte1.HeaderText = $"{totGeneral1.ToString("N2", cultura)}";
            colImporte2.HeaderText = $"{totGeneral2.ToString("N2", cultura)}";
            colImporte2.HeaderText = $"{totGeneral2.ToString("N2", cultura)}";
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
            Nodo nodoPadreOriginal = null;
            int itemIndex = -1;
            bool esDragDePlanilla = DragDropContext.DragSourceUserControl is UcPlanilla;
            bool esDragDeMaestro = DragDropContext.DragSourceUserControl is UcMaestro;

            // 1. Determinar el nodo a mover/clonar y el padre original si aplica
            if (e.DraggingNodes != null && e.DraggingNodes.Count > 0)
                {
                if (esDragDePlanilla)
                    {
                    nodoMovido = e.DraggingNodes[0].Item as Nodo;
                    nodoPadreOriginal = Objeto.FindParentNode(Objeto.Arbol, nodoMovido, null);
                    itemIndex = nodoPadreOriginal != null
                        ? nodoPadreOriginal.Inferiores.IndexOf(nodoMovido)
                        : Objeto.Arbol.IndexOf(nodoMovido);
                    }
                else if (esDragDeMaestro)
                    {
                    var nodoOriginal = e.DraggingNodes[0].Item as Nodo;
                    nodoMovido = Objeto.clonar(nodoOriginal, true);
                    }
                }

            // 2. Determinar el nodo receptor y la colección destino
            ObservableCollection<Nodo> coleccionDestino;
            Nodo padreDestino;
            int posicionDestino;

            if (e.TargetNode != null)
                {
                nodoReceptor = e.TargetNode.Item as Nodo;

                // --- AJUSTE: Si se suelta una tarea sobre un rubro, agregar como hijo ---
                if (nodoMovido != null && nodoMovido.Tipo == "T" && nodoReceptor != null && nodoReceptor.Tipo == "R")
                    {
                    if (nodoReceptor.Inferiores == null)
                        nodoReceptor.Inferiores = new ObservableCollection<Nodo>();
                    coleccionDestino = nodoReceptor.Inferiores;
                    padreDestino = nodoReceptor;
                    posicionDestino = coleccionDestino.Count; // al final de los hijos
                    }
                else
                    {
                    padreDestino = Objeto.FindParentNode(Objeto.Arbol, nodoReceptor, null);
                    coleccionDestino = padreDestino != null ? padreDestino.Inferiores : Objeto.Arbol;
                    int idx = coleccionDestino.IndexOf(nodoReceptor);
                    posicionDestino = (e.DropPosition == Syncfusion.UI.Xaml.TreeGrid.DropPosition.DropAbove) ? idx : idx + 1;
                    }
                }
            else
                {
                padreDestino = null;
                coleccionDestino = Objeto.Arbol;
                posicionDestino = coleccionDestino.Count;
                }

            // 3. Validar reglas de negocio (ejemplo: no mover rubro dentro de tarea)
            if (esDragDePlanilla && nodoReceptor != null && nodoReceptor.Tipo == "T" && nodoMovido.Tipo == "R")
                {
                MessageBox.Show("No se puede mover un rubro dentro de una tarea");
                return;
                }

            // 4. Eliminar el nodo de su colección original si es drag interno
            if (esDragDePlanilla)
                {
                if (nodoPadreOriginal != null)
                    nodoPadreOriginal.Inferiores.Remove(nodoMovido);
                else
                    Objeto.Arbol.Remove(nodoMovido);
                }

            // 5. Insertar el nodo en la colección destino
            coleccionDestino.Insert(posicionDestino, nodoMovido);

            // 6. Registrar undo/redo
            if (esDragDePlanilla)
                {
                var undoRegistro = new Cambios
                    {
                    TipoCambio = "Mover",
                    NodoMovido = nodoMovido,
                    NodoPadreAnterior = nodoPadreOriginal,
                    NodoPadreNuevo = padreDestino,
                    Posicion = posicionDestino,
                    PosicionOriginal = itemIndex // <-- aquí guardas la posición original
                    };
                Objeto.undoStack.Push(undoRegistro);
                }
            else if (esDragDeMaestro)
                {
                var undoRegistro = new Cambios
                    {
                    TipoCambio = "Nuevo",
                    despuesCambio = nodoMovido,
                    NodoPadre = padreDestino,
                    Posicion = posicionDestino
                    };
                Objeto.undoStack.Push(undoRegistro);
                }

            Objeto.redoStack.Clear();
            DragDropContext.DragSourceUserControl = null;
            }


        // Esto me lo pasó Syncfusion para solucionar lo de las celdas de la grilla en caso de Rubro
        private void OnQueryCoveredRange(object? sender, TreeGridQueryCoveredRangeEventArgs e)
            {
            var record = e.Record as Nodo;
            if (record != null && record.Tipo == "R")
                {
                //Customize here based on your requirement
                e.Range = new TreeGridCoveredCellInfo(2, 6, e.RowColumnIndex.RowIndex);
                e.Handled = true;
                }
            }

        private void Agregar_Click(object sender, RoutedEventArgs e)
            {
            string? accion = null;

            // Detectar si el evento viene de un DropDownMenuItem (Ribbon) o MenuItem (ContextMenu)
            if (sender is DropDownMenuItem dropDown)
                {
                accion = dropDown.Name;
                }
            else if (sender is MenuItem menuItem)
                {
                // Puedes usar Name si lo defines, o Header si solo usas el texto
                accion = menuItem.Name;
                if (string.IsNullOrEmpty(accion) && menuItem.Header is string header)
                    accion = header;
                }
            switch (accion)
                {
                case "Rubro":
                case "menuAgregarRubro":
                case "Agregar Rubro":
                    // Agregar Rubro (a la raíz)
                    var (nuevoNodo, mensaje) = Objeto.agregaNodo("R", null);
                    var undoRegistro = new Cambios
                        {
                        TipoCambio = "Nuevo",
                        despuesCambio = nuevoNodo,
                        NodoPadre = null, // raíz
                        Posicion = Objeto.Arbol.IndexOf(nuevoNodo)
                        };

                    Objeto.undoStack.Push(undoRegistro);
                    Objeto.redoStack.Clear();

                    // Seleccionar el nuevo nodo en la grilla
                    this.grillaArbol.SelectedItem = nuevoNodo;
                    break;
                case "Tarea":
                case "menuAgregarTarea":
                case "Agregar Tarea":
                    // Agregar Tarea (a un Rubro)
                    if (this.grillaArbol.SelectedItem is Nodo sele && sele.Tipo == "R")
                        {
                        var (nuevoNodo2, mensaje2) = Objeto.agregaNodo("T", sele);
                        var undoRegistro2 = new Cambios
                            {
                            TipoCambio = "Nuevo",
                            despuesCambio = nuevoNodo2,
                            NodoPadre = sele, // el Rubro seleccionado es el padre
                            Posicion = sele.Inferiores.IndexOf(nuevoNodo2)
                            };
                        Objeto.undoStack.Push(undoRegistro2);
                        Objeto.redoStack.Clear();

                        // Expandir el nodo Rubro si no está expandido
                        var view = grillaArbol.View;
                        if (view != null)
                            {
                            var node = FindNodeByData(view.Nodes, sele);
                            if (node != null && !node.IsExpanded)
                                {
                                grillaArbol.ExpandNode(node);
                                }
                            }
                        }
                    else
                        {
                        MessageBox.Show("Debe seleccionar un rubro para agregar una tarea.");
                        }
                    break;
                default:
                    // Otro caso o no reconocido
                    break;
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

        private void grillaArbol_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
            {
            if (Dosaje == null)
                return;

            if (grillaArbol.SelectedItem is Nodo nodoSeleccionado && nodoSeleccionado.Tipo == "T")
                {
                // Si el ItemsSource es Objeto.Arbol, pasamos el nodo directamente
                if (ReferenceEquals(grillaArbol.ItemsSource, Objeto.Arbol))
                    {
                    Dosaje.MostrarInferiores(nodoSeleccionado);
                    }
                // Si el ItemsSource es Objeto.Tareas, pasamos el ID
                else if (ReferenceEquals(grillaArbol.ItemsSource, Objeto.Tareas))
                    {
                    Dosaje.MostrarInferiores(nodoSeleccionado.ID);
                    }
                }
            else
                {
                Dosaje.MostrarInferiores("0");
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
                        if (grillaArbol.View.Filter == null)
                            {
                            grillaArbol.View.Filter = FiltrarPorTipo;
                            grillaArbol.View.Refresh();
                            }
                        ExpandeRubro();
                        }
                    else if (boton.Name == "Contraer")
                        {
                        if (grillaArbol.View.Filter == null)
                            {
                            grillaArbol.View.Filter = FiltrarPorTipo;
                            grillaArbol.View.Refresh();
                            }
                        grillaArbol.CollapseAllNodes();
                        }
                    else if (boton.Name == "ExpandirTodo")
                        {
                        if (grillaArbol.View != null)
                            {
                            // Quitar el filtro
                            grillaArbol.View.Filter = null;
                            grillaArbol.View.Refresh();
                            ExpandeRubro();
                            }
                        }
                    }
                }
            }



        private void Renumerar_Click(object sender, RoutedEventArgs e)
            {
            Objeto.NumeraItems(Objeto.Arbol, "");
            }

        public void CalcularIncidencia()
            {

            }


        private void Vistas_Click(object sender, RoutedEventArgs e)
            {
            var vista = sender as DropDownMenuItem;
            if (vista == null) return;

            // Guardar filtro actual
            var filtroActual = grillaArbol.View?.Filter;

            // Guardar nodos expandidos solo si estamos saliendo de RT
            if (grillaArbol.View != null && grillaArbol.ChildPropertyName == "Inferiores")
                {
                GuardarNodosExpandidosRT();
                }

            switch (vista.Name)
                {
                case "RT":
                    this.grillaArbol.AllowSorting = false;
                    this.grillaArbol.ChildPropertyName = "Inferiores";
                    this.grillaArbol.ItemsSource = Objeto.Arbol;
                    grillaArbol.SortColumnDescriptions.Clear();
                    break;
                case "R":
                    this.grillaArbol.AllowSorting = true;
                    this.grillaArbol.ChildPropertyName = null;
                    this.grillaArbol.ItemsSource = Objeto.Rubros;
                    break;
                case "T":
                    this.grillaArbol.AllowSorting = true;
                    this.grillaArbol.ChildPropertyName = null;
                    this.grillaArbol.ItemsSource = Objeto.Tareas;
                    break;
                }

            // Restaurar filtro y limpiar orden en R y T
            if (grillaArbol.View != null)
                {
                grillaArbol.View.Filter = filtroActual;
                grillaArbol.View.Refresh();

                // Limpiar orden anterior para permitir reordenar en R y T
                if (vista.Name == "R" || vista.Name == "T")
                    {
                    grillaArbol.SortColumnDescriptions.Clear();
                    }

                // Restaurar nodos expandidos solo para RT
                if (vista.Name == "RT")
                    {
                    RestaurarNodosExpandidosRT();
                    }
                }
            }


        private void GuardarNodosExpandidosRT()
            {
            nodosExpandidosRT.Clear();
            if (grillaArbol.View != null)
                {
                foreach (var node in grillaArbol.View.Nodes)
                    {
                    GuardarNodosExpandidosRecursivo(node, nodosExpandidosRT);
                    }
                }
            }

        private void RestaurarNodosExpandidosRT()
            {
            if (grillaArbol.View != null && nodosExpandidosRT.Count > 0)
                {
                foreach (var node in grillaArbol.View.Nodes)
                    {
                    RestaurarNodosExpandidosRecursivo(node, nodosExpandidosRT);
                    }
                }
            }

        private void GuardarNodosExpandidosRecursivo(TreeNode node, HashSet<string> expandidos)
            {
            if (node.IsExpanded && node.Item is Nodo n && !string.IsNullOrEmpty(n.ID))
                expandidos.Add(n.ID);

            foreach (var child in node.ChildNodes)
                GuardarNodosExpandidosRecursivo(child, expandidos);
            }

        private void RestaurarNodosExpandidosRecursivo(TreeNode node, HashSet<string> expandidos)
            {
            if (node.Item is Nodo n && expandidos.Contains(n.ID))
                grillaArbol.ExpandNode(node);

            foreach (var child in node.ChildNodes)
                RestaurarNodosExpandidosRecursivo(child, expandidos);
            }

        public void SeleccionarNodoEnArbol(Nodo nodo)
            {
            this.grillaArbol.SelectedItem = nodo;
            // Obtiene el índice de la fila correspondiente al nodo
            var rowIndex = grillaArbol.ResolveToRowIndex(nodo);
            if (rowIndex > -1)
                {
                var rowColumnIndex = new RowColumnIndex(rowIndex, 0); // Columna 0 por defecto
                grillaArbol.ScrollInView(rowColumnIndex);
                }


            }
        }

    }

