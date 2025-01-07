using Syncfusion.SfSkinManager;
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

namespace DataObra.Interfaz.Controles
{
    /// <summary>
    /// Lógica de interacción para UcPanelx4.xaml
    /// </summary>
    public partial class UcPanelx4 : UserControl
    {
        public UcPanelx4()
        {
            SfSkinManager.SetTheme(this, new Theme("MaterialLight", new string[] { "TabNavigationControl", "TabControlExt" }));
            InitializeComponent();
        }
    }
}
