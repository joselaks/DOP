using Bibioteca.Clases;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
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
    /// Lógica de interacción para UcPlanilla.xaml
    /// </summary>
    public partial class UcPlanilla : UserControl
    {
        public Presupuesto Objeto;
        public UcPlanilla(Presupuesto objeto)
        {
            InitializeComponent();
            this.grillaArbol.ItemsSource = objeto.Arbol;
            this.grillaArbol.ChildPropertyName = "Inferiores";
            this.grillaArbol.Loaded += GrillaArbol_Loaded;
        }

        private void GrillaArbol_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.grillaArbol.View != null)
            {
                this.grillaArbol.View.Filter = FiltrarPorTipo;
                this.grillaArbol.View.Refresh();
            }
        }

        private bool FiltrarPorTipo(object item)
        {
            if (item is Nodo nodo)
            {
                return nodo.Tipo == "R" || nodo.Tipo == "T";
            }
            return false;
        }

        private void grillaArbol_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
        {

        }

        private void grillaArbol_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {

        }

        private void grillaArbol_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
