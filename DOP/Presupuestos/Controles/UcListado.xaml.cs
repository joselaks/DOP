using Bibioteca.Clases;
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
        }
}
