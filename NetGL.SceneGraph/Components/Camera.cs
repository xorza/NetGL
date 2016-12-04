using System;
using System.Drawing;
using System.Runtime.InteropServices;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;
using NetGL.SceneGraph.Serialization;

namespace NetGL.SceneGraph.Components {
    public enum CameraType {
        Orthographic,
        Perspective
    }

    public delegate void CameraOrderChangedEventHandler(Camera cam, int orderNo);

    [Guid("E7D90540-C9AF-4B5C-A4FD-E92D26DCABE1")]
    public sealed class Camera : Component, IComparable<Camera> {
        private readonly Matrix _viewMatrix = Matrix.Identity;
        private readonly Matrix _projectionMatrix = Matrix.Identity;
        private readonly Matrix _viewProjection = Matrix.Identity;
        private readonly Matrix _invertedViewProjection = Matrix.Identity;
        private float _fov = 3.14f / 4;
        private int _order = 0;

        public event CameraOrderChangedEventHandler OrderChanged = delegate { };

        [NotSerialized]
        internal Size ScreenSize { get; set; }

        [Display("Order #", "Rendering order number of this camera")]
        public int Order {
            get { return _order; }
            set {
                if (_order == value)
                    return;

                _order = value;
                OrderChanged(this, _order);
            }
        }

        [Display("Camera type", "Is camera orthographic or perspective?")]
        public CameraType Type { get; set; }
        [Display("Camera size", "Orthographics camera size")]
        public float Size { get; set; }
        [Display("Layer", "Distance to near clipping plane")]
        public Layer Layer { get; set; }

        [Display("Near", "Distance to near clipping plane")]
        public float Near { get; set; }
        [Display("Far", "Distance to far clipping plane")]
        public float Far { get; set; }
        [Color, Display("Background")]
        public Vector3 ClearColor { get; set; }
        [Color, Display("Ambient")]
        public Vector3 AmbientLight { get; set; }
        [NumberRange(0.1f, MathF.PI - 0.1f), Display("Field of view", "Angle of view in radians")]
        public float FOV {
            get { return _fov; }
            set {
                if (value > MathF.PI || value <= 0)
                    throw new ArgumentOutOfRangeException();

                _fov = value;
            }
        }

        [NotSerialized]
        public Frustum Frustum { get; private set; }
        [NotSerialized]
        public Matrix ViewMatrix {
            get { return _viewMatrix; }
        }
        [NotSerialized]
        public Matrix ProjectionMatrix {
            get { return _projectionMatrix; }
        }
        [NotSerialized]
        public Matrix ViewProjectionMatrix {
            get { return _viewProjection; }
        }
        [NotSerialized]
        public Matrix InvertedViewProjectionMatrix {
            get { return _invertedViewProjection; }
        }

        public Camera(Node owner)
            : base(owner) {
            ClearColor = new Vector3(1, 1, 1);
            FOV = 3.14f / 4;
            Size = 1;
            Near = 0.1f;
            Far = 100;
            Frustum = new Frustum();
            Layer = Layer.Default;
            Type = CameraType.Perspective;
            ClearColor = new Vector3(0.4f, 0.6f, 0.9f);
        }

        public void UpdateMatrix(Matrix cameraModelMatrix) {
            var aspect = ScreenSize.Width / (float)ScreenSize.Height;
            if (Type == CameraType.Orthographic)
                _projectionMatrix.LoadOrthographicAspect(Size, aspect, Near, Far);
            else
                _projectionMatrix.LoadPerspectiveFieldOfView(FOV, aspect, Near, Far);

            Matrix.Invert(cameraModelMatrix, _viewMatrix);
            Matrix.Multiply(_projectionMatrix, _viewMatrix, _viewProjection);
            Matrix.Invert(_viewProjection, _invertedViewProjection);

            Frustum.LoadFromViewProjectionMatrix(_viewProjection);
        }

        public Ray Unproject(Vector2 screenPosition) {
            var screenSize = ScreenSize;

            Vector4 viewportPosition;

            viewportPosition.X = (2.0f * screenPosition.X) / screenSize.Width - 1.0f;
            viewportPosition.Y = 1.0f - (2.0f * screenPosition.Y) / screenSize.Height;
            viewportPosition.Z = -1;
            viewportPosition.W = 1;

            var near = Vector4.Transform(viewportPosition, _invertedViewProjection);
            near = near / near.W;

            viewportPosition.Z = 1;
            var far = Vector4.Transform(viewportPosition, _invertedViewProjection);
            far = far / far.W;

            var result = new Ray((Vector3)near, (Vector3)(far - near));
            return result;
        }

        public Vector3 ScreenToViewport(Vector3 screen) {
            var screenSize = ScreenSize;
            Vector3 viewport;

            viewport.X = 2.0f * screen.X / screenSize.Width - 1.0f;
            viewport.Y = 1.0f - 2.0f * screen.Y / screenSize.Height;
            viewport.Z = screen.Z;

            return viewport;
        }
        public Vector3 ViewportToScreen(Vector3 viewport) {
            var screenSize = ScreenSize;
            Vector3 screen;

            screen.X = 0.5f * screenSize.Width * (viewport.X + 1.0f);
            screen.Y = 0.5f * screenSize.Height * (1.0f - viewport.Y);
            screen.Z = viewport.Z;

            return screen;
        }

        public Vector3 ScreenToWorld(Vector3 screen) {
            var viewport = ScreenToViewport(screen);
            var world = Vector4.Transform(new Vector4(viewport, 1), _invertedViewProjection);
            world /= world.W;
            return (Vector3)world;
        }
        public Vector3 WorldToScreen(Vector3 world) {
            var viewport = Vector4.Transform(new Vector4(world, 1), ViewProjectionMatrix);
            viewport /= viewport.W;
            var screen = ViewportToScreen((Vector3)viewport);

            return screen;
        }

        public int CompareTo(Camera other) {
            Assert.NotNull(other);

            return this.Order.CompareTo(other.Order);
        }
    }
}