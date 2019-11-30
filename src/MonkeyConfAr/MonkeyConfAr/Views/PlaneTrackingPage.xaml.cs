using MonkeyConfAr.ViewModels;
using Xamarin.Forms;

namespace MonkeyConfAr.Views
{
    public partial class PlaneTrackingPage : ContentPage
    {
        public PlaneTrackingPageViewModel ViewModel
        {
            get => BindingContext as PlaneTrackingPageViewModel;
        }

        public PlaneTrackingPage()
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