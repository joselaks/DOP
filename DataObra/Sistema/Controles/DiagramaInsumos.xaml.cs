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

namespace DataObra.Base.Controles
{
    /// <summary>
    /// Lógica de interacción para DiagramaInsumos.xaml
    /// </summary>
    public partial class DiagramaInsumos : UserControl
    {
        public DiagramaInsumos()
        {
            InitializeComponent();
        }

        private void BotonDoc_Click(object sender, RoutedEventArgs e)
        {
            Button sele = sender as Button;

            if (sele != null)
            {
                MessageBox.Show("Edita " + sele.Name.ToString());
            }
        }
    }
}
