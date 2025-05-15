using DataObra.Datos;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
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
            // Asegura la creación del archivo y la tabla Usuario
            var generador = new DataObra.Datos.GeneraArchivoLocal("Config.precosto");

            InitializeComponent();

            var (usuario, password) = LeerUsuarioLocal();
            if (!string.IsNullOrWhiteSpace(usuario) && !string.IsNullOrWhiteSpace(password))
            {
                txtUsuario.Text = usuario;
                txtContraseña.Password = password;
            }
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

            int maxRetries = 3;
            int delay = 2000; // 2 segundos

            for (int i = 0; i < maxRetries; i++)
            {
                DateTime startTime = DateTime.Now;
                var respuesta = await DatosWeb.ValidarUsuarioAsync(txtUsuario.Text, txtContraseña.Password);
                DateTime endTime = DateTime.Now;
                TimeSpan duration = endTime - startTime;

                if (respuesta.Success)
                {
                    if (respuesta.Usuario.DatosUsuario != null)
                    {
                        // Guardar usuario y contraseña localmente si no existen
                        var (usuarioLocal, passwordLocal) = LeerUsuarioLocal();
                        GuardarUsuarioLocal(txtUsuario.Text, txtContraseña.Password);

                        this.DialogResult = true;
                        this.Close();

                        if (respuesta.Usuario.DatosUsuario.Active==false)
                        {
                            // Abrir la ventana WiRoles después de cerrar WiLogin
                            WiRoles rolesWindow = new WiRoles(Inicio)
                            {
                                Owner = Inicio,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner
                            };
                            rolesWindow.ShowDialog();

                        }
                        else
                        {
                            WiTableroDOP tablero = new WiTableroDOP()
                            {
                                Owner = Inicio,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner
                            };

                            // Detectar la resolución de pantalla principal
                            var screenWidth = SystemParameters.PrimaryScreenWidth;
                            var screenHeight = SystemParameters.PrimaryScreenHeight;

                            if (screenWidth <= 1920 && screenHeight <= 1080)
                            {
                                // Maximizar si la resolución es igual o menor a 1920x1080
                                tablero.WindowState = WindowState.Maximized;
                            }
                            else
                            {
                                // Centrar y establecer tamaño fijo si la resolución es mayor
                                tablero.Width = 1900;
                                tablero.Height = 1000;
                                tablero.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                tablero.WindowState = WindowState.Normal;
                            }

                            tablero.ShowDialog();

                        }

                        return;
                    }
                    else
                    {
                        this.esperaLogin.IsBusy = false;
                        MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    if (i == maxRetries - 1)
                    {
                        this.esperaLogin.IsBusy = false;
                        var result = MessageBox.Show($"\n{respuesta.Message}\n¿Desea intentar nuevamente?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (result == MessageBoxResult.Yes)
                        {
                            VerificaUsuario_Click(sender, e);
                        }
                        return;
                    }
                }

                await Task.Delay(delay);
            }
        }

        private (string usuario, string password) LeerUsuarioLocal()
        {
            string usuario = string.Empty;
            string password = string.Empty;
            string path = Directory.GetCurrentDirectory();
            string archivo = System.IO.Path.Combine(path, "Config.precosto");
            string cadenaConexion = $"Data Source={archivo}";

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();

                // Asegura que la tabla exista antes de leer
                var createCmd = connection.CreateCommand();
                createCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Usuario (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                Password TEXT NOT NULL
            );";
                createCmd.ExecuteNonQuery();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Nombre, Password FROM Usuario LIMIT 1";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = reader.GetString(0);
                        password = reader.GetString(1);
                    }
                }
            }
            return (usuario, password);
        }




        private void GuardarUsuarioLocal(string usuario, string password)
        {
            string path = Directory.GetCurrentDirectory();
            string archivo = System.IO.Path.Combine(path, "Config.precosto");
            string cadenaConexion = $"Data Source={archivo}";

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();

                // Asegura que la tabla exista antes de guardar
                var createCmd = connection.CreateCommand();
                createCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Usuario (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                Password TEXT NOT NULL
            );";
                createCmd.ExecuteNonQuery();

                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Usuario";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Usuario (Nombre, Password) VALUES (@nombre, @password)";
                command.Parameters.AddWithValue("@nombre", usuario);
                command.Parameters.AddWithValue("@password", password);
                command.ExecuteNonQuery();
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





