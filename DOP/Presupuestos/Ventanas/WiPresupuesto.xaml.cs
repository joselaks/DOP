using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DOP.Presupuestos.Controles;
using Bibioteca.Clases;
using DOP.Presupuestos.Clases;

namespace DOP.Presupuestos.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiPresupuesto.xaml
    /// </summary>
    public partial class WiPresupuesto : RibbonWindow
    {
        public Presupuesto Objeto;
        public cUndoRedo UndoRedo;
        public UcPlanilla Planilla;
        public WiPresupuesto()
        {
            InitializeComponent();
            this.gPlanilla.Children.Add(Planilla = new UcPlanilla());

        }

        private void Fiebdc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFiebdc_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
