using MonkeyConfAr.ViewModels;
using Xamarin.Forms;

namespace MonkeyConfAr.Views
{
    public partial class PointCloudScanPage : ContentPage
    {
        public PointCloudScanPageViewModel ViewModel
        {
            get => BindingContext as PointCloudScanPageViewModel;
        }

        public PointCloudScanPage()
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