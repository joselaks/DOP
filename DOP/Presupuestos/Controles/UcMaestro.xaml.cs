using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DOP.Presupuestos.Clases;
using Syncfusion.UI.Xaml.TreeGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para cMaestro.xaml
    /// </summary>
    public partial class UcMaestro : UserControl
        {

        public Maestro Objeto;
        private string tipoSeleccionado = null;


        public UcMaestro()
            {
            InitializeComponent();
            this.grillaMaestro.RowDragDropController.Drop += RowDragDropController_Drop;
            this.grillaMaestro.RowDragDropController.DragStart += RowDragDropController_DragStart;
            this.grillaMaestro.Loaded += GrillaMaestro_Loaded;
            this.grillaMaestro.ChildPropertyName = "Inferiores";


            }

        private async void GrillaMaestro_Loaded(object sender, RoutedEventArgs e)
            {

            // Obtén el usuarioID desde donde corresponda en tu aplicación
            int usuarioID = App.IdUsuario; // Usa el ID del login 

            var (success, message, conceptos, relaciones) = await DOP.Datos.DatosWeb.ObtenerConceptosYRelacionesMaestroAsync(usuarioID);
            Objeto = new Maestro(conceptos, relaciones, usuarioID);

            grillaMaestro.ItemsSource = Objeto.Arbol;
            this.grillaMaestro.View.Filter = FiltrarPorTipo;
            this.grillaMaestro.View.Refresh();


            }

        private void SelectorTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                tipoSeleccionado = SelectorTipo.Text;
                grillaMaestro.View.Refresh();
            }), System.Windows.Threading.DispatcherPriority.Background);
            }



        private bool FiltrarPorTipo(object item)
            {
            if (item is Nodo nodo)
                {
                if (string.IsNullOrEmpty(tipoSeleccionado) || tipoSeleccionado == "Todos")
                    return true;

                // Mapeo de los textos del ComboBox a los valores de Tipo
                switch (tipoSeleccionado)
                    {
                    case "Rubros": return nodo.Tipo == "R";
                    case "Tareas": return nodo.Tipo == "T";
                    case "Materiales": return nodo.Tipo == "M";
                    case "Mano de obra": return nodo.Tipo == "D";
                    case "Equipos": return nodo.Tipo == "E";
                    case "Subcontratos": return nodo.Tipo == "S";
                    case "Otros": return nodo.Tipo == "O";
                    case "Auxiliares": return nodo.Tipo == "A";
                    default: return true;
                    }
                }
            return false;
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

        private void RowDragDropController_DragStart(object? sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridRowDragStartEventArgs e)
            {
            DragDropContext.DragSourceUserControl = GetParentUserControl(this.grillaMaestro);
            }


        private void RowDragDropController_Drop(object? sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridRowDropEventArgs e)
            {
            e.Handled = true;
            Nodo nodoMovido = null;

            if (e.DraggingNodes != null && e.DraggingNodes.Count > 0)
                {
                nodoMovido = e.DraggingNodes[0].Item as Nodo;

                // 1. Copiar los inferiores de nodoMovido a nivel raíz (como antes)
                if (nodoMovido != null && nodoMovido.Inferiores != null && (nodoMovido.Tipo == "T" || nodoMovido.Tipo == "A"))
                    {
                    foreach (var inferior in nodoMovido.Inferiores)
                        {
                        var existente = Objeto.Arbol.FirstOrDefault(n => n.ID == inferior.ID);
                        if (existente == null)
                            {
                            Objeto.Arbol.Add(Objeto.clonar(inferior, true));
                            }
                        else
                            {
                            existente.Descripcion = inferior.Descripcion;
                            existente.Unidad = inferior.Unidad;
                            existente.Cantidad = inferior.Cantidad;
                            existente.PU1 = inferior.PU1;
                            existente.Tipo = inferior.Tipo;
                            existente.Inferiores = Objeto.GetClonesInferiores(inferior);
                            }
                        }
                    }

                // 2. Clonar el nodoMovido y agregarlo en la ubicación de drop
                var nodoClonado = (nodoMovido.Tipo == "T" || nodoMovido.Tipo == "A")
                        ? Objeto.clonar(nodoMovido, true)
                        : Objeto.clonar(nodoMovido, false);

                // Determinar el nodo destino y la posición
                var targetNode = e.TargetNode?.Item as Nodo;
                var dropPosition = e.DropPosition; // Before, After, or Child

                if (targetNode == null || dropPosition == Syncfusion.UI.Xaml.TreeGrid.DropPosition.DropAsChild)
                    {
                    // Si no hay destino o es como hijo, agregar a la raíz o a los inferiores del destino
                    if (targetNode == null)
                        {
                        Objeto.Arbol.Add(nodoClonado);
                        }
                    else
                        {
                        if (targetNode.Inferiores == null)
                            targetNode.Inferiores = new ObservableCollection<Nodo>();
                        targetNode.Inferiores.Add(nodoClonado);
                        }
                    }
                else
                    {
                    // Insertar antes o después del nodo destino en la colección correspondiente
                    var parentNode = e.TargetNode.ParentNode?.Item as Nodo;
                    ObservableCollection<Nodo> collection;
                    if (parentNode == null)
                        collection = Objeto.Arbol;
                    else
                        {
                        if (parentNode.Inferiores == null)
                            parentNode.Inferiores = new ObservableCollection<Nodo>();
                        collection = parentNode.Inferiores;
                        }

                    int index = collection.IndexOf(targetNode);
                    if (dropPosition == Syncfusion.UI.Xaml.TreeGrid.DropPosition.DropAbove)
                        collection.Insert(index, nodoClonado);
                    else // After
                        collection.Insert(index + 1, nodoClonado);
                    }
                }
            }


        public ObservableCollection<Nodo> GetClonesInferiores(Nodo elemento)
            {
            if (elemento == null)
                return null;

            if (!elemento.HasItems)
                {
                ObservableCollection<Nodo> inferioresVacios = new ObservableCollection<Nodo>();
                return inferioresVacios;
                }
            else
                {
                ObservableCollection<Nodo> inferioresLlenos = new ObservableCollection<Nodo>();
                foreach (var item in elemento.Inferiores)
                    {
                    Nodo respuesta = new Nodo();
                    respuesta.ID = item.ID;
                    respuesta.Descripcion = item.Descripcion;
                    respuesta.Unidad = item.Unidad;
                    respuesta.Cantidad = item.Cantidad;
                    respuesta.PU1 = item.PU1;
                    respuesta.Tipo = item.Tipo;
                    respuesta.Inferiores = GetClonesInferiores(item);
                    inferioresLlenos.Add(respuesta);
                    if (item.HasItems)
                        {
                        respuesta.Inferiores = GetClonesInferiores(item);
                        }
                    }
                return inferioresLlenos;
                }
            }
        }
    }




