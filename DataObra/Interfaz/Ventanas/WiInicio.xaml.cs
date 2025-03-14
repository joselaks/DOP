using System;
using System.Threading.Tasks;
using System.Windows;

namespace DataObra.Interfaz.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiInicio.xaml
    /// </summary>
    public partial class WiInicio : Window
    {
        public WiInicio()
        {
            InitializeComponent();
            this.Loaded += WiInicio_Loaded;
        }

        private async void WiInicio_Loaded(object sender, RoutedEventArgs e)
        {
            // Mostrar la ventana de espera
            WaitWindow waitWindow = new WaitWindow();
            waitWindow.Owner = this;
            waitWindow.Show();

            // Intentar la conexión
            bool isConnectionSuccessful = await Task.Run(() => App.TryConnect());

            // Cerrar la ventana de espera
            waitWindow.Close();

            // Verificar si la conexión fue exitosa antes de mostrar la ventana de login
            if (isConnectionSuccessful)
            {
                // Crear y mostrar la ventana de login
                WiLogin login = new WiLogin(this)
                {
                    Owner = this, // Establecer la ventana WiInicio como la propietaria de la ventana de login
                    WindowStartupLocation = WindowStartupLocation.CenterOwner // Centrar la ventana de login en WiInicio
                };
                login.ShowDialog();
            }
            else
            {
                MessageBox.Show("No se pudo establecer la conexión con el servidor. Por favor, intente nuevamente más tarde.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }
}

