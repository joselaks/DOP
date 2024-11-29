using System.Windows;
using System.Windows.Controls;

namespace DataObra.Documentos.Controles
{
    public partial class PresDetalle : UserControl
    {
        public PresDetalle()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ListadoRadioButton.IsChecked == true)
            {
                // Acción para la opción "Listado"
            }
            else if (AnalisisRadioButton.IsChecked == true)
            {
                // Acción para la opción "Análisis"
            }
        }
    }
}
