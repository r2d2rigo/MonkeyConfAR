using Urho;

namespace MonkeyConfAr.Ar
{
    public class ImageTrackingResult
    {
        public TrackableImage Image { get; private set; }

        public Vector3 Position { get; private set; }

        public Quaternion Rotation { get; private set; }

        public ImageTrackingResult(TrackableImage image, Vector3 position, Quaternion rotation)
        {
            Image = image;
            Position = position;
            Rotation = rotation;
        }
    }
}
