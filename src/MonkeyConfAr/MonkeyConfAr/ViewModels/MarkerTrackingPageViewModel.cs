using GalaSoft.MvvmLight;
using MonkeyConfAr.Ar;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Urho;
using Urho.Forms;

namespace MonkeyConfAr.ViewModels
{
    public class MarkerTrackingPageViewModel : ViewModelBase
    {
        private static readonly Permission[] DEVICE_PERMISSIONS = new []
        {
            Permission.Camera,
        };

        public MarkerTrackingPageViewModel()
        {
        }

        public async Task InitializeArAsync(UrhoSurface surface)
        {
            await CrossPermissions.Current.RequestPermissionsAsync(DEVICE_PERMISSIONS);

            foreach (var devicePermission in DEVICE_PERMISSIONS)
            {
                var isPermissionEnabled = await CrossPermissions.Current.CheckPermissionStatusAsync(devicePermission);

                if (isPermissionEnabled != PermissionStatus.Granted)
                {
                    // TODO: show initialization error
                    return;
                }
            }

            var application = await surface.Show<MarkerTrackingArApplication>(new ApplicationOptions(assetsFolder: "Data")
            {
            });
        }
    }
}
