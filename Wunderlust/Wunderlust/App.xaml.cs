using System;
using Wunderlust.Services;
using Wunderlust.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace Wunderlust
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            Application.Current.Properties["authorized"] = "expected";
            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();            
        }        
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
