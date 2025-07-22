using Bibioteca.Clases;
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

namespace DOP.Presupuestos.Controles
{
    /// <summary>
    /// Lógica de interacción para UcListado.xaml
    /// </summary>
    public partial class UcListado : UserControl
    {
        public Presupuesto Objeto;
        public UcListado(Presupuesto objeto)
        {
            InitializeComponent();
            Objeto= objeto;
            this.grillaListados.ItemsSource = Objeto.Insumos;
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
            }

        private void comboTipoListado_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
            if (comboTipoListado == null || Objeto == null)
                return;

            if (comboTipoListado.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
                {
                string seleccion = selectedItem.Content.ToString();

                switch (seleccion)
                    {
                    case "Materiales":
                        grillaListados.ItemsSource = new ObservableCollection<Nodo>(
                            Objeto.Insumos?.Where(x => x.Tipo == "M") ?? Enumerable.Empty<Nodo>());
                        break;
                    case "Mano de Obra":
                        grillaListados.ItemsSource = new ObservableCollection<Nodo>(
                            Objeto.Insumos?.Where(x => x.Tipo == "D") ?? Enumerable.Empty<Nodo>());
                        break;
                    case "Equipos":
                        grillaListados.ItemsSource = new ObservableCollection<Nodo>(
                            Objeto.Insumos?.Where(x => x.Tipo == "E") ?? Enumerable.Empty<Nodo>());
                        break;
                    case "Subcontratos":
                        grillaListados.ItemsSource = new ObservableCollection<Nodo>(
                            Objeto.Insumos?.Where(x => x.Tipo == "S") ?? Enumerable.Empty<Nodo>());
                        break;
                    case "Otros":
                        grillaListados.ItemsSource = new ObservableCollection<Nodo>(
                            Objeto.Insumos?.Where(x => x.Tipo == "O") ?? Enumerable.Empty<Nodo>());
                        break;
                    case "Tareas":
                        grillaListados.ItemsSource = Objeto.Tareas ?? new ObservableCollection<Nodo>();
                        break;
                    case "Rubros":
                        grillaListados.ItemsSource = Objeto.Rubros ?? new ObservableCollection<Nodo>();
                        break;
                    default:
                        grillaListados.ItemsSource = null;
                        break;
                    }
                }
            }

        }
}
