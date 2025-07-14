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

            this.grillaTareas.RowDragDropController.Drop += RowDragDropController_Drop;
            this.grillaTareas.RowDragDropController.Dropped += RowDragDropController_Dropped;
            this.grillaTareas.RowDragDropController.DragStart += RowDragDropController_DragStart;
            this.grillaTareas.ChildPropertyName = "Inferiores";
            }

        private void RowDragDropController_Dropped(object? sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridRowDroppedEventArgs e)
            {
            
            }

        private void RowDragDropController_DragStart(object? sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridRowDragStartEventArgs e)
            {
            MessageBox.Show("Arrastrando fila");
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
                        Objeto.Arbol.Add(Objeto.clonar(nodoMovido));
                        }
                    }
                }
            }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
            {
            ProcesaTareaMaestroRequest oGrabar = Objeto.EmpaquetarPresupuesto();
            if (oGrabar != null)
                {
                var (success, message) = await DOP.Datos.DatosWeb.ProcesarTareaMaestroAsync(oGrabar);

                if (success)
                    {
                    MessageBox.Show("Tareas guardadas correctamente.\n" + message, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                else
                    {
                    MessageBox.Show("Error al guardar tareas:\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            else
                {
                MessageBox.Show("Error al procesar la solicitud de grabación.");
                }
            }
         

        private async void Obtener_Click(object sender, RoutedEventArgs e)
            {
            // Obtén el usuarioID desde donde corresponda en tu aplicación
            int usuarioID = App.IdUsuario; // Usa el ID del login 

            var (success, message, conceptos, relaciones) = await DOP.Datos.DatosWeb.ObtenerConceptosYRelacionesMaestroAsync(usuarioID);

            if (success)
                {
                // Actualiza el objeto Maestro con los datos obtenidos
                Objeto = new Maestro(conceptos, relaciones, usuarioID);

                // Si tienes un árbol o grilla, actualízalo
                grillaTareas.ItemsSource = Objeto.Arbol;

                MessageBox.Show("Datos de tareas maestro obtenidos correctamente.");
                }
            else
                {
                MessageBox.Show($"Error al obtener datos: {message}");
                }
            }
        }
    }
