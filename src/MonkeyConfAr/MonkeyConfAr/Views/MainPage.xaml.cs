using GalaSoft.MvvmLight.Views;
using MonkeyConfAr.ViewModels;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.ComponentModel;
using Xamarin.Forms;

namespace MonkeyConfAr.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void _pointCloudButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new PointCloudScanPage());
        }

        private void _planeButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new PlaneTrackingPage());
        }

        private void _markerTracking_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MarkerTrackingPage());
        }
    }
}
