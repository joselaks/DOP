using DataObra.Presupuestos.Controles.SubControles;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools.Controls;
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
using System.Windows.Shapes;

namespace DataObra.Presupuestos.Ventanas
    {
    /// <summary>
    /// Lógica de interacción para WiPres.xaml
    /// </summary>
    public partial class WiPres : RibbonWindow
        {

        private ContenedorPresupuesto _contenedor;

        public WiPres()
            {
            InitializeComponent();

            _contenedor = new ContenedorPresupuesto();
            this.gridPres.Children.Add(_contenedor);
            }
        }
    }
