using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DataObra.Agrupadores;
using DataObra.Controles;
using DataObra.Datos;
using Microsoft.Win32;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.Licensing;
using Syncfusion.UI.Xaml.Diagram;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.UI.Xaml.TreeView;
using Syncfusion.Windows.Controls.PivotGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        public event PropertyChangedEventHandler PropertyChanged;

        public Biblioteca.Documento Encabezado;
        public Presupuesto Objeto;
        private object _originalValue;
        Insumo _copia;
        Nodo _busca;
        bool ddeInsumos = false;
        bool ddeBuscador = false;
        ObservableCollection<Nodo> oBuscador = new ObservableCollection<Nodo>();
        Nodo anterior = new Nodo();
        private Stack<Cambios> undoStack;
        private Stack<Cambios> redoStack;
        public event EventHandler GuardadoConExito;


        //SfTreeGrid grillaNavegador = new SfTreeGrid();
        public UcPresupuesto(Biblioteca.Documento encabezado)
        {
            InitializeComponent();

            undoStack = new Stack<Cambios>();
            redoStack = new Stack<Cambios>();
            this.ComboObras.ItemsSource = App.ListaAgrupadores.Where(a => a.TipoID == 'O' && a.Active);
            if (encabezado!= null)
            {
                Encabezado = encabezado;
                this.descripcion.Text = Encabezado.Descrip;
                this.ComboObras.SelectedItem = App.ListaAgrupadores.FirstOrDefault(a => a.ID == encabezado.ObraID);
                DocumentoDTO doc = new DocumentoDTO();
                doc.ID = (int)Encabezado.ID;
                Objeto = new Presupuesto(doc);
                LlenaPresupuesto(doc.ID);

                }
            else
                {
                Encabezado = new Biblioteca.Documento();
                Objeto = new Presupuesto(null);
                }
               
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
        }

        private void OnGuardadoConExito()
            {
            GuardadoConExito?.Invoke(this, EventArgs.Empty);
            }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            Nodo nodoMovido = null;
            Nodo nodoReceptor = null;
            Nodo nodoPadreOriginal = null;
            int itemIndex = -1;

            if (e.DraggingNodes != null && e.DraggingNodes.Count > 0)
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

            if (e.TargetNode != null)
            {
                nodoReceptor = e.TargetNode.Item as Nodo;
            }
            else
            {
                nodoReceptor = Objeto.Arbol[0];
            }

            // Crear un registro de cambio y agregarlo a undoStack
            var undoRegistro = new Cambios
            {
                TipoCambio = "Mover",
                NodoMovido = nodoMovido,
                NodoPadreNuevo = nodoReceptor,
                NodoPadreAnterior = nodoPadreOriginal,
                Posicion = itemIndex
            };

            undoStack.Push(undoRegistro);
            redoStack.Clear(); // Limpiar la pila de rehacer cuando se realiza una nueva operación

            // Mostrar información de los nodos
            MessageBox.Show($"Nodo Movido: {nodoMovido?.Descripcion}\nNodo Receptor: {nodoReceptor?.Descripcion}\nNodo Padre Original: {nodoPadreOriginal?.Descripcion}\nÍndice del Nodo Movido: {itemIndex}");
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
                Bibioteca.Clases.Presupuesto pres = new Bibioteca.Clases.Presupuesto(null);

                Bibioteca.Clases.Presupuesto objetofieb = new Bibioteca.Clases.Presupuesto(null);
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

            totMateriales1.Value = Objeto.Arbol.Sum(i => i.Materiales1);
            totMDO1.Value = Objeto.Arbol.Sum(i => i.ManodeObra1);
            totEquipos1.Value = Objeto.Arbol.Sum(i => i.Equipos1);
            totSubcontratos1.Value = Objeto.Arbol.Sum(i => i.Subcontratos1);
            totOtros1.Value = Objeto.Arbol.Sum(i => i.Otros1);
            totGeneral1.Value = Objeto.Arbol.Sum(i => i.Importe1);
            decimal totalGeneralDol = Objeto.Arbol.Sum(i => i.Importe2);

            // Asignar el valor explícitamente al HeaderText
            var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
            colImporte1.HeaderText = $"$ {(totGeneral1.Value ?? 0m).ToString("N2", cultura)}";
            colImporte2.HeaderText = $"$ {totalGeneralDol.ToString("N2", cultura)}";


            //Totales grillas
            //listaInsumos.grillaInsumos.CalculateAggregates();
            //this.GrillaArbol.CalculateAggregates();
            //graficoInsumos.recalculo();
        }

        private void aRubro_Click(object sender, RoutedEventArgs e)
        {
            var (nuevoNodo, mensaje) = Objeto.agregaNodo("R", null);
            var undoRegistro = new Cambios
            {
                TipoCambio = "Nuevo",
                despuesCambio = nuevoNodo,
                NodoPadre = null,
                Posicion = Objeto.Arbol.IndexOf(nuevoNodo)
            };

            undoStack.Push(undoRegistro);
            redoStack.Clear(); // Limpiar la pila de rehacer cuando se realiza una nueva operación
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


        private void DropDownMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AgregarNodo("M");
        }

        private void DropDownMenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            AgregarNodo("D");
        }

        private void DropDownMenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            AgregarNodo("E");
        }

        private void DropDownMenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            AgregarNodo("S");
        }

        private void DropDownMenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            AgregarNodo("O");
        }

        private void sAux_Click(object sender, RoutedEventArgs e)
        {
            AgregarNodo("A");
        }


        private void AgregarNodo(string tipo)
        {
            if (this.grillaArbol.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un nodo para agregar el nuevo nodo.");
                return;
            }

            Nodo sele = this.grillaArbol.SelectedItem as Nodo;
            if (sele == null || (tipo != "A" && sele.Tipo != "T"))
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

            undoStack.Push(undoRegistro);
            redoStack.Clear(); // Limpiar la pila de rehacer cuando se realiza una nueva operación
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
            Objeto.cambioDesdeInsumo(Objeto.Arbol, editado.ID, editado.PU1, editado.PU2);
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
                Bibioteca.Clases.Presupuesto pres = new Bibioteca.Clases.Presupuesto(null);

                Bibioteca.Clases.Presupuesto objetofieb = new Bibioteca.Clases.Presupuesto(null);
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
                        case "Nuevo":
                            Objeto.borraNodo(Objeto.Arbol, lastChange.despuesCambio);
                            break;
                        case "Borrado":
                            Objeto.RestaurarNodo(lastChange.despuesCambio, lastChange.NodoPadre, lastChange.Posicion);
                            break;
                        case "Tipeo":
                            edicion(lastChange.antesCambio, lastChange.PropiedadCambiada);
                            break;
                        case "Mover":
                            // Deshacer el movimiento
                            Objeto.borraNodo(Objeto.Arbol, lastChange.NodoMovido);
                            Objeto.RestaurarNodo(lastChange.NodoMovido, lastChange.NodoPadreAnterior,  lastChange.Posicion);
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
                        case "Nuevo":
                            Objeto.RestaurarNodo(lastChange.despuesCambio, lastChange.NodoPadre, lastChange.Posicion);
                            break;
                        case "Borrado":
                            Objeto.borraNodo(Objeto.Arbol, lastChange.despuesCambio);
                            break;
                        case "Tipeo":
                            edicion(lastChange.despuesCambio, lastChange.PropiedadCambiada);
                            break;
                        case "Mover":
                            // Rehacer el movimiento
                            Objeto.borraNodo(Objeto.Arbol, lastChange.NodoMovido);
                            Objeto.RestaurarNodo(lastChange.NodoMovido, lastChange.NodoPadreNuevo,  lastChange.Posicion);
                            break;
                        default:
                            break;
                    }
                    undoStack.Push(lastChange);
                }
            }
        }




        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
            {
            bool esNuevo = Encabezado.ID == null;

            var documento = ConvertirADTO(Encabezado, esNuevo);

            (bool, string, int?) EmpaquetarResultado((bool, string) r, int? id) => (r.Item1, r.Item2, id);

            var resultado = esNuevo
                ? await DatosWeb.CrearDocumentoAsync(documento)
                : EmpaquetarResultado(await DatosWeb.ActualizarDocumentoAsync(documento), documento.ID);

            if (resultado.Item1 && resultado.Item3.HasValue)
                {
                // Asignar el ID devuelto al Encabezado
                if (esNuevo)
                    {
                    Encabezado.ID = resultado.Item3.Value;
                    }

                Objeto.listaConceptosGrabar.Clear();
                Objeto.listaRelacionesGrabar.Clear();
                Objeto.aplanar(Objeto.Arbol, null);

                MessageBox.Show($"Cantidad de registros en listaConceptosGrabar: {Objeto.listaConceptosGrabar.Count}\nCantidad de registros en listaRelacionesGrabar: {Objeto.listaRelacionesGrabar.Count}");
                await ProcesarArbolPresupuestoRequest();

                MessageBox.Show("Documento guardado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Disparar el evento para notificar el guardado exitoso
                OnGuardadoConExito();
                }
            else
                {
                MessageBox.Show($"Error al guardar el documento: {resultado.Item2}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        private async Task ProcesarArbolPresupuestoRequest()
        {
           

            foreach (var conceptoGrabar in Objeto.listaConceptosGrabar)
            {
                var conceptoLeer = Objeto.listaConceptosLeer.FirstOrDefault(c => c.Codigo == conceptoGrabar.Codigo);
                if (conceptoLeer == null)
                {
                    // Si el concepto de listaConceptosGrabar no existe en listaConceptosLeer, agregar "A" al campo acción
                    conceptoGrabar.Accion = 'A';
                }
                else
                {
                    // Al resto de los registros de listaConceptosGrabar, agregar "M" al campo acción
                    conceptoGrabar.Accion = 'M';
                }
            }

            // Comparar listaConceptosLeer y listaConceptosGrabar
            foreach (var conceptoLeer in Objeto.listaConceptosLeer)
                {
                var conceptoGrabar = Objeto.listaConceptosGrabar.FirstOrDefault(c => c.Codigo == conceptoLeer.Codigo);
                if (conceptoGrabar == null)
                    {
                    // Si el concepto de listaConceptosLeer no existe en listaConceptosGrabar, agregar "D" al campo acción
                    conceptoLeer.Accion = 'D';
                    Objeto.listaConceptosGrabar.Add(conceptoLeer);
                    }
                }

            
            foreach (var relacionGrabar in Objeto.listaRelacionesGrabar)
            {
                var relacionLeer = Objeto.listaRelacionesLeer.FirstOrDefault(r => r.Superior == relacionGrabar.Superior && r.Inferior == relacionGrabar.Inferior);
                if (relacionLeer == null)
                {
                    // Si la relación de listaRelacionesGrabar no existe en listaRelacionesLeer, agregar "A" al campo acción
                    relacionGrabar.Accion = 'A';
                }
                else
                {
                    // Al resto de los registros de listaRelacionesGrabar, agregar "M" al campo acción
                    relacionGrabar.Accion = 'M';
                }
            }

            // Comparar listaRelacionesLeer y listaRelacionesGrabar
            foreach (var relacionLeer in Objeto.listaRelacionesLeer)
                {
                var relacionGrabar = Objeto.listaRelacionesGrabar.FirstOrDefault(r => r.Superior == relacionLeer.Superior && r.Inferior == relacionLeer.Inferior);
                if (relacionGrabar == null)
                    {
                    // Si la relación de listaRelacionesLeer no existe en listaRelacionesGrabar, agregar "D" al campo acción
                    relacionLeer.Accion = 'D';
                    Objeto.listaRelacionesGrabar.Add(relacionLeer);
                    }
                }

            // Verificar que estén completos todos los campos que requiere ProcesarArbolPresupuestoAsync
            foreach (var concepto in Objeto.listaConceptosGrabar)
            {
                var missingFields = new List<string>();
                if (concepto.PresupuestoID == 0) missingFields.Add(nameof(concepto.PresupuestoID));
                if (string.IsNullOrEmpty(concepto.Codigo)) missingFields.Add(nameof(concepto.Codigo));
                if (string.IsNullOrEmpty(concepto.Descrip)) missingFields.Add(nameof(concepto.Descrip));
                if (concepto.Tipo == '\0') missingFields.Add(nameof(concepto.Tipo));
                if (concepto.Precio1 == null) missingFields.Add(nameof(concepto.Precio1));
                if (concepto.Precio2 == null) missingFields.Add(nameof(concepto.Precio2));
                if (concepto.FechaPrecio == null) missingFields.Add(nameof(concepto.FechaPrecio));
                //if (string.IsNullOrEmpty(concepto.Unidad)) missingFields.Add(nameof(concepto.Unidad));

                if (missingFields.Any())
                {
                    throw new InvalidOperationException($"Faltan campos requeridos en el concepto con Código {concepto.Codigo}: {string.Join(", ", missingFields)}");
                }
            }

            // Crear el objeto ProcesaPresupuestoDTO
            var request = new ProcesaPresupuestoDTO
            {
                PresupuestoID = Encabezado?.ID ?? 0,
                ListaConceptos = Objeto.listaConceptosGrabar.Select(c => new ConceptoDTO
                {
                    PresupuestoID = c.PresupuestoID,
                    Codigo = c.Codigo,
                    Descrip = c.Descrip,
                    Tipo = c.Tipo,
                    Precio1 = c.Precio1 ?? 0,
                    Precio2 = c.Precio2 ?? 0,
                    FechaPrecio = c.FechaPrecio ?? DateTime.MinValue,
                    Unidad = c.Unidad ?? "Gl",
                    CanPr = c.CanPr ?? 0,
                    CanPe = c.CanPe ?? 0,
                    CanCo = c.CanCo ?? 0,
                    CanEn = c.CanEn ?? 0,
                    CanFa = c.CanFa ?? 0,
                    CanEj = c.CanEj ?? 0,
                    UltimoPrecio1 = c.UltimoPrecio1 ?? 0,
                    UltimoPrecio2 = c.UltimoPrecio2 ?? 0,
                    FechaUltimoPrecio = c.FechaUltimoPrecio ?? DateTime.MinValue,
                    DocumentoID = c.DocumentoID ?? 0,
                    InsumoID = c.InsumoID ?? 0,
                    Accion = c.Accion
                }).ToList(),
                ListaRelaciones = Objeto.listaRelacionesGrabar.Select(r => new RelacionDTO
                {
                    PresupuestoID = r.PresupuestoID,
                    Superior = r.Superior ,
                    Inferior = r.Inferior,
                    Cantidad = r.Cantidad,
                    OrdenInt = r.OrdenInt ?? 0,
                    Accion = r.Accion
                }).ToList()
            };

            // Llamar al método ProcesarArbolPresupuestoAsync
            var result = await DatosWeb.ProcesarArbolPresupuestoAsync(request);

            // Manejar la respuesta
            if (result.Success)
            {
                MessageBox.Show("Árbol de presupuesto procesado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error al procesar el árbol de presupuesto: {result.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }







        private void descripcion_KeyDown(object sender, KeyEventArgs e)
        {
            //Actualiza el campo porque no uso Inotificable
            Encabezado.Descrip = this.descripcion.Text;
        }

        private async void LlenaPresupuesto(int idPres)
        {
            var (success, message, data) = await DatosWeb.ObtenerRegistrosPorPresupuestoIDAsync(idPres);

            if (!success)
            {
                MessageBox.Show($"Error al obtener los registros: {message}");
                return;
            }
            // Generar el presupuesto con los conceptos y relaciones creados
            Objeto.generaPresupuesto(null, data.Conceptos, data.Relaciones);

            // Asignar el árbol generado a la grilla
            recalculo();
        }

        private void ComboObras_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var sele = ComboObras.SelectedItem as AgrupadorDTO;
            if (sele != null)
            {
                Encabezado.ObraID = sele.ID;
                Encabezado.Obra = sele.Descrip;
            }
        }

        private DocumentoDTO ConvertirADTO(Documento doc, bool esNuevo)
        {
            var dto = new DocumentoDTO
            {
                CuentaID = 1,
                TipoID = 10,
                UsuarioID = 3,
                CreadoFecha = DateTime.Now,
                EditadoID = 1,
                EditadoFecha = DateTime.Now,
                AutorizadoID = 1,
                AutorizadoFecha = DateTime.Now,
                AdminID = 1,
                ObraID = Encabezado.ObraID,
                PresupuestoID = null,
                RubroID = null,
                EntidadID = null,
                DepositoID = null,
                Descrip = this.descripcion.Text,
                Concepto1 = null,
                Fecha1 = DateTime.Now,
                Fecha2 = DateTime.Now,
                Fecha3 = DateTime.Now,
                Numero1 = null,
                Numero2 = null,
                Numero3 = null,
                Notas = "bb",
                Active = false,
                Precio1 = Objeto.Arbol.Sum(i => i.Importe1),
                Precio2 = 0,
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

            if (!esNuevo)
                dto.ID = (int)doc.ID;

            return dto;

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

