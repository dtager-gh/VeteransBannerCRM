using System.Configuration;
using System.Data;
using System.Windows;
using VeteransBannerCRM.Data;

namespace VeteransBannerCRM
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Database.Initialize();
        }
    }
}