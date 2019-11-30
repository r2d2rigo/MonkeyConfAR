using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Urho;

namespace MonkeyConfAr.Ar
{
    public abstract class ArComponentBase : Component
    {
        protected List<ImageTrackingResult> _trackingResults;
        protected List<PointCloudPoint> _pointCloud;
        protected List<PlaneTrackingResult> _trackedPlanes;

        public ArComponentBase() : base()
        {
            _trackingResults = new List<ImageTrackingResult>();
            _pointCloud = new List<PointCloudPoint>();
            _trackedPlanes = new List<PlaneTrackingResult>();
        }

        public ArComponentBase(IntPtr handle) : base(handle)
        {
            _trackingResults = new List<ImageTrackingResult>();
            _pointCloud = new List<PointCloudPoint>();
            _trackedPlanes = new List<PlaneTrackingResult>();
        }

        public IReadOnlyList<ImageTrackingResult> TrackingResults
        {
            get => _trackingResults;
        }

        public IReadOnlyList<PointCloudPoint> PointCloud
        {
            get => _pointCloud;
        }

        public IReadOnlyList<PlaneTrackingResult> TrackedPlanes
        {
            get => _trackedPlanes;
        }

        public bool Paused { get; protected set; }

        public Urho.Color AmbientHdrLightIntensity { get; protected set; }

        public abstract Task InitializeAsync();

        public abstract Task<TrackableImage> RegisterTrackableImageAsync(string name, Stream imageData, float realWorldSize);
    }
}
