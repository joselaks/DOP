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
    /// Lógica de interacción para UcDosaje.xaml
    /// </summary>
    public partial class UcDosaje : UserControl
    {
        public UcDosaje() 
        {
            InitializeComponent();


        }


        public void MostrarInferiores(ObservableCollection<Nodo> inferiores)
        {
            grillaDetalle.ItemsSource = inferiores;

        }

        private void grillaDetalle_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.TreeGrid.TreeGridCurrentCellBeginEditEventArgs e)
            {

            }

        private void grillaDetalle_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
            {

            }

        private void grillaDetalle_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
            {

            }

        private void grillaDetalle_KeyDown(object sender, KeyEventArgs e)
            {

            }
        }
}
