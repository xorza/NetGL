using System;
using System.Linq;
using NetGL.Core.Mathematics;
using System.Collections.Generic;
using NetGL.Core.Infrastructure;
using System.IO;

namespace NetGL.Core.Types {
    public class Mesh : IDisposable {
        private Vector3Buffer _vertices, _colors, _normals, _tangents;
        private Vector2Buffer _texCoords;
        private BoundingVolume _bounds;

        public Vector3Buffer Vertices {
            get { return _vertices; }
            set {
                if (_vertices == value)
                    return;

                _vertices = value;
            }
        }
        public Vector3Buffer Tangents {
            get { return _tangents; }
            set {
                if (_tangents == value)
                    return;

                _tangents = value;
            }
        }
        public Vector3Buffer Colors {
            get { return _colors; }
            set {
                if (_colors == value)
                    return;

                _colors = value;
            }
        }
        public Vector3Buffer Normals {
            get { return _normals; }
            set {
                if (_normals == value)
                    return;

                _normals = value;
            }
        }
        public Vector2Buffer TexCoords {
            get { return _texCoords; }
            set {
                if (_texCoords == value)
                    return;

                _texCoords = value;
            }
        }

        public UInt32Buffer Indices { get; set; }

        public PrimitiveType Type { get; set; }
        public PolygonMode DrawStyle { get; set; }
        public FrontFaceDirection FrontFace { get; set; }

        public string Name { get; set; }

        public BoundingVolume Bounds {
            get {
                if (_bounds == null) {
                    CalculateBounds();
                }

                return _bounds;
            }
            set {
                if (ReferenceEquals(_bounds, value))
                    return;

                _bounds = value;
            }
        }

        public Mesh() {
            Type = PrimitiveType.Triangles;
            DrawStyle = PolygonMode.Fill;
            FrontFace = FrontFaceDirection.CounterClockwise;
        }

        public void CalculateBounds() {
            CheckIfValid();

            var min = _vertices[0];
            var max = _vertices[0];

            for (int i = 1; i < _vertices.Length; i++) {
                var v = _vertices[i];

                min.X = Math.Min(min.X, v.X);
                min.Y = Math.Min(min.Y, v.Y);
                min.Z = Math.Min(min.Z, v.Z);

                max.X = Math.Max(max.X, v.X);
                max.Y = Math.Max(max.Y, v.Y);
                max.Z = Math.Max(max.Z, v.Z);
            }

            var center = (max + min) / 2;
            var aabbSize = max - min;
            var sphereRadius = _vertices.Max(_ => Vector3.Distance(center, _));

            _bounds = new BoundingVolume(center, aabbSize, sphereRadius);
        }
        public void CheckIfValid() {
            UInt32 MaximumVertices = UInt32.MaxValue;
            int MinimumVertices = 3;

            Assert.NotNull(Vertices);
            Assert.True(Vertices.Length <= MaximumVertices);
            Assert.True(Vertices.Length >= MinimumVertices);

            if (Tangents != null)
                Assert.True(Tangents.Length == Vertices.Length);

            if (Colors != null)
                Assert.True(Colors.Length == Vertices.Length);

            if (Normals != null)
                Assert.True(Normals.Length == Vertices.Length);

            if (TexCoords != null)
                Assert.True(TexCoords.Length == Vertices.Length);
        }

        [Obsolete("not yet implemented")]
        public void Optimize() {
            throw new NotImplementedException();

            //if (Type != PrimitiveType.Triangles)
            //    throw new NotSupportedException(Type.ToString());

            //if (Indices != null)
            //    throw new NotSupportedException("Optimization of indexed meshes is not yet supported");

            //var newIndices = new List<Int32>(Vertices.Length);
            //var newVertices = new List<Vector3>(Vertices.Length);

            //for (int i = 0; i < Vertices.Length; i++) {
            //    var v = Vertices[i];
            //    var index = newVertices.IndexOf(v);
            //    if (index != -1)
            //        newIndices.Add(index);
            //    else {
            //        newIndices.Add(newVertices.Count);
            //        newVertices.Add(v);
            //    }
            //}

            //Vertices.Dispose();
            //Indices.Dispose();

            //Vertices = newVertices;
            //Indices = newIndices;
        }

