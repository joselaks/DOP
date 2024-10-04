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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWGJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH9ec3ZWRGFZU0JxXkA=");
        }
    }

}
