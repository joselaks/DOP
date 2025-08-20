using DOP.Presupuestos.Ventanas;
using Microsoft.Data.Sqlite;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Net.NetworkInformation;

namespace DOP.Interfaz.Ventanas
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
            var generador = new DOP.Datos.GeneraArchivoLocal("Config.precosto");

            InitializeComponent();

            var (usuario, password) = LeerUsuarioLocal();
            if (!string.IsNullOrWhiteSpace(usuario) && !string.IsNullOrWhiteSpace(password))
                {
                txtUsuario.Text = usuario;
                txtContraseña.Password = password;
                }
            Inicio = inicio;
            this.txtVersion.Text = "Versión 0.2005.6";
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

            // Obtener la MAC Address de la primera interfaz activa
            string macaddress = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up &&
                              nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault() ?? "UNKNOWN";

            int maxRetries = 3;
            int intentos = 0;

            while (intentos < maxRetries)
                {
                DateTime startTime = DateTime.Now;
                var respuesta = await DOP.Datos.DatosWeb.ValidarUsuarioAsync(
                    txtUsuario.Text,
                    txtContraseña.Password,
                    macaddress
                );
                DateTime endTime = DateTime.Now;
                TimeSpan duration = endTime - startTime;

                if (respuesta.Success)
                    {
                    if (respuesta.Usuario.DatosUsuario != null)
                        {
                        // Guardar usuario y contraseña localmente si no existen
                        var (usuarioLocal, passwordLocal) = LeerUsuarioLocal();
                        GuardarUsuarioLocal(txtUsuario.Text, txtContraseña.Password);
                        App.IdUsuario = respuesta.Usuario.DatosUsuario.ID;
                        App.NombreUsuario = $"{respuesta.Usuario.DatosUsuario.Nombre} {respuesta.Usuario.DatosUsuario.Apellido}";
                        if (respuesta.Usuario.DatosUsuario.ID == 1 || respuesta.Usuario.DatosUsuario.ID == 2)
                            {
                            App.tipoUsuario = 1; //Administrador
                            }
                        else if (respuesta.Usuario.DatosUsuario.ID == 4)
                            {
                            App.tipoUsuario = 2; // Contenido
                            }
                        else
                            {
                            App.tipoUsuario = 3; // Usuario normal
                            }
                        this.DialogResult = true;
                        this.Close();

                        // Crea y asigna la nueva ventana principal
                        WiTablero tablero = new WiTablero()
                            {
                            WindowStartupLocation = WindowStartupLocation.CenterScreen
                            };
                        Application.Current.MainWindow = tablero;
                        tablero.Show();

                        // Cierra la ventana de inicio
                        Inicio.Close();

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
                    // Solo reintenta si es un error de comunicación (no de usuario/contraseña)
                    if (respuesta.Message.StartsWith("Error HTTP:") || respuesta.Message.StartsWith("Error de comunicación:"))
                        {
                        intentos++;
                        if (intentos == maxRetries)
                            {
                            this.esperaLogin.IsBusy = false;
                            var result = MessageBox.Show($"\n{respuesta.Message}\n¿Desea intentar nuevamente?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                            if (result == MessageBoxResult.Yes)
                                {
                                VerificaUsuario_Click(sender, e);
                                }
                            return;
                            }
                        // Si no es el último intento, vuelve a intentar automáticamente
                        }
                    else
                        {
                        // Es un error de usuario/contraseña, no reintentar
                        this.esperaLogin.IsBusy = false;
                        MessageBox.Show(respuesta.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                        }
                    }
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

        private async void bSalir_Click(object sender, RoutedEventArgs e)
            {
            // Obtener la MAC Address de la primera interfaz activa
            string macaddress = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up &&
                              nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault() ?? "UNKNOWN";

            // Registrar la salida en el log (si el usuario está logueado)
            if (App.IdUsuario > 0)
                {
                await DOP.Datos.DatosWeb.RegistrarSalidaUsuarioAsync(App.IdUsuario, macaddress);
                }

            Application.Current.Shutdown();
            }
        }
    }
