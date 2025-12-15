using Bibioteca.Clases;
using Biblioteca;
using Biblioteca.DTO;
using DataObra.Interfaz.Ventanas;
using DOP.Presupuestos.Clases;
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
    /// Lógica de interacción para cMaestro.xaml
    /// </summary>
    public partial class UcMaestro : UserControl
        {

        public Presupuesto Objeto;
        private string tipoSeleccionado = null;
        private GridLength? _panSuperioresHeight = null;
        private List<ConceptoDTO> Conceptos;



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

            // Obtiene el presupuesto maestro
            var (ok, msg, conceptos, relaciones) = await DOP.Datos.DatosWeb.ObtenerConceptosYRelacionesAsync(45);
            Conceptos = conceptos; // Guarda los conceptos para usarlos después
            var PresupuestoDTO = new PresupuestoDTO
                {
                ID = 45
                };
            Objeto = new Presupuesto(PresupuestoDTO, conceptos, relaciones);
            grillaMaestro.ItemsSource = Objeto.Arbol;
            // Obtener todos los nodos tipo "T" y "R"
            var Filtrado = new ObservableCollection<Nodo>(ObtenerNodosPorTipos(Objeto.Arbol, "T"));
            this.grillaMaestro.ItemsSource = Filtrado;
            this.grillaMaestro.View.Refresh();

            // Selecciona el primer item ("Tareas") del ComboBox
            comboTipoListado.SelectedIndex = 0;
            }



        private async void ObtenerModelo(int _ID)
            {
            int idpres = _ID;
            // Obtiene el presupuesto maestro
            var (ok, msg, conceptos, relaciones) = await DOP.Datos.DatosWeb.ObtenerConceptosYRelacionesAsync(idpres);
            Conceptos = conceptos; // Guarda los conceptos para usarlos después
            var PresupuestoDTO = new PresupuestoDTO();
            PresupuestoDTO.ID = idpres;
            Objeto = new Presupuesto(PresupuestoDTO, conceptos, relaciones);
            Objeto.RecalculoCompleto();
            TareasCero();
            grillaMaestro.ItemsSource = Objeto.Arbol;
            // Obtener todos los nodos tipo "T" y "R"
            var Filtrado = new ObservableCollection<Nodo>(ObtenerNodosPorTipos(Objeto.Arbol, "T"));
            this.grillaMaestro.ItemsSource = Filtrado;
            this.grillaMaestro.View.Refresh();

            // Selecciona el primer item ("Tareas") del ComboBox
            comboTipoListado.SelectedIndex = 0;
            }

        private void TareasCero()
            {
            if (Objeto == null || Objeto.Arbol == null) return;

            void RecorrerYAnularCantidad(IEnumerable<Nodo> nodos)
                {
                foreach (var nodo in nodos)
                    {
                    if (nodo.Tipo == "T")
                        nodo.Cantidad = 0;

                    if (nodo.Inferiores != null && nodo.Inferiores.Count > 0)
                        RecorrerYAnularCantidad(nodo.Inferiores);
                    }
                }

            RecorrerYAnularCantidad(Objeto.Arbol);
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
            MessageBox.Show("Funcionalidad en implementación");

            }


        private void Button_Click(object sender, RoutedEventArgs e)
            {

            }

        private IEnumerable<Nodo> ObtenerNodosPorTipos(IEnumerable<Nodo> nodos, params string[] tipos)
            {
            foreach (var nodo in nodos)
                {
                if (tipos.Contains(nodo.Tipo))
                    yield return nodo;

                if (nodo.Inferiores != null && nodo.Inferiores.Count > 0)
                    {
                    foreach (var hijo in ObtenerNodosPorTipos(nodo.Inferiores, tipos))
                        yield return hijo;
                    }
                }
            }

        public void FiltrarPorTipoDescripcion(string descripcion)
            {
            var mapTipo = new Dictionary<string, string>
                {
                { "Tareas", "T" },
                { "Materiales", "M" },
                { "Mano de Obra", "D" },
                { "Equipos", "E" },
                { "Subcontratos", "S" },
                { "Otros", "O" },
                { "Auxiliares", "A" }
            // Agrega más si es necesario
            };

            if (!mapTipo.ContainsKey(descripcion))
                return;

            // Validación para evitar NullReferenceException
            if (Objeto == null || Objeto.Arbol == null)
                return;

            string tipo = mapTipo[descripcion];

            ObservableCollection<Nodo> filtrado;
            if (string.IsNullOrEmpty(tipo))
                {
                filtrado = new ObservableCollection<Nodo>(AplanarNodos(Objeto.Arbol));
                }
            else
                {
                filtrado = new ObservableCollection<Nodo>(ObtenerNodosPorTipos(Objeto.Arbol, tipo));
                }

            this.grillaMaestro.ItemsSource = filtrado;
            this.grillaMaestro.View.Refresh();
            }

        // Método auxiliar para aplanar el árbol y mostrar todos los nodos
        private IEnumerable<Nodo> AplanarNodos(IEnumerable<Nodo> nodos)
            {
            foreach (var nodo in nodos)
                {
                yield return nodo;
                if (nodo.Inferiores != null && nodo.Inferiores.Count > 0)
                    {
                    foreach (var hijo in AplanarNodos(nodo.Inferiores))
                        yield return hijo;
                    }
                }
            }

        private void comboTipoListado_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            {
            if (Objeto == null)
                return;

            var combo = sender as ComboBox;
            if (combo == null)
                return;

            var selectedItem = combo.SelectedItem as ComboBoxItem;
            string _seleccion = selectedItem?.Content?.ToString() ?? "Todos";

            ObservableCollection<Nodo> filtrados = null;

            switch (_seleccion)
                {
                case "Rubros":
                    filtrados = Objeto.Arbol;
                    break;
                case "Tareas":
                    filtrados = new ObservableCollection<Nodo>(ObtenerNodosPorTipos(Objeto.Arbol, "T"));
                    break;
                case "Todos":
                    filtrados = Objeto.Insumos ?? new ObservableCollection<Nodo>();
                    break;
                case "Materiales":
                    filtrados = new ObservableCollection<Nodo>(
                        Objeto.Insumos?.Where(x => x.Tipo == "M") ?? Enumerable.Empty<Nodo>());
                    break;
                case "Mano de Obra":
                    filtrados = new ObservableCollection<Nodo>(
                        Objeto.Insumos?.Where(x => x.Tipo == "D") ?? Enumerable.Empty<Nodo>());
                    break;
                case "Equipos":
                    filtrados = new ObservableCollection<Nodo>(
                        Objeto.Insumos?.Where(x => x.Tipo == "E") ?? Enumerable.Empty<Nodo>());
                    break;
                case "Subcontratos":
                    filtrados = new ObservableCollection<Nodo>(
                        Objeto.Insumos?.Where(x => x.Tipo == "S") ?? Enumerable.Empty<Nodo>());
                    break;
                case "Otros":
                    filtrados = new ObservableCollection<Nodo>(
                        Objeto.Insumos?.Where(x => x.Tipo == "O") ?? Enumerable.Empty<Nodo>());
                    break;
                case "Auxiliares":
                    filtrados = Objeto.Auxiliares ?? new ObservableCollection<Nodo>();
                    break;
                default:
                    filtrados = new ObservableCollection<Nodo>();
                    break;
                }

            //if (SeleccionInsumo != null)
            //    SeleccionInsumo.Text = _seleccion;

            grillaMaestro.ItemsSource = filtrados;

            }

        private void comboModelo_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
            // Evitar ejecutar durante la carga o cuando se limpia la selección
            if (comboModelo == null || e.AddedItems == null || e.AddedItems.Count == 0)
                return;

            int id = 0;

            // 1) SelectedValue según SelectedValuePath="ID"
            if (comboModelo.SelectedValue is int v)
                id = v;
            

            if (id > 0)
                ObtenerModelo(id); // async void existente
            }


        private void comboOrigen_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
            // Asegurar que los controles estén inicializados
            if (comboOrigen == null || comboModelo == null)
                return;


            var escritorio = Application.Current.Windows.OfType<WiEscritorio>().FirstOrDefault();
            if (escritorio == null) return;

            var seleccionado = (comboOrigen.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

            if (seleccionado.Equals("DataObra", StringComparison.OrdinalIgnoreCase))
                {
                // Usar modelos públicos
                comboModelo.ItemsSource = escritorio._modelos;
                }
            else if (seleccionado.Equals("Propio", StringComparison.OrdinalIgnoreCase))
                {
                // Usar modelos del usuario actual
                comboModelo.ItemsSource = escritorio._modelosPropios;
                }
            else
                {
                comboModelo.ItemsSource = null;
                }

            // Opcional: limpiar selección previa
            comboModelo.SelectedIndex = -1;
            }

        
        }
    }