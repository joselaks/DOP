using Bibioteca.Clases;
using DataObra.Controles;
using DataObra.Datos;
using Microsoft.Win32;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.Licensing;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.UI.Xaml.TreeView;
using Syncfusion.Windows.Controls.PivotGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataObra.Presupuestos
{
    /// <summary>
    /// Lógica de interacción para VenPresupuesto.xaml
    /// </summary>
    public partial class UcPresupuesto : UserControl
    {
        public Presupuesto Objeto;
        private object _originalValue;
        Insumo _copia;
        Nodo _busca;
        bool ddeInsumos = false;
        bool ddeBuscador = false;
        ObservableCollection<Nodo> oBuscador = new ObservableCollection<Nodo>();
        ConsultasAPI consultasAPI;
        Nodo anterior = new Nodo();
        private Stack<Cambios> undoStack;
        private Stack<Cambios> redoStack;


        //SfTreeGrid grillaNavegador = new SfTreeGrid();
        public UcPresupuesto(int? id)
        {
            InitializeComponent();
            undoStack = new Stack<Cambios>();
            redoStack = new Stack<Cambios>();

            Objeto = new Presupuesto();
            consultasAPI = new ConsultasAPI();
            this.grillaArbol.ItemsSource = Objeto.Arbol;
            this.grillaArbol.ChildPropertyName = "Inferiores";
            this.grillaDetalle.ItemsSource = Objeto.Insumos;
            this.grillaArbol.RowDragDropController.CanAutoExpand = true;
            this.grillaArbol.RowDragDropController.AutoExpandDelay = new TimeSpan(0, 0, 2);
            this.grillaArbol.RowDragDropController.Drop += RowDragDropController_Drop;
            this.grillaNavegador.RowDragDropController.DragStart += RowDragDropController_DragStart;
            this.grillaDetalle.RowDragDropController.DragStart += RowDragDropController_DragStart1;
            this.grillaArbol.SelectionBackground = null;
            //grillaNavegador.AllowDraggingRows = true;
            //Defino los cheques iniciales de columnas
            this.colCodigo.IsChecked = false;
            var cID = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "ID");
            cID.IsHidden = true;
            this.colTipo.IsChecked = true;

            if (id == null)
            {
                // Presupuesto nuevo
                nuevopres();


            }
            else
            {
                // Busco la info del presupuesto existente
            }


        }

        private async void nuevopres()
        {

            #region Datos para testeo 

            var documento = new Biblioteca.Documento
            {
                // Define las propiedades del documento
                CuentaID = 1,
                TipoID = 10,
                UsuarioID = 3,
                CreadoFecha = DateTime.Now,
                EditadoID = 4,
                EditadoFecha = DateTime.Now,
                RevisadoID = 5,
                RevisadoFecha = DateTime.Now,
                AdminID = 3,
                ObraID = 5,
                PresupuestoID = 6,
                RubroID = 6,
                EntidadID = 7,
                DepositoID = 5,
                Descrip = "a",
                Concepto1 = "b",
                Fecha1 = DateTime.Now,
                Fecha2 = DateTime.Now,

                Fecha3 = DateTime.Now,
                Numero1 = 0,
                Numero2 = 0,
                Numero3 = 0,
                Notas = "bb",
                Active = false,
                Pesos = 0,
                Dolares = 0,
                Impuestos = 0,
                ImpuestosD = 0,
                Materiales = 0,
                ManodeObra = 0,
                Subcontratos = 0,
                Equipos = 0,
                Otros = 0,
                MaterialesD = 0,
                ManodeObraD = 0,
                SubcontratosD = 0,
                EquiposD = 0,
                OtrosD = 0,
                RelDoc = false,
                RelArt = false,
                RelMov = false,
                RelImp = false,
                RelRub = false,
                RelTar = false,
                RelIns = false
            };

            #endregion

            // Codigo a utilizar
            var respuesta = await consultasAPI.PostDocumentoAsync(documento);

            //Respuestas
            int? nuevodoc = respuesta.Id;
            bool conexionExitosa = respuesta.Success;
            string mensaje = respuesta.Message;

            //Mensaje para testeo
            MessageBox.Show(respuesta.Success + " " + mensaje + " " + nuevodoc.ToString());
        }

        private void RowDragDropController_DragStart(object? sender, TreeGridRowDragStartEventArgs e)
        {
            ddeBuscador = true;
            foreach (var node in e.DraggingNodes)
            {
                if (node.Item != null)
                {
                    //MessageBox.Show("Si se seleccionaron insumos");
                    _busca = node.Item as Nodo;

                }
                else
                {
                    MessageBox.Show("No se seleccionó nada");
                    e.Handled = true;
                    return;
                }
            }
        }

        private void RowDragDropController_DragStart1(object? sender, TreeGridRowDragStartEventArgs e)
        {
            ddeInsumos = true;
            foreach (var node in e.DraggingNodes)
            {
                if (node.Item != null)
                {
                    //MessageBox.Show("Si se seleccionaron insumos");
                    _copia = node.Item as Insumo;

                }
                else
                {
                    MessageBox.Show("No se seleccionaron insumos");
                    e.Handled = true;
                    return;
                }
            }

        }

        private void RowDragDropController_Drop(object? sender, TreeGridRowDropEventArgs e)
        {
            Nodo nuevo = null;
            if (e.IsFromOutSideSource)
            {
                
                if (ddeInsumos == true || ddeBuscador == true )
                {
                    if (ddeInsumos == true)
                    {
                        nuevo = new Nodo
                        {
                            Descripcion = _copia.Descripcion,
                            Cantidad = _copia.Cantidad
                        };
                    }

                    var dropPosition = e.DropPosition.ToString();
                    var rowIndex = grillaArbol.ResolveToRowIndex(e.TargetNode.Item);
                    int nodeIndex = (int)rowIndex;

                    if (dropPosition != "None" && rowIndex != -1)
                    {
                        TreeNode treeNode = grillaArbol.GetNodeAtRowIndex(rowIndex);

                        if (treeNode == null)
                            return;

                        grillaArbol.SelectionController.SuspendUpdates();
                        var itemIndex = -1;
                        IList sourceCollection = null;

                        if (dropPosition == "DropBelow" || dropPosition == "DropAbove")
                        {
                            TreeNode parentNode = treeNode.ParentNode;

                            if (parentNode == null)
                            {
                                // Caso raíz
                                sourceCollection = grillaArbol.View.SourceCollection as IList;
                            }
                            else
                            {
                                // Verificar la condición del nodo padre
                                var parent = parentNode.Item as Nodo;
                                if (parent.Tipo == "T" || ddeBuscador == true)
                                {
                                    // Colección de hijos del nodo padre
                                    var collection = grillaArbol.View.GetPropertyAccessProvider().GetValue(parentNode.Item, grillaArbol.ChildPropertyName) as IEnumerable;
                                    sourceCollection = GetSourceListCollection(collection);
                                }
                                else
                                {
                                    // Mostrar mensaje de error y salir sin insertar
                                    MessageBox.Show("El nodo padre no cumple con la condición requerida (Tipo = 'T').");
                                    e.Handled = true; ;
                                    return;
                                }
                            }

                            itemIndex = sourceCollection.IndexOf(treeNode.Item);

                            if (dropPosition == "DropBelow")
                            {
                                itemIndex += 1;
                            }
                        }
                        else if (dropPosition == "DropAsChild")
                        {
                            var parent = treeNode.Item as Nodo;
                            if (parent.Tipo == "T")
                            {
                                var collection = grillaArbol.View.GetPropertyAccessProvider().GetValue(treeNode.Item, grillaArbol.ChildPropertyName) as IEnumerable;
                                sourceCollection = GetSourceListCollection(collection);

                                if (sourceCollection == null)
                                {
                                    var list = new ObservableCollection<Nodo>();
                                    grillaArbol.View.GetPropertyAccessProvider().SetValue(treeNode.Item, grillaArbol.ChildPropertyName, list);
                                    sourceCollection = list;
                                }

                                itemIndex = sourceCollection.Count;

                                // Expande el nodo para mostrar los hijos
                                if (!treeNode.IsExpanded)
                                {
                                    grillaArbol.ExpandNode(treeNode);
                                }
                            }
                            else
                            {
                                // Mostrar mensaje de error y salir sin insertar
                                MessageBox.Show("El nodo padre no cumple con la condición requerida (Tipo = 'T').");
                                e.Handled = true;
                                return;
                            }
                        }

                        if (sourceCollection != null && itemIndex >= 0)
                        {
                            if (ddeInsumos == true)
                            {
                                    sourceCollection.Insert(itemIndex, nuevo);
                            }
                            if (ddeBuscador == true) 
                            {
                                sourceCollection.Insert(itemIndex, _busca);
                            }
                        }

                        grillaArbol.SelectionController.ResumeUpdates();
                        (grillaArbol.SelectionController as TreeGridRowSelectionController).RefreshSelection();
                        e.Handled = true;
                    }

                    //if (grillaDetalle.ItemsSource is ObservableCollection<Insumo> insumos)
                    //{
                    //    insumos.Remove(_copia);
                    //}
                }
                else
                {
                    e.Handled = true;
                }
            }
            ddeInsumos = false;
            ddeBuscador = false;
        }

        private IList GetSourceListCollection(IEnumerable collection)
        {
            if (collection is IList list)
            {
                return list;
            }

            return new List<object>(collection.Cast<object>());
        }

        private void Fiebdc_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Archivo Fiebdc|*.bc3";

            if (openFileDialog.ShowDialog().Value)
            {
                FileStream stream = File.OpenRead(openFileDialog.FileName);
                string textoFie;
                using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                {
                    textoFie = reader.ReadToEnd();
                    //this.txtArchivoActualiza.Text = "Archivo seleccionado";
                    string txtNombre = stream.Name;
                }
                Bibioteca.Clases.Fiebdc fie = new Bibioteca.Clases.Fiebdc(textoFie);
                Bibioteca.Clases.Presupuesto pres = new Bibioteca.Clases.Presupuesto();

                Bibioteca.Clases.Presupuesto objetofieb = new Bibioteca.Clases.Presupuesto();
                objetofieb.generaPresupuesto("fie", fie.listaConceptos, fie.listaRelaciones);
                foreach (var item in objetofieb.Arbol)
                {
                    Objeto.Arbol.Add(item);
                }
                recalculo();
            }
        }

        public void recalculo()
        {
            Objeto.recalculo(Objeto.Arbol, true, 0, true);

            Objeto.sinCero();

            totMateriales.Value = Objeto.Arbol.Sum(i => i.Materiales);
            totMDO.Value = Objeto.Arbol.Sum(i => i.ManodeObra);
            totEquipos.Value = Objeto.Arbol.Sum(i => i.Equipos);
            totSubcontratos.Value = Objeto.Arbol.Sum(i => i.Subcontratos);
            totOtros.Value = Objeto.Arbol.Sum(i => i.Otros);
            totGeneral.Value = Objeto.Arbol.Sum(i => i.Importe);
            //Totales grillas
            //listaInsumos.grillaInsumos.CalculateAggregates();
            //this.GrillaArbol.CalculateAggregates();
            //graficoInsumos.recalculo();
        }

        //Agrega Rubro
        private void aRubro_Click(object sender, RoutedEventArgs e)
        {
            Objeto.agregaNodo("R", null);
        }
        
        //Agrega Tarea
        private void aTarea_Click(object sender, RoutedEventArgs e)
        {
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
                }
            }
        }
       
        //Agrega Auxiliar
        private void sAux_Click(object sender, RoutedEventArgs e)
        {
            if (this.grillaArbol.SelectedItem == null)
            {
                MessageBox.Show("debe seleccionar ua tarea para el auxiliar");
            }
            else
            {
                Bibioteca.Clases.Nodo sele = this.grillaArbol.SelectedItem as Bibioteca.Clases.Nodo; //obtine contenido
                if (sele.Tipo != "T" && sele.Tipo != "A")
                {
                    MessageBox.Show("debe seleccionar una tarea u otro auxiliar para el auxiliar");
                }
                else
                {
                    Objeto.agregaNodo("A", sele);
                }
            }
        }




        //Evento cuando se edita una celda del presupuesto
        private void grillaArbol_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            var column = grillaArbol.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var editado = grillaArbol.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
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
            
            undoStack.Push(undoRegistro);

        }

        //Edicion de celdas
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
            recalculo();
        }

        //Graba el ID antes de editar celdas
        private void grillaArbol_CurrentCellBeginEdit(object sender, TreeGridCurrentCellBeginEditEventArgs e)
        {
            var column = grillaArbol.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var record = grillaArbol.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
            // Clonar el objeto record y asignarlo a anterior
            anterior = Objeto.clonar(record);

            if (column == "ID")
            {
                _originalValue = record.ID;
            }

        }

        private void colCodigo_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "ID");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }
        }

        private void colTipo_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "Tipo");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }

        }

        private void colMat_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            // Encontrar la columna con el MappingName "Tipo"
            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "Materiales");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }
        }

        private void colMDO_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            // Encontrar la columna con el MappingName "Tipo"
            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "ManodeObra");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }
        }

        private void colEqi_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            // Encontrar la columna con el MappingName "Tipo"
            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "Equipos");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }

        }

        private void colSub_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            // Encontrar la columna con el MappingName "Tipo"
            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "Subcontratos");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }

        }

        private void colOtr_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isChecked = (bool)e.NewValue;

            // Encontrar la columna con el MappingName "Tipo"
            var column = grillaArbol.Columns.FirstOrDefault(c => c.MappingName == "Otros");

            if (column != null)
            {
                column.IsHidden = !isChecked; // Cambiar la condición IsHidden
            }

        }

        private void DropDownMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.grillaArbol.SelectedItem == null)
            {
                MessageBox.Show("debe seleccionar una tarea o auxiliar para  el material");
            }
            else
            {
                Bibioteca.Clases.Nodo sele = this.grillaArbol.SelectedItem as Bibioteca.Clases.Nodo; //obtine contenido
                if (sele.Tipo != "T" && sele.Tipo != "A")
                {
                    MessageBox.Show("debe seleccionar una tarea o auxiliar para el material");
                }
                else
                {
                    Objeto.agregaNodo("M", sele);
                }
            }
        }

        //Agrega mano de obra
        private void DropDownMenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.grillaArbol.SelectedItem == null)
            {
                MessageBox.Show("debe seleccionar ua tarea para la mano de obra");
            }
            else
            {
                Bibioteca.Clases.Nodo sele = this.grillaArbol.SelectedItem as Bibioteca.Clases.Nodo; //obtine contenido
                if (sele.Tipo != "T" && sele.Tipo != "A")
                {
                    MessageBox.Show("debe seleccionar una tarea para la mano de obra");
                }
                else
                {
                    Objeto.agregaNodo("D", sele);
                }
            }
        }

        private void DropDownMenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (this.grillaArbol.SelectedItem == null)
            {
                MessageBox.Show("debe seleccionar ua tarea para el equipo");
            }
            else
            {
                Bibioteca.Clases.Nodo sele = this.grillaArbol.SelectedItem as Bibioteca.Clases.Nodo; //obtine contenido
                if (sele.Tipo != "T" && sele.Tipo != "A")
                {
                    MessageBox.Show("debe seleccionar una tarea para el equipo");
                }
                else
                {
                    Objeto.agregaNodo("E", sele);
                }
            }
        }

        private void DropDownMenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (this.grillaArbol.SelectedItem == null)
            {
                MessageBox.Show("debe seleccionar ua tarea para la el subcontrato");
            }
            else
            {
                Bibioteca.Clases.Nodo sele = this.grillaArbol.SelectedItem as Bibioteca.Clases.Nodo; //obtine contenido
                if (sele.Tipo != "T" && sele.Tipo != "A")
                {
                    MessageBox.Show("debe seleccionar una tarea para el subcontrato");
                }
                else
                {
                    Objeto.agregaNodo("S", sele);
                }
            }
        }

        private void DropDownMenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (this.grillaArbol.SelectedItem == null)
            {
                MessageBox.Show("debe seleccionar ua tarea para el insumo");
            }
            else
            {
                Bibioteca.Clases.Nodo sele = this.grillaArbol.SelectedItem as Bibioteca.Clases.Nodo; //obtine contenido
                if (sele.Tipo != "T" && sele.Tipo != "A")
                {
                    MessageBox.Show("debe seleccionar una tarea para el insumo");
                }
                else
                {
                    Objeto.agregaNodo("O", sele);
                }
            }
        }


        private void recalculo_Click(object sender, RoutedEventArgs e)
        {
            recalculo();
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
                        undoStack.Push(undoRegistro);
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



        private void bus_Click(object sender, RoutedEventArgs e)
        {
            this.panelBuscador.Width = new GridLength(200, GridUnitType.Pixel);
        }

        private void grillaDetalle_CurrentCellBeginEdit(object sender, TreeGridCurrentCellBeginEditEventArgs e)
        {

        }

        private void grillaDetalle_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            var editado = grillaDetalle.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Insumo;
            Objeto.cambioDesdeInsumo(Objeto.Arbol, editado.ID, editado.PU);
            recalculo();
        }

        private void buscar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Archivo Fiebdc|*.bc3";

            if (openFileDialog.ShowDialog().Value)
            {
                FileStream stream = File.OpenRead(openFileDialog.FileName);
                string textoFie;
                using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                {
                    textoFie = reader.ReadToEnd();
                    //this.txtArchivoActualiza.Text = "Archivo seleccionado";
                    string txtNombre = stream.Name;
                }
                Bibioteca.Clases.Fiebdc fie = new Bibioteca.Clases.Fiebdc(textoFie);
                Bibioteca.Clases.Presupuesto pres = new Bibioteca.Clases.Presupuesto();

                Bibioteca.Clases.Presupuesto objetofieb = new Bibioteca.Clases.Presupuesto();
                objetofieb.generaPresupuesto("fie", fie.listaConceptos, fie.listaRelaciones);
                foreach (var item in objetofieb.Arbol)
                {
                    oBuscador.Add(item);
                }
                grillaNavegador.ChildPropertyName = "Inferiores";
                grillaNavegador.ItemsSource = oBuscador;
               
                //this.nav.Children.Add(grillaNavegador);

            }

        }



        private void UndoRedo_Click(object sender, RoutedEventArgs e)
        {
            if (sender == hacer)
            {
                if (undoStack.Count > 0)
                {
                    Cambios lastChange = undoStack.Pop();
                    switch (lastChange.TipoCambio)
                    {
                        case "Borrado":
                            Objeto.RestaurarNodo(lastChange.despuesCambio, lastChange.NodoPadre, lastChange.Posicion);
                            break;
                        case "Tipeo":
                            edicion(lastChange.antesCambio, lastChange.PropiedadCambiada);
                            break;
                        default:
                            break;
                    }
                    redoStack.Push(lastChange);
                }
            }
            else if (sender == deshacer)
            {
                if (redoStack.Count > 0)
                {
                    Cambios lastChange = redoStack.Pop();
                    switch (lastChange.TipoCambio)
                    {
                        case "Borrado":
                            Objeto.borraNodo(Objeto.Arbol, lastChange.despuesCambio);
                            break;
                        case "Tipeo":
                            edicion(lastChange.despuesCambio, lastChange.PropiedadCambiada);
                            break;
                        default:
                            break;
                    }
                    undoStack.Push(lastChange);
                }
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
        public int Posicion { get; set; }
    }


}

