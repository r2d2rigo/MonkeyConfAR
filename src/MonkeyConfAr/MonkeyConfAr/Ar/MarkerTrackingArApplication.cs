using System;
using System.Collections.Generic;
using System.Text;
using Urho;
using Xamarin.Forms;

namespace MonkeyConfAr.Ar
{
    public class MarkerTrackingArApplication : SimpleApplication
    {
        private readonly IArComponentFactory ArComponentFactory;

        private ArComponentBase _arComponent;
        private List<Node> _monkeys;


        public MarkerTrackingArApplication(ApplicationOptions options) : base(options)
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

            _monkeys = new List<Node>();

            _arComponent = ArComponentFactory.CreateArComponent(Scene);
            await _arComponent.InitializeAsync();

            await _arComponent.RegisterTrackableImageAsync("monkeyMarker1", this.GetType().Assembly.GetManifestResourceStream("MonkeyConfAr.Data.ar-tracker.png"), 0.10f);
        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);

            foreach (var monkey in _monkeys)
            {
                monkey.Enabled = false;
            }

            var trackedResultIndex = 0;

            foreach (var trackingResult in _arComponent.TrackingResults)
            {
                if (trackedResultIndex > _monkeys.Count - 1)
                {
                    _monkeys.Add(CreateMonkeyModel());
                }

                var currentMonkey = _monkeys[trackedResultIndex];

                currentMonkey.Enabled = true;
                currentMonkey.Position = _arComponent.TrackingResults[trackedResultIndex].Position;
                currentMonkey.Rotation = _arComponent.TrackingResults[trackedResultIndex].Rotation;
                currentMonkey.GetComponent<AnimatedModel>().GetMaterial(0).SetShaderParameter("MatDiffColor", _arComponent.AmbientHdrLightIntensity);
            }
        }

        private Node CreateMonkeyModel()
        {
            var monkeyNode = Scene.CreateChild();
            var monkeyModel = monkeyNode.CreateComponent<AnimatedModel>();

            // Xamarin monkey model created by Vic Wang at http://vidavic.weebly.com
            monkeyModel.Model = ResourceCache.GetModel("monkey1.mdl");
            monkeyModel.SetMaterial(ResourceCache.GetMaterial("Materials/phong1.xml"));

            monkeyNode.Scale = new Vector3(0.025f, 0.025f, 0.025f); 

            return monkeyNode;
        }
    }
}
