using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace Backend
    {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
        {
        public IServiceProvider ServiceProvider { get; private set; }
        public HttpClient HttpClient { get; private set; }
        public static string BaseUrl { get; private set; }
        // Propiedad estática para almacenar el estado de la conexión
        public static bool IsConnectionSuccessful { get; private set; }

        public App()
            {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF1cWGhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEBjXH1dcXxXQGRVUUF3Wklfag==");

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

            }

        protected override void OnStartup(StartupEventArgs e)
            {
            base.OnStartup(e);
            // Aquí puedes decidir cuál URL usar, por ejemplo, basado en una configuración

            // cambiar cuando pase a local
            // BaseUrl = "https://localhost:5000/";
            // cambiar cuando pase a producción
            BaseUrl = "https://servidordataobra.azurewebsites.net/";


            }

        }
    }
