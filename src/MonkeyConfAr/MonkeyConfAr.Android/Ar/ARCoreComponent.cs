using Android;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Views;
using Google.AR.Core;
using MonkeyConfAr.Ar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Urho;
using Urho.IO;
using Urho.Urho2D;

namespace MonkeyConfAr.Droid.Ar
{
    public class ARCoreComponent : ArComponentBase
    {
        const uint GL_TEXTURE_EXTERNAL_OES = 36197;

        private Session _arCoreSession;
        private Config _arCoreConfig;
        private AugmentedImageDatabase _imageDatabase;

        public Texture2D CameraTexture { get; private set; }
        public Viewport Viewport { get; private set; }
        public Urho.Camera Camera { get; private set; }
        public string ARCoreShader { get; set; } = "ARCore";

        [Preserve]
        public ARCoreComponent() { ReceiveSceneUpdates = true; }

        [Preserve]
        public ARCoreComponent(IntPtr handle) : base(handle) { ReceiveSceneUpdates = true; }

        public override unsafe void OnAttachedToNode(Node node)
        {
            Application.Paused += OnPause;
            Application.Resumed += OnResume;
        }

        public override Task InitializeAsync()
        {
            if (CameraTexture != null)
                throw new InvalidOperationException("ARCore component is already initialized, if you want to re-configure ARCore session - use Session property.");

            if (Camera == null)
                Camera = base.Scene.GetComponent<Urho.Camera>(true);

            if (Camera == null)
                throw new InvalidOperationException("Camera component was not found.");

            CameraTexture = new Texture2D();
            CameraTexture.SetCustomTarget(GL_TEXTURE_EXTERNAL_OES);
            CameraTexture.SetNumLevels(1);
            CameraTexture.FilterMode = TextureFilterMode.Bilinear;
            CameraTexture.SetAddressMode(TextureCoordinate.U, TextureAddressMode.Clamp);
            CameraTexture.SetAddressMode(TextureCoordinate.V, TextureAddressMode.Clamp);
            CameraTexture.SetSize(Application.Graphics.Width, Application.Graphics.Height, Graphics.RGBFormat, TextureUsage.Dynamic);
            CameraTexture.Name = nameof(CameraTexture);
            Application.ResourceCache.AddManualResource(CameraTexture);

            AmbientHdrLightIntensity = Urho.Color.White;

            Viewport = Application.Renderer.GetViewport(0);

            if (base.Application is SimpleApplication simpleApp)
            {
                simpleApp.MoveCamera = false;
            }

            var videoRp = new RenderPathCommand(RenderCommandType.Quad);
            videoRp.PixelShaderName = (UrhoString)ARCoreShader; //see CoreData/Shaders/GLSL/ARCore.glsl
            videoRp.VertexShaderName = (UrhoString)ARCoreShader;
            videoRp.SetOutput(0, "viewport");
            videoRp.SetTextureName(TextureUnit.Diffuse, CameraTexture.Name);
            Viewport.RenderPath.InsertCommand(1, videoRp);

            var tcs = new TaskCompletionSource<bool>();
            var activity = (Activity)Urho.Application.CurrentWindow.Target;
            activity.RunOnUiThread(() =>
            {
                var cameraAllowed = activity.CheckSelfPermission(Manifest.Permission.Camera);
                if (cameraAllowed != Permission.Granted)
                    throw new InvalidOperationException("Camera permission: Denied");

                _arCoreSession = new Session(activity);
                _arCoreSession.SetCameraTextureName((int)CameraTexture.AsGPUObject().GPUObjectName);
                _arCoreSession.SetDisplayGeometry((int)SurfaceOrientation.Rotation0 /*windowManager.DefaultDisplay.Rotation*/,
                    Application.Graphics.Width, Application.Graphics.Height);

                _arCoreConfig = new Config(_arCoreSession);
                _arCoreConfig.SetFocusMode(Config.FocusMode.Auto);
                _arCoreConfig.SetLightEstimationMode(Config.LightEstimationMode.EnvironmentalHdr);
                _arCoreConfig.SetUpdateMode(Config.UpdateMode.LatestCameraImage);
                _arCoreConfig.SetPlaneFindingMode(Config.PlaneFindingMode.Horizontal);

                _imageDatabase = new AugmentedImageDatabase(_arCoreSession);
                _arCoreConfig.SetAugmentedImageDatabase(_imageDatabase);

                if (!_arCoreSession.IsSupported(_arCoreConfig))
                {
                    throw new Exception("AR is not supported on this device with given config");
                }

                _arCoreSession.Configure(_arCoreConfig);

                Paused = false;
                _arCoreSession.Resume();
                Urho.Application.InvokeOnMain(() => tcs.TrySetResult(true));
            });

            return tcs.Task;
        }

        public override async Task<TrackableImage> RegisterTrackableImageAsync(string name, Stream imageData, float realWorldSize)
        {
            var bitmap = await BitmapFactory.DecodeStreamAsync(imageData);
            var trackableId = _imageDatabase.AddImage(name, bitmap, realWorldSize);

            _arCoreConfig.SetAugmentedImageDatabase(_imageDatabase);
            _arCoreSession.Configure(_arCoreConfig);

            return new TrackableImage(trackableId, name);
        }

