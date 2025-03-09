using DataObra.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataObra.Interfaz.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiLogin.xaml
    /// </summary>
    public partial class WiLogin : Window
    {
        public string Usuario { get; private set; }
        public string Rol { get; private set; }

        private WiInicio Inicio;

        public WiLogin(WiInicio inicio)
        {
            InitializeComponent();

            txtUsuario.Text = "jose@dataobra.com";
            txtContraseña.Password = "contra";
            Inicio = inicio;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "Usuario")
            {
                textBox.Text = "";
            }
        }

        private async void VerificaUsuario_Click(object sender, RoutedEventArgs e)
        {
            this.esperaLogin.IsBusy = true;

            // Código a utilizar para la validación
            // En este caso no usa la cola de envios al servidor. Se conecta directamente
            var respuesta = await DatosWeb.ValidarUsuarioAsync(txtUsuario.Text, txtContraseña.Password);

            //var respuesta = await ConsultasAPI.ValidarUsuarioAsync(txtUsuario.Text, txtContraseña.Password);
            if (respuesta.Success)
            {
                if (respuesta.Usuario.DatosUsuario != null)
                {
                    //Existe el usuario. puede verificarse abono, etc

                    this.DialogResult = true;
                    this.Close();

                    // Abrir la ventana WiRoles después de cerrar WiLogin
                    WiRoles rolesWindow = new WiRoles(Inicio)
                    {
                        Owner = Inicio, // Establecer la ventana WiInicio como la propietaria de la ventana de login
                        WindowStartupLocation = WindowStartupLocation.CenterOwner // Centrar la ventana de login en WiInicio
                    };
                    rolesWindow.ShowDialog();
                }
                else
                {
                    this.esperaLogin.IsBusy = false;

                    MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                this.esperaLogin.IsBusy = false;
                var result = MessageBox.Show("\n¿Error de conexión. Desea intentar nuevamente?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                {
                    VerificaUsuario_Click(sender, e);
                }

            }
        }




        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Usuario";
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox.Password == "Contraseña")
            {
                passwordBox.Password = "";
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                passwordBox.Password = "Contraseña";
            }
        }

        private void bSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}





