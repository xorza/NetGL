using System;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;

namespace NetGL.Core.Meshes {
    public static class TrefoilKnot {
        private const uint Slices = 128;
        private const uint Stacks = 32;

        public static Mesh Create() {
            var mesh = new Mesh();
            CreateVertexNormalBuffer(mesh);
            CreateIndexBuffer(mesh);

            mesh.CalculateBounds();
            return mesh;
        }

        private static void CreateVertexNormalBuffer(Mesh mesh) {
            var vertexCount = Slices * Stacks;

            var vertices = new Vector3Buffer((int)vertexCount);
            var normals = new Vector3Buffer((int)vertexCount);

            int count = 0;

            float ds = 1.0f / Slices;
            float dt = 1.0f / Stacks;

            // The upper bounds in these loops are tweaked to reduce the
            // chance of precision error causing an incorrect # of iterations.

            for (float s = 0; s < 1 - ds / 2; s += ds) {
                for (float t = 0; t < 1 - dt / 2; t += dt) {
                    const float E = 0.01f;
                    var p = EvaluateTrefoil(s, t);
                    var u = EvaluateTrefoil(s + E, t) - p;
                    var v = EvaluateTrefoil(s, t + E) - p;
                    var n = Vector3.Normalize(Vector3.Cross(u, v));
                    vertices[count] = p;
                    normals[count] = n;
                    count++;
                }
            }

            mesh.Vertices = vertices;
            mesh.Normals = normals;
        }

        private static void CreateIndexBuffer(Mesh mesh) {
            const uint vertexCount = Slices * Stacks;
            const uint indexCount = vertexCount * 6;
            var indices = new UInt32Buffer(indexCount);
            int count = 0;

            ushort n = 0;
            for (ushort i = 0; i < Slices; i++) {
                for (ushort j = 0; j < Stacks; j++) {
                    indices[count++] = (UInt32)((n + j + Stacks) % vertexCount);
                    indices[count++] = (UInt32)(n + (j + 1) % Stacks);
                    indices[count++] = (UInt32)(n + j);

                    indices[count++] = (UInt32)((n + (j + 1) % Stacks + Stacks) % vertexCount);
                    indices[count++] = (UInt32)((n + (j + 1) % Stacks) % vertexCount);
                    indices[count++] = (UInt32)((n + j + Stacks) % vertexCount);
                }

                n += (ushort)Stacks;
            }

            mesh.Indices = indices;
        }

        private static Vector3 EvaluateTrefoil(float s, float t) {
            const float TwoPi = (float)Math.PI * 2;

            var a = 0.5f;
            var b = 0.3f;
            var c = 0.5f;
            var d = 0.1f;
            var u = (1 - s) * 2 * TwoPi;
            var v = t * TwoPi;
            var r = (float)(a + b * Math.Cos(1.5f * u));
            var x = (float)(r * Math.Cos(u));
            var y = (float)(r * Math.Sin(u));
            var z = (float)(c * Math.Sin(1.5f * u));

            var dv = new Vector3();
            dv.X =
                (float)
                    (-1.5f * b * Math.Sin(1.5f * u) * Math.Cos(u) -
                     (a + b * Math.Cos(1.5f * u)) * Math.Sin(u));
            dv.Y =
                (float)
                    (-1.5f * b * Math.Sin(1.5f * u) * Math.Sin(u) +
                     (a + b * Math.Cos(1.5f * u)) * Math.Cos(u));
            dv.Z = (float)(1.5f * c * Math.Cos(1.5f * u));

            dv.Normalize();
            var qvn = new Vector3(dv.Y, -dv.X, 0.0f);
            qvn.Normalize();
            var ww = Vector3.Cross(dv, qvn);

            var range = new Vector3();
            range.X = (float)(x + d * (qvn.X * Math.Cos(v) + ww.X * Math.Sin(v)));
            range.Y = (float)(y + d * (qvn.Y * Math.Cos(v) + ww.Y * Math.Sin(v)));
            range.Z = (float)(z + d * ww.Z * Math.Sin(v));

            return range;
        }
    }
}