        public void Transform(Matrix matrix) {
            for (int i = 0; i < _vertices.Length; i++)
                _vertices[i] = Vector3.Transform(_vertices[i], matrix);

            if (_normals != null)
                for (int i = 0; i < _normals.Length; i++)
                    _normals[i] = Vector3.NormalTransform(_normals[i], matrix);

            if (_tangents != null)
                for (int i = 0; i < _tangents.Length; i++)
                    _tangents[i] = Vector3.NormalTransform(_tangents[i], matrix);

            CalculateBounds();
        }
        public void CalculateNormals() {
            Disposer.Dispose(ref _normals);
            _normals = new Vector3Buffer(_vertices.Length);

            if (Indices != null) {
                var normalsList = new List<Vector3>[_vertices.Length];
                var indices = Indices;

                for (int i = 0; i < indices.Length; i++) {
                    Int32 pindex = (Int32)indices[i];
                    Int32 aindex, bindex;

                    switch (i % 3) {
                        case 0:
                            aindex = (Int32)indices[i + 1];
                            bindex = (Int32)indices[i + 2];
                            break;
                        case 1:
                            aindex = (Int32)indices[i + 1];
                            bindex = (Int32)indices[i - 1];
                            break;
                        case 2:
                            aindex = (Int32)indices[i - 2];
                            bindex = (Int32)indices[i - 1];
                            break;

                        default:
                            throw new ArithmeticException();
                    }

                    var vertexNormals = normalsList[pindex];
                    if (vertexNormals == null)
                        vertexNormals = normalsList[pindex] = new List<Vector3>(1);

                    var p = _vertices[pindex];
                    var a = _vertices[aindex];
                    var b = _vertices[bindex];

                    var normal = Vector3.Cross(a - p, b - p).Normalized;
                    vertexNormals.Add(normal);
                }
                for (int i = 0; i < _vertices.Length; i++)
                    _normals[i] = normalsList[i]
                        .Sum()
                        .Normalized;
            } else {
                for (int i = 0; i < _vertices.Length; i++) {
                    var pindex = i;
                    int aindex, bindex;

                    switch (i % 3) {
                        case 0:
                            aindex = i + 1;
                            bindex = i + 2;
                            break;
                        case 1:
                            aindex = i + 1;
                            bindex = i - 1;
                            break;
                        case 2:
                            aindex = i - 2;
                            bindex = i - 1;
                            break;

                        default:
                            throw new ArithmeticException();
                    }

                    var p = _vertices[pindex];
                    var a = _vertices[aindex];
                    var b = _vertices[bindex];

                    _normals[i] = Vector3
                        .Cross(a - p, b - p)
                        .Normalized;
                }
            }
        }

        public void Dispose() {
            Disposer.Dispose(ref _vertices);
            Disposer.Dispose(ref _colors);
            Disposer.Dispose(ref _normals);
            Disposer.Dispose(ref _tangents);
            Disposer.Dispose(ref _texCoords);
            Disposer.Dispose(Indices);
            Indices = null;
        }

        public void ToStream(Stream stream) {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (stream.CanWrite == false)
                throw new IOException("stream is not writable");

            using (var writer = new BinaryWriter(stream))
                ToStream(writer);
        }
        public void ToStream(BinaryWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(Vertices != null);
            if (Vertices != null)
                Vertices.ToStream(writer.BaseStream);

            writer.Write(Tangents != null);
            if (Tangents != null)
                Tangents.ToStream(writer.BaseStream);

            writer.Write(Colors != null);
            if (Colors != null)
                Colors.ToStream(writer.BaseStream);

            writer.Write(Normals != null);
            if (Normals != null)
                Normals.ToStream(writer.BaseStream);

            writer.Write(TexCoords != null);
            if (TexCoords != null)
                TexCoords.ToStream(writer.BaseStream);

            writer.Write(Indices != null);
            if (Indices != null)
                Indices.ToStream(writer.BaseStream);

            writer.Write((uint)Type);
            writer.Write((uint)DrawStyle);
            writer.Write((uint)FrontFace);
            writer.WriteNullableString(Name);

            var bounds = Bounds;
            writer.Write(bounds.Center);
            writer.Write(bounds.Size);
            writer.Write(bounds.Radius);
        }
        public void FromStream(Stream stream) {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (stream.CanRead == false)
                throw new IOException("stream is not readable");

            using (var reader = new BinaryReader(stream))
                FromStream(reader);
        }
        public void FromStream(BinaryReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.ReadBoolean())
                Vertices = new Vector3Buffer(reader);
            if (reader.ReadBoolean())
                Tangents = new Vector3Buffer(reader);
            if (reader.ReadBoolean())
                Colors = new Vector3Buffer(reader);
            if (reader.ReadBoolean())
                Normals = new Vector3Buffer(reader);
            if (reader.ReadBoolean())
                TexCoords = new Vector2Buffer(reader);
            if (reader.ReadBoolean())
                Indices = new UInt32Buffer(reader);

            Type = (PrimitiveType)reader.ReadUInt32();
            DrawStyle = (PolygonMode)reader.ReadUInt32();
            FrontFace = (FrontFaceDirection)reader.ReadUInt32();
            Name = reader.ReadNullbaleString();

            var boundsCenter = reader.ReadVector3();
            var boundsSize = reader.ReadVector3();
            var boundsRadius = reader.ReadSingle();

            Bounds = new BoundingVolume(boundsCenter, boundsSize, boundsRadius);
        }

        public void CalculateTangents() {
            Disposer.Dispose(Tangents);

            Tangents = new Vector3Buffer(Vertices.Length);

            for (int i = 0; i < Vertices.Length; i += 3) {
                Vector3 v1 = Vertices[i + 0];
                Vector3 v2 = Vertices[i + 1];
                Vector3 v3 = Vertices[i + 2];

                Vector2 w1 = TexCoords[i + 0];
                Vector2 w2 = TexCoords[i + 1];
                Vector2 w3 = TexCoords[i + 2];

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float r = 1.0f / (s1 * t2 - s2 * t1);
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                Tangents[i + 0] = sdir;
                Tangents[i + 1] = sdir;
                Tangents[i + 2] = sdir;
            }

            for (int a = 0; a < Vertices.Length; a++) {
                Vector3 n = Normals[a];
                Vector3 t = Tangents[a];

                // Gram-Schmidt orthogonalize
                Tangents[a] = Vector3.Normalize((t - n * Vector3.Dot(n, t)));

                // Calculate handedness
                //tangents[a].w = (dot(cross(n, t), bitangents[a]) < 0.0F) ? -1.0F : 1.0F;
            }
        }
    }
}