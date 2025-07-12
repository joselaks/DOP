using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
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

namespace DOP.Presupuestos.Controles
    {
    /// <summary>
    /// Lógica de interacción para UcTareas.xaml
    /// </summary>
    public partial class UcTareas : UserControl
        {
        public Maestro Objeto;


        public UcTareas()
            {
            InitializeComponent();

            // Inicializa el objeto Maestro si es necesario
            if (Objeto == null)
                Objeto = new Maestro(new List<ConceptoMDTO>(), new List<RelacionMDTO>(), 2);

            // Crea un nodo de ejemplo
            var nodoEjemplo = new Nodo
                {
                ID = "N1",
                Descripcion = "Nodo de ejemplo",
                Tipo = "T",
                Unidad = "m2",
                Cantidad = 10,
                PU1 = 100,
                Importe1 = 1000
                };

            // Agrega el nodo al árbol
            if (Objeto.Arbol == null)
                Objeto.Arbol = new System.Collections.ObjectModel.ObservableCollection<Nodo>();

            //Objeto.Arbol.Add(nodoEjemplo);

            // Enlaza la grilla a la colección de nodos
            grillaTareas.ItemsSource = Objeto.Arbol;

            this.grillaTareas.RowDragDropController.Drop += RowDragDropController_Drop;
            this.grillaTareas.RowDragDropController.DragStart += RowDragDropController_DragStart;
            this.grillaTareas.ChildPropertyName = "Inferiores";
            }

        private void RowDragDropController_DragStart(object? sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridRowDragStartEventArgs e)
            {
            MessageBox.Show("Arrastrando fila");
            }

        private void RowDragDropController_Drop(object? sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridRowDropEventArgs e)
            {
            Nodo nodoMovido = null;
            Nodo nodoReceptor = null;

            if (e.DraggingNodes != null && e.DraggingNodes.Count > 0)
                {
                nodoMovido = e.DraggingNodes[0].Item as Nodo;
                Objeto.Arbol.Add(Objeto.clonar(nodoMovido));
                }

            }

        private void Button_Click(object sender, RoutedEventArgs e)
            {
            ProcesaTareaMaestroRequest oGrabar = Objeto.EmpaquetarPresupuesto();
            if (oGrabar != null)
                {
                // Aquí puedes procesar la solicitud de grabación
                MessageBox.Show("Solicitud de grabación procesada correctamente.");
                }
            else
                {
                MessageBox.Show("Error al procesar la solicitud de grabación.");
                }
            }
        }
    }
