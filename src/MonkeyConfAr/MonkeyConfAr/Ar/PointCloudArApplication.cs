using System.Collections.Generic;
using System.Linq;
using Urho;
using Xamarin.Forms;

namespace MonkeyConfAr.Ar
{
    public class PointCloudArApplication : SimpleApplication
    {
        private const int MAX_CLOUD_POINTS = 1000;

        private readonly IArComponentFactory ArComponentFactory;

        private ArComponentBase _arComponent;
        private DebugRenderer _debugRenderer;

        private List<PointCloudPoint> _trackedPoints;

        public PointCloudArApplication(ApplicationOptions options) : base(options)
        {
            ArComponentFactory = DependencyService.Get<IArComponentFactory>();
        }

        public int TrackedPointCount
        {
            get => _trackedPoints.Count;
        }

        protected override void Setup()
        {
            base.Setup();
        }

        protected override async void Start()
        {
            base.Start();

            _debugRenderer = Scene.CreateComponent<DebugRenderer>();
            _trackedPoints = new List<PointCloudPoint>();

            _arComponent = ArComponentFactory.CreateArComponent(Scene);
            await _arComponent.InitializeAsync();
        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);

            var debugRenderer = Scene.GetComponent<DebugRenderer>();

            _trackedPoints.AddRange(_arComponent.PointCloud);

            _trackedPoints = _trackedPoints
                .Skip(_trackedPoints.Count - MAX_CLOUD_POINTS)
                .ToList();

            foreach (var point in _trackedPoints)
            {
                debugRenderer.AddSphere(
                    new SphereShape(point.Position, 0.005f),
                    (Urho.Color.Green * point.Confidence + 
                    Urho.Color.Red * (1.0f - point.Confidence)), false);
            }
        }
    }
}
