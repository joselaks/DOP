using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using DataObra.Datos;
using Microsoft.Extensions.DependencyInjection;

namespace DataObra
{
    public partial class App : Application
    {
        public static HttpQueueManager QueueManager { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public HttpClient HttpClient { get; private set; }
        public static string BaseUrl { get; private set; }

        public static int IdUsuario { get; set; }

        public static RoutedCommand OpenConectoresCommand = new RoutedCommand();

        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NMaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWX5fdnVWRWFfVENwXEI=");

            // Configurar HttpClient
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            var servicios = serviceCollection.BuildServiceProvider();
            var httpClientFactory = servicios.GetRequiredService<IHttpClientFactory>();
            HttpClient = httpClientFactory.CreateClient();

            // Agregar el CommandBinding
            CommandManager.RegisterClassCommandBinding(typeof(App), new CommandBinding(OpenConectoresCommand, OpenConectores));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Aquí puedes decidir cuál URL usar, por ejemplo, basado en una configuración

            // cambiar cuando pase a local
            // BaseUrl = "http://localhost:5000/";
            // cambiar cuando pase a producción
            BaseUrl = "https://servidordataobra.azurewebsites.net/";

            // Inicializa QueueManager con HttpClient
            QueueManager = new HttpQueueManager(HttpClient);

            // Registrar el evento de teclado
            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown));
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

