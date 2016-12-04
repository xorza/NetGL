using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.OpenGL;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Components {
    public class ParticlesRenderable : Renderable {
        private ParticleRenderer _particleRenderer;

        internal override IRenderer Renderer {
            get { return _particleRenderer; }
        }

        public ParticlesRenderable(Node owner)
            : base(owner) { }

        protected override void OnStart() {
            base.OnStart();

            _particleRenderer = new ParticleRenderer(Time);
        }
        protected override void OnDispose() {
            Disposer.Dispose(ref _particleRenderer);

            base.OnDispose();
        }
    }
}
