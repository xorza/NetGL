using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.Core.Meshes {
    public delegate Vector3 Vector3Transformation(Vector3 v);

    public sealed class MeshBuilder {
        private static readonly UInt32[] _boxIndices = {
                                                 0, 1, 2,
                                                 0, 2, 3,

                                                 6, 5, 4,
                                                 7, 6, 4,

                                                 5, 1, 0,
                                                 4, 5, 0,

                                                 3, 2, 6,
                                                 3, 6, 7,

                                                 2, 1, 5,
                                                 2, 5, 6,

                                                 0, 3, 7,
                                                 0, 7, 4
                                             };
        private static readonly Vector3[] _boxVertices = {
                                                    new Vector3(-1, -1, -1),
                                                    new Vector3(-1, 1, -1),
                                                    new Vector3(1, 1, -1),
                                                    new Vector3(1, -1, -1),

                                                    new Vector3(-1, -1, 1),
                                                    new Vector3(-1, 1, 1),
                                                    new Vector3(1, 1, 1),
                                                    new Vector3(1, -1, 1)
                                                  };



        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<UInt32> _indices = new List<UInt32>();
        private readonly List<Vector3> _normals = new List<Vector3>();
        private readonly List<Vector3> _colors = new List<Vector3>();

        private static readonly Vector3Transformation _identityTransformation = vector => vector;

        public MeshBuilder() {

        }

        public Mesh GetMesh() {
            var result = new Mesh();

            result.Vertices = _vertices;
            if (_indices.Count > 0)
                result.Indices = _indices;
            if (_normals.Count > 0)
                result.Normals = _normals;
            if (_colors.Count > 0)
                result.Colors = _colors;

            result.CalculateBounds();
            return result;
        }

        public void AddBox(Vector3 position, Vector3 size) {
            var newVertices = _boxVertices
                .Select(_ => (0.5f * _ * size) + position)
                .ToArray();
            var newIndices = _boxIndices
                .Select(_ => _ + (UInt32)_vertices.Count)
                .ToArray();

            _vertices.AddRange(newVertices);
            _indices.AddRange(newIndices);
        }
        public void AddBox(Matrix transform) {
            var newVertices = _boxVertices
                .Select(_ => Vector3.Transform(0.5f * _, transform))
                .ToArray();
            var newIndices = _boxIndices
                .Select(_ => _ + (UInt32)_vertices.Count)
                .ToArray();

            _vertices.AddRange(newVertices);
            _indices.AddRange(newIndices);
        }

        public void AddBoxInverted(Vector3 position, Vector3 size) {
            var newVertices = _boxVertices
                .Select(_ => (0.5f * _ * size) + position)
                .ToArray();
            var newIndices = _boxIndices
                .Reverse()
                .Select(_ => _ + (UInt32)_vertices.Count)
                .ToArray();

            _vertices.AddRange(newVertices);
            _indices.AddRange(newIndices);
        }

        public void AddMesh(Mesh mesh) {
            AddMesh(mesh, _identityTransformation, _identityTransformation);
        }

        public void AddMesh(Mesh mesh, Vector3Transformation vertexTransform, Vector3Transformation normalTransform) {
            if (mesh == null)
                throw new ArgumentNullException();

            if (mesh.Indices != null) {
                var newIndices = mesh.Indices
                    .Select(_ => _ + (UInt32)_vertices.Count)
                    .ToArray();
                _indices.AddRange(newIndices);
            }

            var vertices = mesh.Vertices
                .Select(_ => vertexTransform(_));
            _vertices.AddRange(vertices);

            if (mesh.Normals != null) {
                if (normalTransform == null)
                    throw new ArgumentNullException("normalTransform");

                var normals = mesh.Normals
                    .Select(_ => normalTransform(_));
                _normals.AddRange(normals);
            }
            if (mesh.Colors != null)
                _colors.AddRange(mesh.Colors);
        }

        public void AddMesh(Mesh mesh, Matrix matrix) {
            var vertexTransform = new Vector3Transformation(_ => Vector3.Transform(_, matrix));
            var normalTransform = new Vector3Transformation(_ => Vector3.NormalTransform(_, matrix));
            AddMesh(mesh, vertexTransform, normalTransform);
        }
        public void AddMesh(Mesh mesh, Quaternion rotation) {
            var transform = new Vector3Transformation(_ => Vector3.Transform(_, rotation));
            AddMesh(mesh, transform, transform);
        }
        public void AddMesh(Mesh mesh, Vector3 translation) {
            var transform = new Vector3Transformation(_ => _ + translation);
            AddMesh(mesh, transform, _identityTransformation);
        }
        public void AddMesh(Mesh mesh, Vector3 translation, Vector3 scale) {
            var vertexTransform = new Vector3Transformation(_ => (_ * scale) + translation);
            var normalTransform = new Vector3Transformation(_ => (scale * _).Normalized);
            AddMesh(mesh, vertexTransform, normalTransform);
        }
    }
}
