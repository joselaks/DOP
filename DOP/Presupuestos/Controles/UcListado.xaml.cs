using Bibioteca.Clases;
using DOP.Presupuestos.Clases;

using Syncfusion.Licensing;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows.Controls.RichTextBoxAdv;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace DOP.Presupuestos.Controles
    {
    /// <summary>
    /// Lógica de interacción para UcListado.xaml
    /// </summary>
    public partial class UcListado : UserControl
        {
        public Presupuesto Objeto;
        private GridLength? _panSuperioresHeight = null;

        public UcListado(Presupuesto objeto)
            {
            InitializeComponent();
            Objeto = objeto;
            grillaListados.Loaded += GrillaListados_Loaded;
            this.grillaListados.RowDragDropController.DragStart += grillaListados_DragStart;
            Objeto.RecalculoFinalizado += Presupuesto_RecalculoFinalizado;

            this.grillaListados.ItemsSource = Objeto.Insumos;
            }

        private void grillaListados_DragStart(object? sender, TreeGridRowDragStartEventArgs e)
            {
            DragDropContext.DragSourceUserControl = GetParentUserControl(this.grillaListados);
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



        private void Presupuesto_RecalculoFinalizado(object sender, EventArgs e)
            {
            decimal totGeneral1 = Objeto.Arbol.Sum(i => i.Importe1);

            // Asignar el valor explícitamente al HeaderText
            var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
            colImporte1.HeaderText = $"{totGeneral1.ToString("N2", cultura)}";

            // Ordenar la grilla por Importe1 descendente
            grillaListados.SortColumnDescriptions.Clear();
            grillaListados.SortColumnDescriptions.Add(new Syncfusion.UI.Xaml.Grid.SortColumnDescription()
                {
                ColumnName = "Importe1",
                SortDirection = System.ComponentModel.ListSortDirection.Descending
                });
            }

        private void GrillaListados_Loaded(object sender, RoutedEventArgs e)
            {
            }

        private void grillaListados_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
            {
            }

       
        private void grillaListados_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
            {
            var editado = grillaListados.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex).Item as Nodo;
            if (editado != null)
                {
                Objeto.cambioDesdeInsumo(Objeto.Arbol, editado.ID, editado.PU1);
                Objeto.RecalculoCompleto();
                }
            }
        private void grillaListados_KeyDown(object sender, KeyEventArgs e)
            {

            }

        private void grillaListados_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
            {
            // Guardar el alto actual antes de ocultar
            _panSuperioresHeight = panSuperiores.Height;

            sepSuperiores.Height = new GridLength(0);
            panSuperiores.Height = new GridLength(0);
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

            if (SeleccionInsumo != null)
                SeleccionInsumo.Text = _seleccion;

            grillaListados.ItemsSource = filtrados;

            // Calcular el total solo de los elementos filtrados
            decimal totGeneral1 = filtrados?.Sum(i => i.Importe1) ?? 0;
            var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
            if (colImporte1 != null)
                colImporte1.HeaderText = $"{totGeneral1.ToString("N2", cultura)}";
        }


        // Obtiene todos los superiores tipo "T" en todas las ramas
        private ObservableCollection<Nodo> ObtenerSuperioresTipoT(Nodo nodo)
            {
            var superiores = new ObservableCollection<Nodo>();
            var visitados = new HashSet<string>();
            BuscarSuperioresTipoTRec(Objeto.Arbol, nodo, superiores, visitados);
            return superiores;
            }

        // Búsqueda recursiva de todos los padres tipo "T"
        private void BuscarSuperioresTipoTRec(IEnumerable<Nodo> arbol, Nodo hijo, ObservableCollection<Nodo> superiores, HashSet<string> visitados)
            {
            foreach (var nodo in arbol)
                {
                if (nodo.Inferiores != null && nodo.Inferiores.Any(inf => inf.ID == hijo.ID))
                    {
                    // Evitar ciclos
                    if (!visitados.Contains(nodo.ID))
                        {
                        visitados.Add(nodo.ID);

                        if (nodo.Tipo == "T")
                            superiores.Add(nodo);

                        // Buscar recursivamente hacia arriba
                        BuscarSuperioresTipoTRec(Objeto.Arbol, nodo, superiores, visitados);
                        }
                    }

                if (nodo.Inferiores != null)
                    {
                    BuscarSuperioresTipoTRec(nodo.Inferiores, hijo, superiores, visitados);
                    }
                }
            }

        private void VerSuperiores_Click(object sender, RoutedEventArgs e)
            {
            var nodo = grillaListados.SelectedItem as Nodo;
            if (nodo != null)
                {
                // Mostrar panel de superiores
                gridPrincipal.RowDefinitions[2].Height = GridLength.Auto;
                gridPrincipal.RowDefinitions[3].Height = new GridLength(300);

                // Obtener y mostrar la colección de superiores tipo "T"
                var superioresT = ObtenerSuperioresTipoT(nodo);
                gridSuperiores.ItemsSource = superioresT;
                }
            }

        
    }
    }