        void OnPause()
        {
            Paused = true;
            _arCoreSession?.Pause();
        }

        void OnResume()
        {
            Paused = false;
            _arCoreSession?.Resume();
        }

        protected override void OnDeleted()
        {
            Application.Paused -= OnPause;
            Application.Resumed -= OnResume;

            base.OnDeleted();
            try
            {
                _arCoreSession?.Pause();
            }
            catch (Exception exc)
            {
                Log.Write(LogLevel.Warning, "ARCore pause error: " + exc);
            }
        }

        Anchor anchor = null;

        protected override void OnUpdate(float timeStep)
        {
            _trackingResults.Clear();
            _pointCloud.Clear();

            if (Paused)
                return;

            if (Camera == null)
                throw new Exception("ARCore.Camera property was not set");

            try
            {
                if (_arCoreSession == null)
                    return;

                var frame = _arCoreSession.Update();
                if (Paused) //in case if Config.UpdateMode.LatestCameraImage is not used
                    return;

                var camera = frame.Camera;
                if (camera.TrackingState != TrackingState.Tracking)
                    return;

                UpdatePointCloud(frame);

                UpdatePlaneTrackables(frame);

                UpdateImageTrackables(frame);

                var lightIntensity = frame.LightEstimate.GetEnvironmentalHdrMainLightIntensity();
                AmbientHdrLightIntensity = new Urho.Color(lightIntensity[0], lightIntensity[1], lightIntensity[2]);

                var farPlane = 100f;
                var nearPlane = 0.01f;

                float[] projmx = new float[16];
                camera.GetProjectionMatrix(projmx, 0, nearPlane, farPlane);

                var prj = new Urho.Matrix4(
                    projmx[0], projmx[4], projmx[8], projmx[12],
                    projmx[1], projmx[5], projmx[9], projmx[13],
                    projmx[2], projmx[6], projmx[10], projmx[14],
                    projmx[3], projmx[7], projmx[11], projmx[15]
                );

                prj.M34 /= 2f;
                prj.M33 = farPlane / (farPlane - nearPlane);
                prj.M43 *= -1;

                Camera.SetProjection(prj);

                TransformByPose(Camera.Node, camera.DisplayOrientedPose);
            }
            catch (Exception exc)
            {
                Log.Write(LogLevel.Warning, "ARCore error: " + exc);
            }
        }

        public static void TransformByPose(Node node, Pose pose)
        {
            // Right-Handed coordinate system to Left-Handed
            node.Rotation = new Quaternion(pose.Qx(), pose.Qy(), -pose.Qz(), -pose.Qw());
            node.Position = new Vector3(pose.Tx(), pose.Ty(), -pose.Tz());
        }

        private void UpdatePlaneTrackables(Frame frame)
        {
            var trackables = frame.GetUpdatedTrackables(Java.Lang.Class.FromType(typeof(Google.AR.Core.Plane)));
            var newTrackedPlanes = new List<PlaneTrackingResult>();

            foreach (Google.AR.Core.Plane trackedPlane in trackables)
            {
                if (trackedPlane.TrackingState == TrackingState.Tracking && trackedPlane.GetType() == Google.AR.Core.Plane.Type.HorizontalUpwardFacing)
                {
                    newTrackedPlanes.Add(new PlaneTrackingResult(
                            new Vector3(trackedPlane.CenterPose.Tx(), trackedPlane.CenterPose.Ty(), -trackedPlane.CenterPose.Tz()),
                            trackedPlane.ExtentX,
                            trackedPlane.ExtentZ
                        ));
                }
            }

            if (newTrackedPlanes.Count > 0)
            {
                _trackedPlanes = newTrackedPlanes;
            }
        }

        private void UpdatePointCloud(Frame frame)
        {
            using (var pointCloud = frame.AcquirePointCloud())
            {
                var pointBuffer = pointCloud.Points;

                while (pointBuffer.HasRemaining)
                {
                    var pointX = pointBuffer.Get();
                    var pointY = pointBuffer.Get();
                    var pointZ = pointBuffer.Get();

                    var pointConfidence = pointBuffer.Get();

                    _pointCloud.Add(new PointCloudPoint()
                    {
                        Position = new Urho.Vector3(pointX, pointY, -pointZ),
                        Confidence = pointConfidence
                    });
                }
            }
        }

        private void UpdateImageTrackables(Frame frame)
        {
            var trackables = frame.GetUpdatedTrackables(Java.Lang.Class.FromType(typeof(AugmentedImage)));

            foreach (AugmentedImage image in trackables)
            {
                if (image.TrackingState == TrackingState.Tracking)
                {
                    _trackingResults.Add(new ImageTrackingResult(
                        new TrackableImage(image.Index, image.Name),
                        new Vector3(image.CenterPose.Tx(), image.CenterPose.Ty(), -image.CenterPose.Tz()),
                        new Quaternion(image.CenterPose.Qx(), image.CenterPose.Qy(), -image.CenterPose.Qz(), -image.CenterPose.Qw())));
                }
            }
        }
    }
}