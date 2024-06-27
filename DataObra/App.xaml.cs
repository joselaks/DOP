using System.Configuration;
using System.Data;
using System.Windows;

namespace DataObra
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzM1NDAzN0AzMjM2MmUzMDJlMzBLTnVMTDcyanFpbFRZa0RLNExzcVFGTnhOd0JadEU2U1VrRmNvdVY4bUhrPQ==");
        }
    }

}
