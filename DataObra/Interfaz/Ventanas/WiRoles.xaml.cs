using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DataObra.Interfaz.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiRoles.xaml
    /// </summary>
    public partial class WiRoles : Window
    {
        public WiRoles()
        {
            InitializeComponent();
        }

        private void Boton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton)
            {
                // Obtener el color de fondo del botón
                var backgroundBrush = boton.Background as SolidColorBrush;
                var backgroundColor = backgroundBrush?.Color;

                // Convertir el color a formato HEX
                string hexColor = backgroundColor.HasValue ? backgroundColor.Value.ToString() : "#005CA2";

                // Mostrar el color de fondo en un MessageBox (opcional)
                //MessageBox.Show($"El color de fondo del botón es: {hexColor}");

                // Iniciar la animación de desvanecimiento
                var fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                fadeOutAnimation.Completed += (s, a) =>
                {
                    this.Close();

                    // Abrir la ventana WiPrincipal después de cerrar WiRoles
                    WiPrincipal principalWindow = new WiPrincipal(hexColor, boton.Name);
                    principalWindow.Show();

                    // Iniciar la animación de desvanecimiento en WiInicio
                    var fadeOutAnimationInicio = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                    fadeOutAnimationInicio.Completed += (s2, a2) =>
                    {
                        // Cerrar la ventana WiInicio después de la animación
                        Application.Current.MainWindow.Close();
                    };
                    Application.Current.MainWindow.BeginAnimation(Window.OpacityProperty, fadeOutAnimationInicio);
                };
                this.BeginAnimation(Window.OpacityProperty, fadeOutAnimation);
            }
        }
    }
}








