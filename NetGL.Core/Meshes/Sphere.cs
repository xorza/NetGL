using System;
using System.Collections.Generic;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;

namespace NetGL.Core.Meshes {
    public static class Sphere {
        public static Mesh Create(float radius, int m, int n) {
            if (n < 3)
                throw new ArgumentOutOfRangeException("n");
            if (m < 3)
                throw new ArgumentOutOfRangeException("m");

            var pi = MathF.PI;

            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var tangents= new List<Vector3>();
            var texCoords = new List<Vector2>();

            for (int i = 0; i <= m; i++) {
                for (int j = 0; j <= n; j++) {
                    var m1 = i / (float)m;
                    var n1 = j / (float)n;

                    var v = new Vector3(MathF.Sin(pi * m1) * MathF.Cos(2 * pi * n1), MathF.Cos(pi * m1), MathF.Sin(pi * m1) * MathF.Sin(2 * pi * n1));                    
                    v.Normalize();
                    var tangent = Vector3.Cross(new Vector3(0, 1, 0), v);

                    tangents.Add(tangent);
                    normals.Add(v);
                    vertices.Add(radius * v);
                    texCoords.Add(new Vector2(1 - n1, m1));
                }
            }

            var indices = new List<UInt32>();
            for (int i = 0; i < m; i++) {
                for (int j = 0; j <= n; j++) {
                    indices.Add((ushort)((n+1) * i + j));
                    indices.Add((ushort)((n+1) * i + (j + 1) ));
                    indices.Add((ushort)((n+1) * (i + 1) + j));

                    indices.Add((ushort)((n+1) * (i + 1) + j));
                    indices.Add((ushort)((n+1) * i + (j + 1) ));
                    indices.Add((ushort)((n+1) * (i + 1) + (j + 1)));
                }
            }

            return new Mesh() {
                Vertices = vertices,
                Normals = normals,
                Indices = indices,
                Tangents = tangents,
                TexCoords = texCoords,
                DrawStyle = PolygonMode.Fill,
                FrontFace = FrontFaceDirection.CounterClockwise,
                Type = PrimitiveType.Triangles
            };
        }
    }
}
