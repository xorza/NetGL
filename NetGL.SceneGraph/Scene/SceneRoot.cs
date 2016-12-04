using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Colliders;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Control;
using NetGL.SceneGraph.OpenGL;
using NetGL.SceneGraph.Serialization;
using NetGL.SceneGraph.Tweens;
using Newtonsoft.Json;
using Graphics = NetGL.SceneGraph.OpenGL.Graphics;

namespace NetGL.SceneGraph.Scene {
    public class Scene : IDisposable {
        private readonly List<Camera> _cameras = new List<Camera>(5);
        private readonly List<LightSource> _lights = new List<LightSource>(4);
        private readonly List<IUpdatable> _updatables = new List<IUpdatable>(100);
        private readonly List<Collider> _colliders = new List<Collider>(100);

        private readonly TweenCollection _tweens = new TweenCollection();

        private readonly RendererCollection _rendererCollection = new RendererCollection();

        private readonly List<Node> _sceneObjects = new List<Node>(1000);
        private readonly List<Component> _newComponents = new List<Component>(20);

        private readonly Raycaster _raycaster = new Raycaster();

        private readonly Graphics _graphics;
        private readonly SceneTime _time;
        private readonly NetGLControl _control;

        private Size _screenSize;

        public int DrawCalls { get; private set; }
        public int FrameNo { get; private set; }
        public float FPS { get; private set; }
        public float FrameTime { get; private set; }
        public SceneTime Time {
            get {
                return _time;
            }
        }

        public IReadOnlyList<Node> SceneObjects { get; private set; }
        public Camera MainCamera {
            get {
                if (_cameras.Count == 0)
                    return null;

                return _cameras[0];
            }
        }
        public Size ScreenSize {
            get {
                return _screenSize;
            }
            internal set {
                if (_screenSize == value)
                    return;

                _screenSize = value;

                foreach (var cam in _cameras)
                    cam.ScreenSize = _screenSize;

                _graphics.ScreenSize = _screenSize;
            }
        }
        public TweenCollection Tweens { get { return _tweens; } }
        public INetGLControl Control { get { return _control; } }

        internal Scene(NetGLControl control, Graphics graphics, SceneTime time) {
            Assert.NotNull(control);

            _control = control;
            _time = time;
            _graphics = graphics;

            SceneObjects = _sceneObjects.AsReadOnly();

            DrawCalls = 0;
            FrameNo = 0;
        }

        internal void Add(Node sceneObject) {
            Assert.True(sceneObject.Scene == this);
            Assert.True(sceneObject.Components.Count == 0);
            Assert.True(sceneObject.Transform.Children.Count == 0);

            _sceneObjects.Add(sceneObject);
        }
        internal void Remove(Node sceneObject) {
            Assert.True(sceneObject.Scene == this);
            Assert.True(sceneObject.Components.Count == 0);
            Assert.True(sceneObject.Transform.Children.Count == 0);

            _sceneObjects.Remove(sceneObject);
        }
        internal void Add(Component component) {
            if (component is Camera) {
                var camera = (Camera)component;
                camera.ScreenSize = _screenSize;

                camera.OrderChanged += Camera_OrderChanged;
                _cameras.Add(camera);
                _cameras.Sort();
            }
            if (component is LightSource)
                _lights.Add((LightSource)component);
            if (component is Collider)
                _colliders.Add((Collider)component);
            if (component is IUpdatable)
                _updatables.Add((IUpdatable)component);
            if (component is Renderable)
                _rendererCollection.Add((Renderable)component);

            _newComponents.Add(component);
        }
        internal void Remove(Component component) {
            if (component is Collider)
                _colliders.Remove((Collider)component);
            if (component is Camera) {
                var camera = (Camera)component;
                camera.OrderChanged += Camera_OrderChanged;
                _cameras.Remove(camera);
            }
            if (component is LightSource)
                _lights.Remove((LightSource)component);
            if (component is Renderable)
                _rendererCollection.Remove((Renderable)component);

            _newComponents.Remove(component);
        }

        internal void Frame() {
            _time.CalcCurrent();
            var frameStartTime = RealTime.Time;

            Update();
            _graphics.GLContext.ResourceTracker.RunGC();
            Render();

            FrameTime = (float)(RealTime.Time - frameStartTime);
            CalcFPS();
            FrameNo++;
            DrawCalls = _graphics.DrawCalls;
        }
        private void Update() {
            _tweens.Update();

            if (_newComponents.Count > 0) {
                var newComponentsCopy = _newComponents.ToArray();
                _newComponents.Clear();

                for (int i = 0; i < newComponentsCopy.Length; i++) {
                    var item = newComponentsCopy[i];
                    if (item.IsDisposed == false)
                        item.Start();
                }
            }

            for (int i = _updatables.Count - 1; i >= 0; i--) {
                var item = _updatables[i];
                if (item.IsDisposed)
                    _updatables.RemoveAt(i);
                else {
                    if (item.IsEnabled)
                        item.Update();
                }
            }
        }
        private void Render() {
            if (_screenSize.Width == 0 || _screenSize.Height == 0)
                return;
            if (_cameras.Count == 0) {
                _graphics.RenderEmpty();
                return;
            }

            _graphics.RenderAll(_cameras, _lights, _rendererCollection);            
        }

        public RaycastMeshRendererHit RaycastMeshes(Ray ray) {
            var result = _raycaster.Raycast(ray, _rendererCollection);
            return result;
        }
        public RaycastColliderHit RaycastColliders(Ray ray) {
            var results = new List<RaycastColliderHit>();

            foreach (var collider in _colliders) {
                var hit = collider.Raycast(ray);
                if (hit != null)
                    results.Add(hit);
            }
            if (results.Count == 0)
                return null;
            if (results.Count == 1)
                return results[0];

            results.Sort();
            return results[0];
        }

        public void Clear() {
            _sceneObjects
                .ToArray()
                .ForEach(_ => _.Dispose());
            _sceneObjects.Clear();

            _rendererCollection.Clear();
            _lights.Clear();
            _cameras.Clear();
            _updatables.Clear();
            _newComponents.Clear();
        }
        public void Serialize(string fileName) {
            var context = new SerializationContext(this, fileName);
            context.Serialize();
        }
        public void Derialize(string fileName) {
            Clear();

            var context = new DeserializationContext(this, fileName);
            context.Deserialize();
        }

        public void Dispose() {
            GC.SuppressFinalize(this);

            _graphics.Dispose();
            Clear();
        }

        private void Camera_OrderChanged(Camera cam, int orderNo) {
            _cameras.Sort();
        }

        #region fpsCalc
        private int _frames = 0;
        private float _lastFpsMeasureTime;

        private void CalcFPS() {
            _frames++;
            var delta = Time.CurrentFloat - _lastFpsMeasureTime;
            if (delta >= 1) {
                FPS = _frames / delta;
                _frames = 0;
                _lastFpsMeasureTime = Time.CurrentFloat;
            }
        }
        #endregion
    }
}