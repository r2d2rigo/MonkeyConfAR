using GalaSoft.MvvmLight;
using MonkeyConfAr.Ar;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Urho;
using Urho.Forms;

namespace MonkeyConfAr.ViewModels
{
    public class PlaneTrackingPageViewModel : ViewModelBase
    {
        private static readonly Permission[] DEVICE_PERMISSIONS = new []
        {
            Permission.Camera,
        };

        private int _planeCount;
        private PlaneTrackingArApplication _application;

        public PlaneTrackingPageViewModel()
        {
        }

        public int PlaneCount
        {
            get => _planeCount;
            private set => Set(ref _planeCount, value);
        }

        public async Task InitializeArAsync(UrhoSurface surface)
        {
            await CrossPermissions.Current.RequestPermissionsAsync(DEVICE_PERMISSIONS);

            foreach (var devicePermission in DEVICE_PERMISSIONS)
            {
                var isPermissionEnabled = await CrossPermissions.Current.CheckPermissionStatusAsync(devicePermission);

                if (isPermissionEnabled != PermissionStatus.Granted)
                {   
                    return;
                }
            }

            _application = await surface.Show<PlaneTrackingArApplication>(new ApplicationOptions(assetsFolder: "Data")
            {
            });

            _application.Update += Application_Update;
        }

        private void Application_Update(UpdateEventArgs obj)
        {
            PlaneCount = _application
                .Scene
                .GetComponent<ArComponentBase>()
                .TrackedPlanes
                .Count;
        }
    }
}
