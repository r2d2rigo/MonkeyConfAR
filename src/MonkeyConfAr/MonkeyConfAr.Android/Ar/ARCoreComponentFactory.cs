using MonkeyConfAr.Ar;
using MonkeyConfAr.Droid.Ar;
using Urho;
using Xamarin.Forms;

[assembly: Dependency(typeof(ARCoreComponentFactory))]
namespace MonkeyConfAr.Droid.Ar
{
    public class ARCoreComponentFactory : IArComponentFactory
    {
        public ArComponentBase CreateArComponent(Scene scene)
        {
            return scene.CreateComponent<ARCoreComponent>();
        }
    }
}