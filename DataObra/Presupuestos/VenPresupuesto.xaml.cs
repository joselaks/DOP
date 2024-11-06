using Bibioteca.Clases;
using Microsoft.Win32;
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
    public partial class VenPresupuesto : Window
    {
        public Presupuesto Objeto;
        private object _originalValue;
        Insumo _copia;
        public VenPresupuesto()
        {
            InitializeComponent();
            Objeto = new Presupuesto();
            this.grillaArbol.ItemsSource = Objeto.Arbol;
            this.grillaArbol.ChildPropertyName = "Inferiores";
            this.grillaDetalle.ItemsSource = Objeto.Insumos;
            this.grillaArbol.RowDragDropController.CanAutoExpand = true;
            this.grillaArbol.RowDragDropController.AutoExpandDelay = new TimeSpan(0, 0, 2);
            this.grillaArbol.RowDragDropController.Drop += RowDragDropController_Drop;
            this.grillaDetalle.RowDragDropController.DragStart += RowDragDropController_DragStart1;
        }


        private void RowDragDropController_DragStart1(object? sender, TreeGridRowDragStartEventArgs e)
        {

            foreach (var node in e.DraggingNodes)
            {
                if (node.Item !=null)
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



            //if (grillaDetalle.SelectedItems != null)
            //{
            //    foreach (var item in grillaDetalle.SelectedItems)
            //    {
            //        _copia = item as Insumo;
            //    }
            //}
            //else
            //{
            //    e.Handled = true;
            //}
            
        }

        private void RowDragDropController_Drop(object? sender, TreeGridRowDropEventArgs e)
        {
            if (e.IsFromOutSideSource)
            {
                if (_copia != null)
                {
                    Nodo nuevo = new Nodo
                    {
                        Descripcion = _copia.Descripcion,
                        Cantidad = _copia.Cantidad
                    };

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
                                if (parent.Tipo == "T")
                                {
                                    // Colección de hijos del nodo padre
                                    var collection = grillaArbol.View.GetPropertyAccessProvider().GetValue(parentNode.Item, grillaArbol.ChildPropertyName) as IEnumerable;
                                    sourceCollection = GetSourceListCollection(collection);
                                }
                                else
                                {
                                    // Mostrar mensaje de error y salir sin insertar
                                    MessageBox.Show("El nodo padre no cumple con la condición requerida (Tipo = 'T').");
                                     e.Handled = true;;
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
                            sourceCollection.Insert(itemIndex, nuevo);
                        }

                        grillaArbol.SelectionController.ResumeUpdates();
                        (grillaArbol.SelectionController as TreeGridRowSelectionController).RefreshSelection();
                        e.Handled = true;
                    }

                    if (grillaDetalle.ItemsSource is ObservableCollection<Insumo> insumos)
                    {
                        //insumos.Remove(_copia);
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
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

        private void grillaArbol_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {


            var column = grillaArbol.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var editado = grillaArbol.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
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


        private void grillaArbol_CurrentCellBeginEdit(object sender, TreeGridCurrentCellBeginEditEventArgs e)
        {
            var column = grillaArbol.Columns[e.RowColumnIndex.ColumnIndex].MappingName;
            var record = grillaArbol.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;

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

        private void aRubro_Click(object sender, RoutedEventArgs e)
        {
            Objeto.agregaNodo("R", null);
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
                    RemoveItemRecursively(Objeto.Arbol, item);
                }
            }


        }

        private bool RemoveItemRecursively(ObservableCollection<Nodo> collection, Nodo itemToRemove)
        {
            if (collection.Contains(itemToRemove))
            {
                collection.Remove(itemToRemove); return true;
            }
            foreach (var item in collection)
            {
                if (RemoveItemRecursively(item.inferiores, itemToRemove))
                {
                    return true;
                }
            }
            return false;
        }
    }

}

