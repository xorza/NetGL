using NetGL.Core.Types;
using NetGL.SceneGraph.OpenGL;
using NetGL.SceneGraph.Scene;
using System;
using System.Runtime.InteropServices;

namespace NetGL.SceneGraph.Components {
    [Guid("B6F1D688-93AB-4759-833C-5B69B8EF370B")]
    public class MeshRenderable : Renderable {
        private Mesh _mesh;
        private readonly MeshRenderer _meshRenderer;

        internal override IRenderer Renderer {
            get { return _meshRenderer; }
        }

        public bool IsRaytracable { get; set; }
        public bool IsShadowCaster { get; set; }       

        public Mesh Mesh {
            get { return _mesh; }
            set {
                if (_mesh == value)
                    return;

                if (value.Bounds == null)
                    throw new NullReferenceException("Mesh.Bounds");
                if (value.Vertices == null)
                    throw new NullReferenceException("Mesh.Vertices");

                _mesh = value;
                if (_meshRenderer != null)
                    _meshRenderer.Mesh = _mesh;
            }
        }

        public MeshRenderable(Node owner)
            : base(owner) {
            IsRaytracable = true;
            IsShadowCaster = true;

            _meshRenderer = new MeshRenderer();
        }

        public void UpdateMesh() {
            if (IsDisposed)
                throw new ObjectDisposedException("Renderer");
            if (_mesh == null)
                throw new NullReferenceException("Mesh");

            _meshRenderer.UpdateMesh();
        }

        protected override void OnStart() {
            base.OnStart();
        }        
        protected override void OnDispose() {
            if (_meshRenderer != null)
                _meshRenderer.Dispose();

            base.Dispose();
        }
    }
}