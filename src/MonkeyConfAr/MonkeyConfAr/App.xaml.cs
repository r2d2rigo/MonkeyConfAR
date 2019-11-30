using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MonkeyConfAr.Views;
using Urho.Forms;
using Xamarin.Forms;

namespace MonkeyConfAr
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            UrhoSurface.OnPause();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            UrhoSurface.OnResume();
        }
    }
}
