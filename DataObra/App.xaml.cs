using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using DataObra.Datos;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace DataObra
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public HttpClient HttpClient { get; private set; }
        public static string BaseUrl { get; private set; }

        public static int IdUsuario { get; set; }

        public static int IdCuenta { get; set; }

        public static RoutedCommand OpenConectoresCommand = new RoutedCommand();

        public static List<Agrupadores.Agrupador>? ListaAgrupadores;

        // Propiedad estática para almacenar el estado de la conexión
        public static bool IsConnectionSuccessful { get; private set; }

        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NMaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWX5fdnVWRWFfVENwXEI=");

            // Configurar HttpClient
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient("default", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30); // Establecer un tiempo de espera adecuado
                client.DefaultRequestHeaders.ConnectionClose = false; // Mantener viva la conexión
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                UseCookies = true,
                AllowAutoRedirect = true,
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });

            var servicios = serviceCollection.BuildServiceProvider();
            var httpClientFactory = servicios.GetRequiredService<IHttpClientFactory>();
            HttpClient = httpClientFactory.CreateClient("default");

            IdCuenta = 1;

            // Agregar el CommandBinding
            CommandManager.RegisterClassCommandBinding(typeof(App), new CommandBinding(OpenConectoresCommand, OpenConectores));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Aquí puedes decidir cuál URL usar, por ejemplo, basado en una configuración

            // cambiar cuando pase a local
            // BaseUrl = "https://localhost:5000/";
            // cambiar cuando pase a producción
            BaseUrl = "https://servidordataobra.azurewebsites.net/";

            // Registrar el evento de teclado
            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown));
        }

        public static async Task<bool> TryConnect()
        {
            while (true)
            {
                DateTime sendTime = DateTime.Now;
                DatosWeb.LogEntries.Add($"{sendTime:yyyy-MM-dd HH:mm:ss.fff} - Enviado - {BaseUrl}health");

                try
                {
                    var response = await ((App)Application.Current).HttpClient.GetAsync($"{BaseUrl}health");
                    var responseContent = await response.Content.ReadAsStringAsync(); // Leer el contenido de la respuesta

                    DateTime receiveTime = DateTime.Now;
                    TimeSpan duration = receiveTime - sendTime;
                    double elapsedSeconds = duration.TotalSeconds;

                    if (response.IsSuccessStatusCode)
                    {
                        // La solicitud fue exitosa, la conexión está precalentada
                        IsConnectionSuccessful = true;
                        DatosWeb.LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - {responseContent} - Tiempo transcurrido: {elapsedSeconds:F3} segundos");
                        return true;
                    }
                    else
                    {
                        // La solicitud no fue exitosa
                        IsConnectionSuccessful = false;
                        Console.WriteLine("Conexion no exitosa");
                        DatosWeb.LogEntries.Add($"{receiveTime:yyyy-MM-dd HH:mm:ss.fff} - Recibido - {responseContent} - Tiempo transcurrido: {elapsedSeconds:F3} segundos");
                    }
                }
                catch (Exception ex)
                {
                    // Manejar cualquier error que ocurra durante la solicitud
                    DateTime errorTime = DateTime.Now;
                    TimeSpan errorDuration = errorTime - sendTime;
                    double errorElapsedSeconds = errorDuration.TotalSeconds;

                    IsConnectionSuccessful = false;
                    Console.WriteLine($"Error al precalentar la conexión: {ex.Message}");
                    DatosWeb.LogEntries.Add($"{errorTime:yyyy-MM-dd HH:mm:ss.fff} - Error - {ex.Message} - Tiempo transcurrido: {errorElapsedSeconds:F3} segundos");
                }

                // Esperar antes de reintentar
                await Task.Delay(2000); // Esperar 2 segundos antes de reintentar
            }
        }



        private void OpenConectores(object sender, ExecutedRoutedEventArgs e)
        {
            var conectoresWindow = new Conectores();
            conectoresWindow.Show();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                OpenConectores(this, null);
            }
        }
    }
}

