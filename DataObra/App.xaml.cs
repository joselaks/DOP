using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using DataObra.Datos;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        private void OpenConectores(object sender, ExecutedRoutedEventArgs e)
        {
            var conectoresWindow = new Conectores();
            conectoresWindow.Show();
        }

        private void testPresupuesto(object sender, ExecutedRoutedEventArgs e)
        {
            UserControl presup = new DataObra.Presupuestos.UcPresupuesto(null);
            DataObra.Interfaz.Ventanas.WiDocumento ventanaPres = new DataObra.Interfaz.Ventanas.WiDocumento("Presupuesto", presup);
            ventanaPres.ShowDialog();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                OpenConectores(this, null);

            }
            if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                testPresupuesto(this, null);
            }
        }
    }
}

