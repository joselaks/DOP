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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using Syncfusion.Windows.Tools.Controls;
using DataObra.Interfaz.Controles;

namespace DataObra.Interfaz.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiPrincipal.xaml
    /// </summary>
    public partial class WiPrincipal : Window, INotifyPropertyChanged
    {
        private SolidColorBrush _primaryColorBrush;
        private double _previousLeft;
        private double _previousTop;
        private double _previousWidth;
        private double _previousHeight;
        private bool _isCustomMaximized = false;

        public SolidColorBrush PrimaryColorBrush
        {
            get { return _primaryColorBrush; }
            set
            {
                _primaryColorBrush = value;
                OnPropertyChanged(nameof(PrimaryColorBrush));
            }
        }

        string Rol;

        public WiPrincipal(string hexColor, string rol)
        {
            InitializeComponent();
            Rol = rol;
            DataContext = this;
            PrimaryColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
            Maximize_Click(null, null);
            //Cargo lso controles parametrizados con el rol
            UcNavegador ucNavegador = new UcNavegador(rol);
            contenidoDocumentos.Children.Add(ucNavegador);
            UcAgrupador ucAgrupador = new UcAgrupador(rol);
            contenidoAgrupadores.Children.Add(ucAgrupador);
            UcInsumos ucInsumos = new UcInsumos(rol);
            contenidoInsumos.Children.Add(ucInsumos);
            UcInformes ucInformes = new UcInformes(rol);
            contenidoInformes.Children.Add(ucInformes);
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

        public void Boton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonName = button.Name;
                MessageBox.Show($"El nombre del botón es: {buttonName}");
            }
            else
            {
                MessageBox.Show("El evento no fue disparado por un botón.");
            }
        }

        private void hRol_Click(object sender, RoutedEventArgs e)
        {
            // Preguntar al usuario si quiere ejecutar la orden
            var result = MessageBox.Show("¿Está seguro de que quiere reingresar al sistema con un nuevo rol?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Reiniciar la aplicación
                var currentProcess = Process.GetCurrentProcess();
                Process.Start(currentProcess.MainModule.FileName);
                Application.Current.Shutdown();
            }
        }
    }
}

