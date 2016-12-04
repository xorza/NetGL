using NetGL.Core.Infrastructure;
using NetGL.Core.Types;
using System;
using System.Collections.Generic;

namespace NetGL.SceneGraph.OpenGL {
    internal sealed class MeshVaoCollection : IDisposable {
        private bool _isDisposed = false;

        private readonly Dictionary<Mesh, VertexArrayObjectReference> _meshVaoReferenceDictionary = new Dictionary<Mesh, VertexArrayObjectReference>(500);

        public VertexArrayObject Get(Mesh mesh) {
            VertexArrayObjectReference vaoReference;
            VertexArrayObject result;

            if (_meshVaoReferenceDictionary.TryGetValue(mesh, out vaoReference)) {
                Assert.False(vaoReference.IsDisposed);
                result = vaoReference.Vao;
            }
            else {
                result = new VertexArrayObject();
                vaoReference = new VertexArrayObjectReference(mesh, result);
                _meshVaoReferenceDictionary.Add(mesh, vaoReference);
                result.SetMesh(mesh);
            }

            vaoReference.Capture();
            return result;
        }
        public void Release(Mesh mesh) {
            if (_isDisposed)
                return;

            VertexArrayObjectReference vaoReference;
            if (_meshVaoReferenceDictionary.TryGetValue(mesh, out vaoReference) == false)
                throw new ArgumentException("No vao for this mesh found");

            vaoReference.Release();
            if (vaoReference.IsDisposed) {
                var result = _meshVaoReferenceDictionary.Remove(mesh);
                Assert.True(result);
            }
        }

        public void Dispose() {
            if (_isDisposed)
                return;
            _isDisposed = true;

            _meshVaoReferenceDictionary.Clear();
        }

        internal class VertexArrayObjectReference {
            private int _refenceCount;

            public bool IsDisposed { get; private set; }

            public Mesh Mesh { get; private set; }
            public VertexArrayObject Vao { get; private set; }

            public VertexArrayObjectReference(Mesh mesh, VertexArrayObject vao) {
                Assert.NotNull(mesh);
                Assert.NotNull(vao);

                Mesh = mesh;
                Vao = vao;
                _refenceCount = 0;
                IsDisposed = false;
            }

            public void Capture() {
                Assert.True(_refenceCount >= 0);
                Assert.False(IsDisposed);

                _refenceCount++;
            }
            public void Release() {
                Assert.False(IsDisposed);
                Assert.True(_refenceCount > 0);

                _refenceCount--;

                if (_refenceCount <= 0) {
                    Vao.Dispose();
                    IsDisposed = true;
                    Log.Info(string.Format("vao for mesh {0} disposed", Mesh));
                }
            }
        }
    }
}
