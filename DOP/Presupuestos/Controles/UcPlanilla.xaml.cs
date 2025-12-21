using Bibioteca.Clases;
using Biblioteca;
using DOP.Presupuestos.Clases;
using Syncfusion.SfSkinManager;
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
        string PU1 = "PU1 $";
        string PU2 = "PU2 u$s";
        string PU3 = "PU cons $";
        string PU4 = "PU cons u$s";
        string Imp1 = "Importe $";
        string Imp2 = "Importe u$s";
        string Imp3 = "Importe cons $";
        string Imp4 = "Importe cons u$s";


        public UcPlanilla(Presupuesto objeto, UcDosaje dosaje)
            {
            SfSkinManager.ApplyThemeAsDefaultStyle = true;
            SfSkinManager.SetTheme(this, new Theme("FluentLight"));

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
            TitulosColumnas(PU1, PU2, PU3, PU4, Imp1, Imp2, Imp3, Imp4);
            }

        private void Presupuesto_RecalculoFinalizado(object sender, EventArgs e)
            {

            decimal totGeneral1 = Objeto.Arbol.Sum(i => i.Importe1);
            decimal totGeneral2 = Objeto.Arbol.Sum(i => i.Importe2);
            decimal totGeneral3 = Objeto.Arbol.Sum(i => i.Importe3);
            decimal totGeneral4 = Objeto.Arbol.Sum(i => i.Importe4);
            decimal totMateriales1 = Objeto.Arbol.Sum(i => i.Materiales1);
            decimal totManoDeObra1 = Objeto.Arbol.Sum(i => i.ManodeObra1);
            decimal totEquipos1 = Objeto.Arbol.Sum(i => i.Equipos1);
            decimal totSubcontratos1 = Objeto.Arbol.Sum(i => i.Subcontratos1);
            decimal totOtros1 = Objeto.Arbol.Sum(i => i.Otros1);

            // Asignar el valor explícitamente al HeaderText
            var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
            colImporte1.HeaderText = $"{totGeneral1.ToString("N2", cultura)}";
            colImporte2.HeaderText = $"{totGeneral2.ToString("N2", cultura)}";
            colImporte3.HeaderText = $"{totGeneral3.ToString("N2", cultura)}";
            colImporte4.HeaderText = $"{totGeneral4.ToString("N2", cultura)}";
            colMateriales1.HeaderText = $"{totMateriales1.ToString("N2", cultura)}";
            colManoDeObra1.HeaderText = $"{totManoDeObra1.ToString("N2", cultura)}";
            colEquipos1.HeaderText = $"{totEquipos1.ToString("N2", cultura)}";
            colSubcontratos1.HeaderText = $"{totSubcontratos1.ToString("N2", cultura)}";
            colOtros1.HeaderText = $"{totOtros1.ToString("N2", cultura)}";

            // Actualiza los StackedColumn.HeaderText en la primera fila de StackedHeaderRows
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
            try
                {
                e.Handled = true;

                // 0) Guardar estado de expansión actual (solo RT)
                GuardarNodosExpandidosRT();

                Nodo nodoMovido = null;
                Nodo nodoReceptor = null;
                Nodo nodoPadreOriginal = null;
                int itemIndex = -1;
                bool esDragDePlanilla = DragDropContext.DragSourceUserControl is UcPlanilla;
                bool esDragDeMaestro = DragDropContext.DragSourceUserControl is UcMaestro;

                // 1) Determinar nodo movido y padre original
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

                // 2) Colección origen
                var coleccionOrigen = nodoPadreOriginal != null ? nodoPadreOriginal.Inferiores : Objeto.Arbol;
                if (coleccionOrigen == null)
                    coleccionOrigen = Objeto.Arbol;

                // 3) Nodo receptor y colección destino
                ObservableCollection<Nodo> coleccionDestino;
                Nodo padreDestino;
                int posicionDestino;

                if (e.TargetNode != null)
                    {
                    nodoReceptor = e.TargetNode.Item as Nodo;

                    if (nodoMovido != null && nodoMovido.Tipo == "T" && nodoReceptor != null && nodoReceptor.Tipo == "R")
                        {
                        if (nodoReceptor.Inferiores == null)
                            nodoReceptor.Inferiores = new ObservableCollection<Nodo>();
                        coleccionDestino = nodoReceptor.Inferiores;
                        padreDestino = nodoReceptor;
                        posicionDestino = coleccionDestino.Count;
                        }
                    else
                        {
                        padreDestino = Objeto.FindParentNode(Objeto.Arbol, nodoReceptor, null);
                        coleccionDestino = padreDestino != null ? padreDestino.Inferiores : Objeto.Arbol;

                        int idx = coleccionDestino.IndexOf(nodoReceptor);
                        if (idx < 0) idx = 0;

                        posicionDestino = (e.DropPosition == Syncfusion.UI.Xaml.TreeGrid.DropPosition.DropAbove) ? idx : idx + 1;
                        }
                    }
                else
                    {
                    padreDestino = null;
                    coleccionDestino = Objeto.Arbol;
                    posicionDestino = coleccionDestino.Count;
                    }

                // 4) Regla: si mismo padre y solo un hijo, no mover
                if (ReferenceEquals(coleccionOrigen, coleccionDestino) && coleccionOrigen != null && coleccionOrigen.Count == 1)
                    return;

                // 5) Validación negocio
                if (esDragDePlanilla && nodoReceptor != null && nodoReceptor.Tipo == "T" && nodoMovido.Tipo == "R")
                    {
                    MessageBox.Show("No se puede mover un rubro dentro de una tarea");
                    return;
                    }

                // 6) Remover del origen si drag interno
                if (esDragDePlanilla)
                    {
                    if (nodoPadreOriginal != null)
                        nodoPadreOriginal.Inferiores.Remove(nodoMovido);
                    else
                        Objeto.Arbol.Remove(nodoMovido);
                    }

                // 7) Ajustar posición si origen == destino
                if (ReferenceEquals(coleccionOrigen, coleccionDestino) && itemIndex >= 0)
                    {
                    if (posicionDestino > itemIndex)
                        posicionDestino--;
                    }

                // 8) Clamp posición
                if (posicionDestino < 0) posicionDestino = 0;
                if (posicionDestino > coleccionDestino.Count) posicionDestino = coleccionDestino.Count;

                // 9) Insertar
                coleccionDestino.Insert(posicionDestino, nodoMovido);

                // 10) Undo/Redo
                if (esDragDePlanilla)
                    {
                    var undoRegistro = new Cambios
                        {
                        TipoCambio = "Mover",
                        NodoMovido = nodoMovido,
                        NodoPadreAnterior = nodoPadreOriginal,
                        NodoPadreNuevo = padreDestino,
                        Posicion = posicionDestino,
                        PosicionOriginal = itemIndex
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

                // 10.1) Recalcular rubros y totales
                Objeto.RecalculoCompleto();

                // 11) Refrescar vista y restaurar expansión
                grillaArbol.View?.Refresh();
                RestaurarNodosExpandidosRT();

                // 12) UX: asegurar expandido del padre destino (si aplica)
                if (grillaArbol.View != null)
                    {
                    var padreNode = e.TargetNode ?? grillaArbol.View.Nodes.FirstOrDefault(n => n.Item == padreDestino);
                    if (padreNode != null)
                        grillaArbol.ExpandNode(padreNode);
                    }
                }
            catch (ArgumentOutOfRangeException ex)
                {
                MessageBox.Show($"No se pudo completar el movimiento: índice fuera de rango. Detalle: {ex.Message}");
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error al mover el ítem: {ex.Message}");
                }
            finally
                {
                DragDropContext.DragSourceUserControl = null;
                }
            }


        // Esto me lo pasó Syncfusion para solucionar lo de las celdas de la grilla en caso de Rubro
        private void OnQueryCoveredRange(object? sender, TreeGridQueryCoveredRangeEventArgs e)
            {
            var record = e.Record as Nodo;
            if (record != null && record.Tipo == "R")
                {
                //Customize here based on your requirement
                e.Range = new TreeGridCoveredCellInfo(2, 8, e.RowColumnIndex.RowIndex);
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
                // Puedes usar Name si lodefines, o Header si solo usas el texto
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
                Objeto.RecalculoCompleto();
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

            var treeGrid = sender as SfTreeGrid;
            foreach (var item in e.RemovedItems)
                {
                treeGrid.UpdateDataRow((item as TreeGridRowInfo).RowIndex);
                }
            foreach (var item in e.AddedItems)
                {
                treeGrid.UpdateDataRow((item as TreeGridRowInfo).RowIndex);
                }


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


        // Guarda los IDs de los nodos expandidos (solo aplica cuando ChildPropertyName == "Inferiores")
        private void GuardarNodosExpandidosRT()
            {
            nodosExpandidosRT.Clear();
            if (grillaArbol?.View == null || grillaArbol.ChildPropertyName != "Inferiores")
                return;

            foreach (var node in grillaArbol.View.Nodes)
                GuardarNodosExpandidosRecursivo(node, nodosExpandidosRT);
            }

        private void GuardarNodosExpandidosRecursivo(TreeNode node, HashSet<string> expandidos)
            {
            if (node == null) return;

            if (node.IsExpanded && node.Item is Nodo n && !string.IsNullOrEmpty(n.ID))
                expandidos.Add(n.ID);

            foreach (var child in node.ChildNodes)
                GuardarNodosExpandidosRecursivo(child, expandidos);
            }

        // Restaura la expansión previamente guardada. Se difiere al Dispatcher para asegurar que la vista esté lista tras el Refresh.
        private void RestaurarNodosExpandidosRT()
            {
            if (grillaArbol?.View == null || nodosExpandidosRT.Count == 0 || grillaArbol.ChildPropertyName != "Inferiores")
                return;

            Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (var node in grillaArbol.View.Nodes)
                        RestaurarNodosExpandidosRecursivo(node, nodosExpandidosRT);
                }));
            }

        private void RestaurarNodosExpandidosRecursivo(TreeNode node, HashSet<string> expandidos)
            {
            if (node == null) return;

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

        public void ActualizarColumnasMonedas(
    string headerPU1,
    string headerPU2,
    string headerPU3,
    string headerPU4,
    string stackedTituloPU,
    string stackedTituloPU1,
    string stackedTituloPU3,
    string stackedTituloPU4)
            {
            // Asegura ejecución en hilo de UI y delega al método interno
            if (!Dispatcher.CheckAccess())
                {
                Dispatcher.Invoke(() => TitulosColumnas(
                    headerPU1, headerPU2, headerPU3, headerPU4,
                    stackedTituloPU, stackedTituloPU1, stackedTituloPU3, stackedTituloPU4));
                return;
                }

            TitulosColumnas(
                headerPU1, headerPU2, headerPU3, headerPU4,
                stackedTituloPU, stackedTituloPU1, stackedTituloPU3, stackedTituloPU4);
            }


        private void TitulosColumnas(
                                    string headerPU1,
                                    string headerPU2,
                                    string headerPU3,
                                    string headerPU4,
                                    string stackedTituloPU,
                                    string stackedTituloPU1,
                                    string stackedTituloPU3,
                                    string stackedTituloPU4)
            {
            try
                {
                if (grillaArbol == null) return;

                // Actualiza columnas PU por MappingName (robusto)
                void SetHeaderByMapping(string mapping, string text)
                    {
                    var col = grillaArbol.Columns
                        .FirstOrDefault(c => string.Equals(c?.MappingName ?? string.Empty, mapping, StringComparison.OrdinalIgnoreCase));
                    if (col != null) col.HeaderText = text;
                    }

                SetHeaderByMapping("PU1", headerPU1);
                SetHeaderByMapping("PU2", headerPU2);
                SetHeaderByMapping("PU3", headerPU3);
                SetHeaderByMapping("PU4", headerPU4);

                stackImporte1.HeaderText = stackedTituloPU;
                stackImporte2.HeaderText = stackedTituloPU1;
                stackImporte3.HeaderText = stackedTituloPU3;
                stackImporte4.HeaderText = stackedTituloPU4;


                }
            catch (Exception ex)
                {
                System.Diagnostics.Debug.WriteLine($"TitulosColumnas error: {ex}");
                }
            }

        private async void menuBuscarIA_Click(object sender, RoutedEventArgs e)
            {
            try
                {
                if (this.grillaArbol.SelectedItem is not Nodo origen)
                    {
                    MessageBox.Show("Debe seleccionar una tarea.");
                    return;
                    }

                string descripcion = origen.Descripcion?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(descripcion))
                    {
                    MessageBox.Show("La tarea no tiene descripción válida.");
                    return;
                    }

                // 1) Guardar estado de expansión actual (solo vista RT)
                GuardarNodosExpandidosRT();

                // 2) Llama al servidor IA
                var (ok, msg, items) = await DOP.Datos.DatosWeb.AnalizarCostoIAAsync(descripcion);
                if (!ok)
                    {
                    MessageBox.Show(msg);
                    return;
                    }
                if (items == null || items.Count == 0)
                    {
                    MessageBox.Show("La IA no devolvió ítems para esta descripción.");
                    return;
                    }

                // 3) Reemplaza los inferiores de la tarea por los sugeridos
                if (origen.Inferiores == null)
                    origen.Inferiores = new ObservableCollection<Nodo>();
                else
                    origen.Inferiores.Clear();

                foreach (var it in items)
                    {
                    var n = new Nodo
                        {
                        ID = string.IsNullOrWhiteSpace(it.Id) ? Guid.NewGuid().ToString("N") : it.Id,
                        Descripcion = it.Descripcion,
                        Tipo = it.Tipo,              // M/D/E/S/O
                        Unidad = it.Unidad,
                        Cantidad = it.Cantidad,
                        PU1 = it.PU1,
                        PU2 = it.PU2,
                        Sup = false,
                        Inferiores = new ObservableCollection<Nodo>()
                        };
                    origen.Inferiores.Add(n);
                    }

                // 4) Recalcula y refresca
                Objeto.RecalculoCompleto();
                grillaArbol.View?.Refresh();

                // 5) Restaurar expansión global
                RestaurarNodosExpandidosRT();

                // 6) Asegurar que la tarea origen quede expandida
                var view = grillaArbol.View;
                if (view != null)
                    {
                    var node = FindNodeByData(view.Nodes, origen);
                    if (node != null)
                        grillaArbol.ExpandNode(node);
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error al buscar análisis de costo con IA: {ex.Message}");
                }
            }
        }
    }


