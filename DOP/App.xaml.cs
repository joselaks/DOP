using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace DOP
    {
    public record Currency(string Moneda, string Codigo);

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
        {

        public IServiceProvider ServiceProvider { get; private set; }
        public HttpClient HttpClient { get; private set; }
        public static string BaseUrl { get; private set; }
        public static int IdUsuario { get; set; }
        // Propiedad estática para almacenar el estado de la conexión
        public static string NombreUsuario { get; set; }
        // Propiedad estática para almacenar nombre del usuario
        public static int tipoUsuario { get; set; }
        // Propiedad estática para almacenar el estado de la conexión
        public static bool IsConnectionSuccessful { get; private set; }
        private static Mutex? _singleInstanceMutex;

        // Colección global de monedas (Moneda, Codigo)
        public static IReadOnlyList<Currency> Monedas { get; } = new List<Currency>
            {
            new Currency("Peso argentino", "P"),
            new Currency("Dólar", "D"),
            new Currency("Unidad Indexada", "U")
            };

        // Diccionario para acceso rápido por código (case-insensitive)
        public static IReadOnlyDictionary<string, Currency> MonedaPorCodigo { get; } =
            Monedas.ToDictionary(m => m.Codigo, StringComparer.OrdinalIgnoreCase);

        public App()
            {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JGaF5cXGpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH1feHZVRWZZUkRzV0tWYEs=");

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

            // Establece la cultura para todos los threads de la aplicación
            var cultura = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentCulture = cultura;
            Thread.CurrentThread.CurrentUICulture = cultura;
            CultureInfo.DefaultThreadCurrentCulture = cultura;
            CultureInfo.DefaultThreadCurrentUICulture = cultura;



            }

        protected override void OnStartup(StartupEventArgs e)
            {

            // Mutex para evitar múltiples instancias
            bool createdNew;
            _singleInstanceMutex = new Mutex(true, "DataObra_SingleInstance_Mutex", out createdNew);

            if (!createdNew)
                {
                MessageBox.Show("La aplicación ya está en ejecución.", "DataObra", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
                }

            base.OnStartup(e);
            // Aquí puedes decidir cuál URL usar, por ejemplo, basado en una configuración
            // cambiar cuando pase a local
            // BaseUrl = "https://localhost:5000/";
            // cambiar cuando pase a producción
            BaseUrl = "https://servidordataobra.azurewebsites.net/";


            }

        }
    }
