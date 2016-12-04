using System;
using System.Collections.Generic;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;

namespace NetGL.Core.Meshes {
    public static class CompoundMesh {
        public static Mesh Create(Mesh m1, Mesh m2) {
            m1.CheckIfValid();
            m2.CheckIfValid();

            Assert.True(m1.Type == m2.Type);

            var result = new Mesh();
            result.Type = m1.Type;

            var vertices = new List<Vector3>(m1.Vertices.Length + m2.Vertices.Length);
            vertices.AddRange(m1.Vertices.ToArray());
            vertices.AddRange(m2.Vertices.ToArray());
            result.Vertices = vertices;

            if (m1.Colors != null && m2.Colors != null) {
                var colors = new List<Vector3>(m1.Colors.Length + m2.Colors.Length);
                colors.AddRange(m1.Colors.ToArray());
                colors.AddRange(m2.Colors.ToArray());
                result.Colors = colors;
            }

            if (m1.Tangents != null && m2.Tangents != null) {
                var tangents = new List<Vector3>(m1.Tangents.Length + m2.Tangents.Length);
                tangents.AddRange(m1.Tangents.ToArray());
                tangents.AddRange(m2.Tangents.ToArray());
                result.Tangents = tangents;
            }

            if (m1.Normals != null && m2.Normals != null) {
                var normals = new List<Vector3>(m1.Normals.Length + m2.Normals.Length);
                normals.AddRange(m1.Normals.ToArray());
                normals.AddRange(m2.Normals.ToArray());
                result.Normals = normals;
            }

            if (m1.TexCoords != null && m2.TexCoords != null) {
                var texCoords = new List<Vector2>(m1.TexCoords.Length + m2.TexCoords.Length);
                texCoords.AddRange(m1.TexCoords.ToArray());
                texCoords.AddRange(m2.TexCoords.ToArray());
                result.TexCoords = texCoords;
            }

            if (m1.Indices != null && m2.Indices != null) {
                var indices = new List<UInt32>(m1.Indices.Length + m2.Indices.Length);
                indices.AddRange(m1.Indices.ToArray());
                var indexOffset = (UInt32)m1.Vertices.Length;
                for (int i = 0; i < m2.Indices.Length; i++)
                    indices.Add((UInt32)(m2.Indices[i] + indexOffset));

                result.Indices = indices;
            }

            result.CalculateBounds();
            return result;
        }
    }
}
