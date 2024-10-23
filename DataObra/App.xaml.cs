using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace DataObra
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public HttpClient HttpClient { get; private set; }
        public static string BaseUrl { get; private set; }
        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWGJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH9ec3ZWRGFZU0JxXkA=");

            // Configurar HttpClient
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            var servicios = serviceCollection.BuildServiceProvider();
            var httpClientFactory = servicios.GetRequiredService<IHttpClientFactory>();
            HttpClient = httpClientFactory.CreateClient();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Aquí puedes decidir cuál URL usar, por ejemplo, basado en una configuración

            BaseUrl = "https://localhost:7255/";
            // cambiar cuando pase a producción
            //BaseUrl = "https://dataobra.com/";


            // Otros códigos de inicialización si es necesario
        }
    }

}
