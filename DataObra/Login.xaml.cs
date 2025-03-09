using DataObra.Datos;
using System.Windows.Controls;
using System.Windows;
using Biblioteca;

namespace DataObra
{
    public partial class Login : Window
    {
        public string Usuario { get; private set; }
        public string Rol { get; private set; }


        public Login()
        {
            InitializeComponent();

            txtUsuario.Text = "jose@dataobra.com";
            txtContraseña.Password = "contra";
        }

        private async void VerificaUsuario_Click(object sender, RoutedEventArgs e)
        {
            // Código a utilizar para la validación
            //var respuesta = await ConsultasAPI.ValidarUsuarioAsync(txtUsuario.Text, txtContraseña.Password);

            //if (respuesta.Success && respuesta.Usuario != null)
            //{
            //    Usuario = respuesta.Usuario.Nombre;
            //    Rol = "Compras"; // respuesta.Usuario.Rol;

            //    this.DialogResult = true;
            //    this.Close();
            //}
            //else
            //{
            //    // Demo
            //    Usuario = "Demo";
            //    Rol = "Demo";

            //    this.DialogResult = true;
            //    this.Close();
            //}
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "Usuario")
            {
                textBox.Text = "";
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
    }
}
