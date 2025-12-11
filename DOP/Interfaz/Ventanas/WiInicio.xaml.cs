using System;
using DOP.Datos;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace DOP.Interfaz.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiInicio.xaml
    /// </summary>
    public partial class WiInicio : Window
    {
        public WiEspera ventanaEspera;
        public WiInicio()
        {
            InitializeComponent();
            ventanaEspera = new WiEspera();
            this.Loaded += WiInicio_Loaded;
        }

        private async void WiInicio_Loaded(object sender, RoutedEventArgs e)
            {
            // Mostrar la ventana de espera
            ventanaEspera.Owner = this;
            ventanaEspera.Show();

            // Intentar la conexión
            bool isConnectionSuccessful = await TryConnect();

            // Cerrar la ventana de espera
            ventanaEspera.Close();

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
            }

        private async Task<bool> TryConnect()
            {
            // Verificar la conexión a Internet
            if (!IsInternetAvailable())
                {
                // Cerrar la ventana de espera
                ventanaEspera.Close();
                MessageBox.Show("No hay conexión a Internet. Por favor, verifique su conexión y vuelva a intentarlo.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                //return false;
                }

            int failedAttempts = 0;

            while (true)
                {
                DateTime sendTime = DateTime.Now;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DatosWeb.LogEntries.Add($"{sendTime:yyyy-MM-dd HH:mm:ss.fff} - Enviado - {App.BaseUrl}health");
                });

                try
                    {
                    var response = await ((App)Application.Current).HttpClient.GetAsync($"{App.BaseUrl}health");
                    var responseContent = await response.Content.ReadAsStringAsync(); // Leer el contenido de la respuesta

                    DateTime receiveTime = DateTime.Now;
                    TimeSpan duration = receiveTime - sendTime;
                    double elapsedSeconds = duration.TotalSeconds;

                    if (response.IsSuccessStatusCode)
                        {
                        // La solicitud fue exitosa, la conexión está precalentada
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            DatosWeb.LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - {responseContent} - Tiempo transcurrido: {elapsedSeconds:F3} segundos");
                        });
                        return true;
                        }
                    else
                        {
                        // La solicitud no fue exitosa
                        Console.WriteLine("Conexion no exitosa");
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            DatosWeb.LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - {responseContent} - Tiempo transcurrido: {elapsedSeconds:F3} segundos");
                        });
                        }
                    }
                catch (Exception ex)
                    {
                    // Manejar cualquier error que ocurra durante la solicitud
                    DateTime errorTime = DateTime.Now;
                    TimeSpan errorDuration = errorTime - sendTime;
                    double errorElapsedSeconds = errorDuration.TotalSeconds;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        DatosWeb.LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Error - {ex.Message} - Tiempo transcurrido: {errorElapsedSeconds:F3} segundos");
                    });
                    }

                failedAttempts++;

                if (failedAttempts >= 2)
                    {
                    ventanaEspera.Close();
                    var result = MessageBox.Show("No se pudo establecer la conexión con el servidor después de varios intentos. ¿Desea intentar nuevamente?", "Error de conexión", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (result == MessageBoxResult.Yes)
                        {

                        // Crear una nueva instancia de la ventana de espera
                        ventanaEspera = new WiEspera()
                            {
                            Owner = this
                            };
                        ventanaEspera.Show();

                        failedAttempts = 0; // Reiniciar el contador de intentos fallidos
                        }
                    else
                        {
                        Application.Current.Shutdown();
                        return false;
                        }
                    }

                // Esperar antes de reintentar
                await Task.Delay(2000); // Esperar 2 segundos antes de reintentar
                }
            }

        private static bool IsInternetAvailable()
            {
            try
                {
                using (var ping = new Ping())
                    {
                    var reply = ping.Send("www.google.com");
                    return reply.Status == IPStatus.Success;
                    }
                }
            catch
                {
                return false;
                }
            }



        }
}
