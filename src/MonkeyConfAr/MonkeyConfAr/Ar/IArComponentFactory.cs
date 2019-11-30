using Urho;

namespace MonkeyConfAr.Ar
{
    public interface IArComponentFactory
    {
        ArComponentBase CreateArComponent(Scene scene);
    }
}
