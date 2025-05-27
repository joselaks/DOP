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

namespace DOP.Presupuestos.Controles.SubControles
{
    /// <summary>
    /// Lógica de interacción para UcListaConceptos.xaml
    /// </summary>
    public partial class UcListaConceptos : UserControl
    {
        public Presupuesto Objeto;
        public UcListaConceptos(Presupuesto objeto)
        {
            InitializeComponent();
            Objeto = objeto;
            this.grillaListado.ItemsSource = Objeto.Insumos;
        }

        private void grillaListado_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
        {

        }

        private void grillaListado_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {

        }
    }
}
