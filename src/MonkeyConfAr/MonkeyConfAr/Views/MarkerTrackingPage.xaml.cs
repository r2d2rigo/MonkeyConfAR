using GalaSoft.MvvmLight.Views;
using MonkeyConfAr.ViewModels;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.ComponentModel;
using Xamarin.Forms;

namespace MonkeyConfAr.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MarkerTrackingPage : ContentPage
    {
        public MarkerTrackingPageViewModel ViewModel
        {
            get => BindingContext as MarkerTrackingPageViewModel;
        }

        public MarkerTrackingPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await ViewModel.InitializeArAsync(_arSurface);
        }
    }
}
