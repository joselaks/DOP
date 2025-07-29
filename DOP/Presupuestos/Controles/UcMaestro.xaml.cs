using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DOP.Presupuestos.Clases;
using Syncfusion.UI.Xaml.TreeGrid;
using System;
using System.Collections.Generic;
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
                if (nodoMovido != null)
                    {
                    // Verifica si ya existe un nodo con el mismo ID en el árbol
                    bool yaExiste = Objeto.Arbol.Any(n => n.ID == nodoMovido.ID);
                    if (!yaExiste)
                        {
                        if (nodoMovido.Tipo == "T" || nodoMovido.Tipo == "A")
                            {
                            Objeto.Arbol.Add(Objeto.clonar(nodoMovido, true));

                            }
                        else
                            {
                            Objeto.Arbol.Add(Objeto.clonar(nodoMovido, false));

                            }
                        }
                   
                    }
                }
            }
        }
    }




