using Bibioteca.Clases;
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
            Objeto.RecalculoFinalizado += Presupuesto_RecalculoFinalizado;



            this.grillaListados.ItemsSource = Objeto.Insumos;
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

            }

        private void grillaListados_KeyDown(object sender, KeyEventArgs e)
            {

            }

        private void grillaListados_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
            {
            // Guardar el alto actual antes de ocultar
            _panSuperioresHeight = panSuperiores.Height;

            sepSuperiores.Height = new GridLength(0); ;
            panSuperiores.Height = new GridLength(0); ;

            }


        public void CambiarFiltroPorTexto(string texto)
            {
            // Buscar el ComboBoxItem cuyo Content coincide con el texto
            foreach (var obj in comboTipoListado.Items)
                {
                if (obj is ComboBoxItem item && item.Content?.ToString() == texto)
                    {
                    comboTipoListado.SelectedItem = item;
                    break;
                    }
                }
            // El evento SelectionChanged ya actualizará la grilla y el TextBlock
            }




        private void comboTipoListado_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
            if (comboTipoListado == null || Objeto == null)
                return;

            ObservableCollection<Nodo> filtrados = null;

            if (comboTipoListado.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
                {
                string seleccion = selectedItem.Content.ToString();

                switch (seleccion)
                    {
                    case "Insumos básicos":
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
                    case "Tareas":
                        filtrados = Objeto.Tareas ?? new ObservableCollection<Nodo>();
                        break;
                    case "Rubros":
                        filtrados = Objeto.Rubros ?? new ObservableCollection<Nodo>();
                        break;
                    default:
                        filtrados = null;
                        break;
                    }
                SeleccionInsumo.Text = seleccion;

                grillaListados.ItemsSource = filtrados;

                // Calcular el total solo de los elementos filtrados
                decimal totGeneral1 = filtrados?.Sum(i => i.Importe1) ?? 0;
                var cultura = new CultureInfo("es-ES") { NumberFormat = { NumberGroupSeparator = ".", NumberDecimalSeparator = "," } };
                colImporte1.HeaderText = $"{totGeneral1.ToString("N2", cultura)}";
                }
            }

        private void VerSuperiores_Click(object sender, RoutedEventArgs e)
            {
            // Obtiene el nodo seleccionado
            var nodo = grillaListados.SelectedItem as Nodo;
            if (nodo != null)
                {
                string id = nodo.ID;
                // Aquí puedes mostrar los superiores o realizar la acción deseada
                sepSuperiores.Height = GridLength.Auto;
                panSuperiores.Height = new GridLength(300);

                // Lógica adicional para mostrar los superiores...
                }
            }


        }



    }

