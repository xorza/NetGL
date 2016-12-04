using System;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;

namespace NetGL.Core.Meshes {
    public static class Cone {
        private static Vector3[] GetVertices(float radius, float height, int sideSegmentCount) {
            var n = 2 * sideSegmentCount + 2;
            var verts = new Vector3[n];

            verts[n - 2] = new Vector3(0, 0, 0);
            verts[n - 1] = new Vector3(0, height, 0);

            var angle = 0f;
            var dangle = 2 * MathF.PI / sideSegmentCount;
            for (int i = 0; i < 2 * sideSegmentCount; i += 2) {
                var x = radius * MathF.Sin(angle);
                var y = radius * MathF.Cos(angle);
                verts[i + 0] = new Vector3(x, 0f, y);
                verts[i + 1] = new Vector3(x, 0f, y);
                angle += dangle;
            }

            return verts;
        }

        private static UInt32[] GetIndices(int sideSegmentCount) {
            var n = 2 * sideSegmentCount;
            var indices = new UInt32[6 * sideSegmentCount];

            int t = -1;
            for (int i = 0; i < 2 * sideSegmentCount; i += 2) {
                indices[++t] = (UInt32)((i + 0) % n);
                indices[++t] = (UInt32)n;
                indices[++t] = (UInt32)((i + 2) % n);

                indices[++t] = (UInt32)((i + 3) % n);
                indices[++t] = (UInt32)(n + 1);
                indices[++t] = (UInt32)((i + 1) % n);
            }

            return indices;
        }

        private static Vector3[] GetNormals(float radius, float height, int sideSegmentCount) {
            var n = 2 * sideSegmentCount + 2;
            var normals = new Vector3[n];

            normals[n - 2] = new Vector3(0, -1, 0);
            normals[n - 1] = new Vector3(0, 1, 0);

            var nz = MathF.Cos(MathF.Atan2(radius, height));

            var angle = 0f;
            var dangle = 2 * MathF.PI / sideSegmentCount;
            for (int i = 0; i < 2 * sideSegmentCount; i += 2) {
                var x = MathF.Sin(angle);
                var y = MathF.Cos(angle);
                normals[i + 0] = new Vector3(0, -1, 0);
                normals[i + 1] = new Vector3(x, nz, y);

                angle += dangle;
            }

            return normals;
        }

        [Obsolete()]
        private static Vector2[] GetTexCoords(float radius, float height, int sideSegmentCount) {
            throw new NotImplementedException();

            //return new Vector2[4 * sideSegmentCount + 2]; //TODO: implement texture coords here
        }

        public static Mesh Create(float radius, float height, int sideSegmentCount = 24) {
            var mesh = new Mesh();

            mesh.Vertices = GetVertices(radius, height, sideSegmentCount);
            mesh.Normals = GetNormals(radius, height, sideSegmentCount);
            //mesh.TexCoords = GetTexCoords(radius, height, sideSegmentCount);
            mesh.Indices = GetIndices(sideSegmentCount);
            mesh.CalculateBounds();

            return mesh;
        }
    }
}