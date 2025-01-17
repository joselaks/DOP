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

namespace DataObra.Interfaz.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiPrincipal.xaml
    /// </summary>
    public partial class WiPrincipal : Window, INotifyPropertyChanged
    {
        private SolidColorBrush _primaryColorBrush;

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
            CargaTabs();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CargaTabs()
        {
            Interfaz.Controles.UcNavegador ucNavegador1 = new Controles.UcNavegador(Rol, "Documentos");
            contenidoDocumentos.Children.Add(ucNavegador1);
            Interfaz.Controles.UcNavegador ucNavegador2 = new Controles.UcNavegador(Rol, "Agrupadores");
            contenidoAgrupadores.Children.Add(ucNavegador2);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
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
            MessageBox.Show("accionaBoton");
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









