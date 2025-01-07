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
            ConfiguraGrilla();
            LlenaGrilla();
           
        }

        private void LlenaGrilla()
        {
            
        }

        private void ConfiguraGrilla()
        {
            grilla.Columns.Add(new TreeGridTextColumn() { MappingName = "Tipo", HeaderText = "Tipo de documento" });
            grilla.Columns.Add(new TreeGridTextColumn() { MappingName = "Nombre", HeaderText = "Nombre" });
            grilla.Columns.Add(new TreeGridTextColumn() { MappingName = "ID", HeaderText = "ID" });
            grilla.Columns.Add(new TreeGridDateTimeColumn() { MappingName = "Fecha" });
            grilla.Columns.Add(new TreeGridNumericColumn() { MappingName = "Importe" });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
