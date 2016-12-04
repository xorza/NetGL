using NetGL.SceneGraph.Scene;
using NetGL.SceneGraph.Serialization;

namespace NetGL.SceneGraph.Components {
    internal delegate void RendererEventHandler(Renderable sender);

    public abstract class Renderable : Component {
        private Material _material;

        internal abstract IRenderer Renderer { get; }

        public Material Material {
            get { return _material; }
            set {
                if (_material == value)
                    return;

                _material = value;
                MaterialUpdated(this);
            }
        }
        [NotSerialized]
        public bool IsVisible { get; internal set; }

        internal event RendererEventHandler MaterialUpdated = delegate { };

        internal Renderable(Node owner) : base(owner) { }

        protected override void OnDispose() {
            MaterialUpdated = null;

            _material = null;

            base.OnDispose();
        }
    }
}