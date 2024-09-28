using System.Configuration;
using System.Data;
using System.Windows;

namespace Gestion
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzUwMjU2MUAzMjM3MmUzMDJlMzBDZ3ZHalBiSHdOVTAybkNRWFlwako1ejJvQ0c5V245M0pSbktjVGRjNDRJPQ==");
        }

    }

}
