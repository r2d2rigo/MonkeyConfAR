using System.Collections.Generic;
using System.Linq;
using Urho;
using Xamarin.Forms;

namespace MonkeyConfAr.Ar
{
    public class PlaneTrackingArApplication : SimpleApplication
    {
        private readonly IArComponentFactory ArComponentFactory;

        private ArComponentBase _arComponent;
        private DebugRenderer _debugRenderer;
        private CustomGeometry _planeRenderer;

        public PlaneTrackingArApplication(ApplicationOptions options) : base(options)
        {
            ArComponentFactory = DependencyService.Get<IArComponentFactory>();
        }

        protected override void Setup()
        {
            base.Setup();
        }

        protected override async void Start()
        {
            base.Start();

            _debugRenderer = Scene.CreateComponent<DebugRenderer>();

            _planeRenderer = Scene.CreateComponent<CustomGeometry>();
            var material = new Material();
            material.SetTechnique(0, CoreAssets.Techniques.NoTextureUnlitVCol, 1, 1);
            material.CullMode = CullMode.None;
            _planeRenderer.SetMaterial(material);

            _arComponent = ArComponentFactory.CreateArComponent(Scene);
            await _arComponent.InitializeAsync();
        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);

            var debugRenderer = Scene.GetComponent<DebugRenderer>();

            _planeRenderer.BeginGeometry(0, PrimitiveType.TriangleList);

            foreach (var plane in _arComponent.TrackedPlanes)
            {
                _planeRenderer.DefineVertex(plane.Position + new Vector3(plane.ExtentsX * 0.5f, 0, plane.ExtentsZ * 0.5f));
                _planeRenderer.DefineColor(Urho.Color.Red);

                _planeRenderer.DefineVertex(plane.Position + new Vector3(plane.ExtentsX * 0.5f, 0, -plane.ExtentsZ * 0.5f));
                _planeRenderer.DefineColor(Urho.Color.Red);

                _planeRenderer.DefineVertex(plane.Position + new Vector3(-plane.ExtentsX * 0.5f, 0, -plane.ExtentsZ * 0.5f));
                _planeRenderer.DefineColor(Urho.Color.Red);

                _planeRenderer.DefineVertex(plane.Position + new Vector3(-plane.ExtentsX * 0.5f, 0, -plane.ExtentsZ * 0.5f));
                _planeRenderer.DefineColor(Urho.Color.Red);

                _planeRenderer.DefineVertex(plane.Position + new Vector3(-plane.ExtentsX * 0.5f, 0, plane.ExtentsZ * 0.5f));
                _planeRenderer.DefineColor(Urho.Color.Red);

                _planeRenderer.DefineVertex(plane.Position + new Vector3(plane.ExtentsX * 0.5f, 0, plane.ExtentsZ * 0.5f));
                _planeRenderer.DefineColor(Urho.Color.Red);
            }

            _planeRenderer.Commit();
        }
    }
}
