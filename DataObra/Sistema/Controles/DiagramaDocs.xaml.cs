using System.Windows;
using System.Windows.Controls;

namespace DataObra.Base.Controles
{
    /// <summary>
    /// Lógica de interacción para DiagramaDocs.xaml
    /// </summary>
    public partial class DiagramaDocs : UserControl
    {
        public DiagramaDocs()
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
