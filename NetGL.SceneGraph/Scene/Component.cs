using NetGL.SceneGraph.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace NetGL.SceneGraph.Scene {
    public abstract class Component : IDisposable {
        private bool _isStarted;
        private bool _isDisposed;

        private readonly Node _sceneObject;
        private readonly Transform _transform;
        private readonly Scene _scene;
        private readonly SceneTime _time;

        [NotSerialized]
        internal Guid Id { get; set; }

        [NotSerialized]
        public SceneTime Time {
            get { return _time; }
        }
        [NotSerialized]
        public Node SceneObject {
            get { return _sceneObject; }
        }
        [NotSerialized]
        public Transform Transform {
            get { return _transform; }
        }
        [NotSerialized]
        public Scene Scene {
            get { return _scene; }
        }

        [NotSerialized]
        public bool IsDisposed { get { return _isDisposed; } }
        [NotSerialized]
        public bool IsEnabled { get; set; }

        protected Component(Node owner) {
            if (owner == null)
                throw new ArgumentNullException("owner");
            if (owner.IsDisposed)
                throw new ObjectDisposedException("owner");

            _isStarted = false;
            _isDisposed = false;
            IsEnabled = true;
            Id = Guid.NewGuid();

            _sceneObject = owner;
            _transform = _sceneObject.Transform;
            _scene = _sceneObject.Scene;
            _time = _scene.Time;

            Assert.NotNull(_transform);
            Assert.NotNull(_scene);
            Assert.NotNull(_time);

            _sceneObject.AddComponent(this);

            OnInit();
        }

        internal void Start() {
            Assert.False(IsDisposed);
            Assert.False(_isStarted);

            _isStarted = true;
            OnStart();
        }

        public void Dispose() {
            if (_isDisposed)
                return;
            _isDisposed = true;

            _sceneObject.RemoveComponent(this);
            OnDispose();
        }

        protected virtual void OnInit() { }
        protected virtual void OnStart() { }
        protected virtual void OnDispose() { }
    }
}