using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NetGL.SceneGraph.Scene {
    public class Node : IDisposable {
        private readonly List<Component> _components;

        private readonly Transform _transform;
        private readonly Scene _scene;
        private readonly SceneTime _time;

        private Layer _layer;

        internal Guid Id { get; set; }

        public Transform Transform {
            get { return _transform; }
        }

        public IReadOnlyList<Component> Components { get; private set; }
        public string Name { get; set; }
        public bool IsDisposed { get; private set; }
        public bool IsSerialized { get; set; }
        public Layer Layer {
            get { return _layer; }
            set {
                //Assert.NotNull(value);
                _layer = value;
            }
        }
        public Scene Scene {
            get { return _scene; }
        }

        public Node(Scene scene)
            : this(scene, "SceneObject") {
        }
        public Node(Scene scene, string name) {
            Assert.NotNull(scene);

            this.IsSerialized = true;
            this.Name = name;
            this.IsDisposed = false;

            _scene = scene;
            _time = _scene.Time;

            _transform = new Transform(this);
            _components = new List<Component>();
            Components = _components.AsReadOnly();

            Id = Guid.NewGuid();
            Layer = Layer.Default;

            _scene.Add(this);
        }

        public T AddComponent<T>() where T : Component {
            return (T)AddComponent(typeof(T));
        }
        public Component AddComponent(Type componentType) {
            Assert.True(componentType.IsSubclassOf(typeof(Component)));
            var component = (Component)Activator.CreateInstance(componentType, this);
            Assert.NotNull(component);
            return component;
        }
        internal void AddComponent(Component component) {
            Assert.NotNull(component);

            _components.Add(component);
            if (_scene != null)
                _scene.Add(component);
        }
        internal void RemoveComponent(Component component) {
            Assert.NotNull(component);

            _components.Add(component);
            if (_scene != null)
                _scene.Remove(component);
        }
        public T GetComponent<T>() where T : class {
            for (int i = 0; i < _components.Count; i++) {
                var c = _components[i] as T;
                if (c != null)
                    return c;
            }

            return null;
        }

        public void Dispose() {
            if (IsDisposed)
                return;
            IsDisposed = true;

            _transform.Children
                .ToArray()
                .ForEach(_ => _.SceneObject.Dispose());
            _transform.Dispose();

            _components
                .ToArray()
                .ForEach(_ => _.Dispose());
            _components.Clear();

            _scene.Remove(this);
        }

        public override string ToString() {
            if (string.IsNullOrWhiteSpace(Name))
                return "SceneObject";
            else
                return Name;
        }
    }
}