using Syncfusion.SfSkinManager;
using Syncfusion.UI.Xaml.TreeGrid;
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

namespace DataObra.Interfaz.Controles.SubControles
{
    /// <summary>
    /// Lógica de interacción para UsGrilla.xaml
    /// </summary>
    public partial class UsGrilla : UserControl
    {
        public UsGrilla()
        {
            SfSkinManager.SetTheme(this, new Theme("MaterialLight", new string[] { "ToolBarAdv", "SfTreeGrid" }));
            InitializeComponent();
        }
    }
}
