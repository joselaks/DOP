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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataObra
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para manejar el inicio de sesión
            string usuario = txtUsuario.Text;
            string contraseña = txtContraseña.Password;

            if (usuario == "demo@dataobra.com" && contraseña == "demo")
            {
                // Cambiar la pantalla
                LoginSection.Visibility = Visibility.Collapsed;
                TextBlock userName = new TextBlock
                {
                    Text = "Bienvenido, " + usuario,
                    FontSize = 20,
                    Margin = new Thickness(10)
                };
                MainContent.Children.Add(userName);

                textodemo.Text = "";
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos");
            }
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