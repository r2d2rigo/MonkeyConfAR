using GalaSoft.MvvmLight;
using MonkeyConfAr.Ar;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Urho;
using Urho.Forms;

namespace MonkeyConfAr.ViewModels
{
    public class PointCloudScanPageViewModel : ViewModelBase
    {
        private static readonly Permission[] DEVICE_PERMISSIONS = new []
        {
            Permission.Camera,
        };

        private int _pointCount;
        private PointCloudArApplication _application;

        public PointCloudScanPageViewModel()
        {
        }

        public int PointCount
        {
            get => _pointCount;
            private set => Set(ref _pointCount, value);
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

            _application = await surface.Show<PointCloudArApplication>(new ApplicationOptions(assetsFolder: "Data")
            {
            });

            _application.Update += Application_Update;
        }

        private void Application_Update(UpdateEventArgs obj)
        {
            PointCount = _application.TrackedPointCount;
        }
    }
}
