using System.Windows;
using System.Windows.Controls;

namespace DataObra.Sistema
{
    public partial class SeleccionRol : Window
    {
        public SeleccionRol()
        {
            InitializeComponent();
        }

        private void RedimensionarBotones()
        {
            GridBotones.RowDefinitions.Clear();
            GridBotones.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            GridBotones.ColumnDefinitions.Clear();
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.20, GridUnitType.Star) });
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.20, GridUnitType.Star) });
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.20, GridUnitType.Star) });
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.20, GridUnitType.Star) });
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.20, GridUnitType.Star) });
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.20, GridUnitType.Star) });

            foreach (Button btn in new[] { BotonRestaurar, BotonAdministracion, BotonCompras, BotonPresupuestos, BotonDepositos, BotonSocio })
            {
                btn.Height = 50;
                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.Margin = new Thickness(5);
                Grid.SetRow(btn, 0);
            }

            BotonRestaurar.Visibility = Visibility.Visible;
        }
        private void Boton_Click(object sender, RoutedEventArgs e) 
        {
            RedimensionarBotones();
            
            if (sender is Button botonPresionado)
            {
                string contenido = botonPresionado.Content.ToString();

                AreaContenido.Content = new TextBlock { Text = contenido, FontSize = 24, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; 
            }
        }

        private void BotonRestaurar_Click(object sender, RoutedEventArgs e)
        {
            BotonRestaurar.Visibility = Visibility.Collapsed;

            GridBotones.RowDefinitions.Clear();
            GridBotones.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            GridBotones.ColumnDefinitions.Clear();
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            GridBotones.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            foreach (Button btn in new[] { BotonAdministracion, BotonCompras, BotonPresupuestos, BotonDepositos, BotonSocio })
            {
                btn.Height = double.NaN;
                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                btn.VerticalAlignment = VerticalAlignment.Stretch;
                btn.Margin = new Thickness(0);
                Grid.SetRow(btn, 0);
            }

            AreaContenido.Content = null;
        }
    }
}
