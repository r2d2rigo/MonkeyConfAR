namespace MonkeyConfAr.Ar
{
    public class TrackableImage
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public TrackableImage(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
