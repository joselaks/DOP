using System.Windows;
using System.Windows.Controls;

namespace DataObra.Interfaz.Controles
{
    public partial class UcNavegador : UserControl
    {
        public UcNavegador()
        {
            InitializeComponent();
            CrearBotones();
        }

        private void CrearBotones()
        {
            for (int i = 0; i < 5; i++)
            {
                RadioButton radioButton = new RadioButton
                {
                    Width = 50,
                    Height = 35,
                    Content = i + 1,
                    Margin = new Thickness(5),
                    Style = (Style)FindResource("RoundedRadioButtonStyle")
                };
                radioButton.Checked += RadioButton_Checked;
                items.Children.Add(radioButton);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            MessageBox.Show($"RadioButton {radioButton.Content} seleccionado");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
        }
    }
}








