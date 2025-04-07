using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DataObra.Interfaz.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiDocumento.xaml
    /// </summary>
    public partial class WiDocumento : Window, INotifyPropertyChanged
    {
        private double _previousLeft;
        private double _previousTop;
        private double _previousWidth;
        private double _previousHeight;
        private bool _isCustomMaximized = false;
        public bool GuardadoConExito { get; private set; } = false;

        public WiDocumento(string TipoDoc, UserControl userControl)
        {
            InitializeComponent();
            TituloVentana.Text = TipoDoc;
            espacioPrincipal.Children.Add(userControl);

            this.Closed += (s, e) =>
            {
                if (userControl is Documentos.MaxDocumento maxDoc)
                {
                    GuardadoConExito = maxDoc.GuardadoConExito;
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (_isCustomMaximized)
            {
                // Restaurar el tamaño, la posición y la sombra anteriores
                WindowState = WindowState.Normal;
                Left = _previousLeft;
                Top = _previousTop;
                Width = _previousWidth;
                Height = _previousHeight;
                MainBorder.Margin = new Thickness(10);
                WindowShadow.Opacity = 0.5;
                _isCustomMaximized = false;
            }
            else
            {
                // Almacenar el tamaño y la posición actuales
                _previousLeft = Left;
                _previousTop = Top;
                _previousWidth = Width;
                _previousHeight = Height;

                // Maximizar la ventana y eliminar la sombra
                var screen = System.Windows.SystemParameters.WorkArea;
                Left = screen.Left;
                Top = screen.Top;
                Width = screen.Width;
                Height = screen.Height;
                MainBorder.Margin = new Thickness(0);
                WindowShadow.Opacity = 0;
                _isCustomMaximized = true;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Maximize_Click(sender, e);
            }
            else
            {
                DragMove();
            }
        }

        private void EspacioEstado_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double newHeight = espacioEstado.ActualHeight == 100 ? 30 : 100;

            // Crear animación para la altura de la grilla
            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = espacioEstado.ActualHeight,
                To = newHeight,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            // Iniciar la animación
            espacioEstado.BeginAnimation(HeightProperty, heightAnimation);


            if (espacioEstado.Height == 100)
            {
                Estado.Text = "Documentos relacionados";
            }
            else
            {
                Estado.Text = "No hay documentos relacionados";
            }
        }
    }
}


