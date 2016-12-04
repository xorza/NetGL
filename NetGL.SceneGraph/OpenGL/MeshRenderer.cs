using NetGL.Core;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.Scene;
using System;

namespace NetGL.SceneGraph.OpenGL {
    internal class MeshRenderer : IRenderer, IDisposable {
        private uint _verticesCount;
        private bool _isIndexed;
        private DrawElementsType _indexType;
        private PrimitiveType _primitiveType;
        private PolygonMode _polygonMode;
        private FrontFaceDirection _frontFace = FrontFaceDirection.CounterClockwise;
        private Mesh _mesh;
        private Sphere _boundSphere;
        private bool _isDisposed = false;
        private bool _shouldUpdateMesh = false;
        private VertexArrayObject _vao;

        private readonly GL _glContext;
        private readonly Graphics _graphics;
        private readonly MeshVaoCollection _sharedMeshVaoCollection;

        public Mesh Mesh {
            get { return _mesh; }
            set {
                if (value != null && value.Bounds == null)
                    throw new NullReferenceException("Mesh.Bounds");

                if (ReferenceEquals(_mesh, value))
                    return;

                ReleaseMeshVao();
                _mesh = value;

                if (_mesh != null)
                    UpdateMesh();
            }
        }
        public bool IsDisposed { get { return _isDisposed; } }
        public Sphere BoundSphere { get { return _boundSphere; } }

        public MeshRenderer() {
            _glContext = GL.GetCurrent(true);
            _graphics = Graphics.GetCurrent(true);
            _sharedMeshVaoCollection = _graphics.MeshVaoCollection;
            _vao = null;
            _isDisposed = false;

            Assert.NotNull(_glContext);
            Assert.NotNull(_graphics);
            Assert.NotNull(_sharedMeshVaoCollection);
        }

        public void UpdateMesh() {
            if (_isDisposed)
                throw new ObjectDisposedException("OpenGLMeshRenderer");
            if (_mesh == null)
                throw new NullReferenceException("Mesh");
            if (_mesh.Vertices == null)
                throw new NullReferenceException("Mesh.Vertices");

            if (_vao != null)
                _shouldUpdateMesh = true;

            _boundSphere = _mesh.Bounds.Sphere;

            _primitiveType = _mesh.Type;
            _frontFace = _mesh.FrontFace;
            _polygonMode = _mesh.DrawStyle;

            if (_mesh.Indices == null) {
                _verticesCount = (uint)_mesh.Vertices.Length;
                _isIndexed = false;
            }
            else {
                _verticesCount = (uint)_mesh.Indices.Length;
                _isIndexed = true;
                switch (_mesh.Indices.DataType) {
                    case VertexAttribPointerType.UnsignedShort:
                        _indexType = DrawElementsType.UnsignedShort;
                        break;
                    case VertexAttribPointerType.UnsignedInt:
                        _indexType = DrawElementsType.UnsignedInt;
                        break;
                    default:
                        throw new NotSupportedException("Unknown index type: " + _mesh.Indices.DataType);
                }
            }
        }

        public void Prerender() { }
        public void Render() {
            if (_isDisposed)
                throw new ObjectDisposedException("MeshRenderer");
            Assert.NotNull(_mesh);

            if (_vao == null) {
                _vao = _sharedMeshVaoCollection.Get(_mesh);
                Assert.NotNull(_vao);
                Assert.False(_vao.IsDisposed);
            }

            _glContext.PolygonMode(MaterialFace.FrontAndBack, _polygonMode);
            _glContext.FrontFace(_frontFace);

            if (_shouldUpdateMesh) {
                _shouldUpdateMesh = false;
                _vao.SetMesh(_mesh);
            }

            _vao.Bind();
            if (_isIndexed)
                _glContext.DrawElements(_primitiveType, _verticesCount, _indexType, IntPtr.Zero);
            else
                _glContext.DrawArrays(_primitiveType, 0, _verticesCount);
        }

        public void Dispose() {
            if (_isDisposed)
                return;
            _isDisposed = true;

            ReleaseMeshVao();
        }

        private void ReleaseMeshVao() {
            if (_vao == null)
                return;

            _sharedMeshVaoCollection.Release(_mesh);
            _vao = null;
            _mesh = null;
        }
    }
}