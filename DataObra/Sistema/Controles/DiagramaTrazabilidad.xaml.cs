using System.Windows;
using System.Windows.Controls;

namespace DataObra.Base.Controles
{
    public partial class DiagramaTrazabilidad : UserControl
    {
        public DiagramaTrazabilidad()
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
