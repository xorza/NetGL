using System;
using System.Linq;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;

namespace NetGL.Core.Meshes {
    public static class Plane {
        public static Mesh Create(float width, float height) {
            var vertices = new Vector3Buffer(8);
            vertices[0] = new Vector3(-width, -height, 0) / 2;
            vertices[1] = new Vector3(-width, height, 0) / 2;
            vertices[2] = new Vector3(width, height, 0) / 2;
            vertices[3] = new Vector3(width, -height, 0) / 2;

            vertices[4] = new Vector3(-width, -height, 0) / 2;
            vertices[5] = new Vector3(-width, height, 0) / 2;
            vertices[6] = new Vector3(width, height, 0) / 2;
            vertices[7] = new Vector3(width, -height, 0) / 2;

            var normals = new Vector3Buffer(8);
            normals[0] = new Vector3(0, 0, 1);
            normals[1] = new Vector3(0, 0, 1);
            normals[2] = new Vector3(0, 0, 1);
            normals[3] = new Vector3(0, 0, 1);

            normals[4] = new Vector3(0, 0, -1);
            normals[5] = new Vector3(0, 0, -1);
            normals[6] = new Vector3(0, 0, -1);
            normals[7] = new Vector3(0, 0, -1);

            var tangents = new Vector3Buffer(8);
            tangents[0] = new Vector3(0, 0, 1);
            tangents[1] = new Vector3(0, 0, 1);
            tangents[2] = new Vector3(0, 0, 1);
            tangents[3] = new Vector3(0, 0, 1);

            tangents[4] = new Vector3(0, 0, -1);
            tangents[5] = new Vector3(0, 0, -1);
            tangents[6] = new Vector3(0, 0, -1);
            tangents[7] = new Vector3(0, 0, -1);

            var texCoords = new Vector2Buffer(8);
            texCoords[0] = Vector2.Zero;
            texCoords[1] = new Vector2(0, 1);
            texCoords[2] = Vector2.One;
            texCoords[3] = new Vector2(1, 0);

            texCoords[4] = Vector2.Zero;
            texCoords[5] = new Vector2(0, 1);
            texCoords[6] = Vector2.One;
            texCoords[7] = new Vector2(1, 0);

            UInt32Buffer indices = new UInt32[]
            {
                2, 1, 0,
                3, 2, 0,
                4, 5, 6,
                4, 6, 7
            };

            var mesh = new Mesh();
            mesh.Vertices = vertices;
            mesh.Normals = normals;
            mesh.TexCoords = texCoords;
            mesh.Indices = indices;
            mesh.Tangents = Enumerable.Repeat(new Vector3(1, 0, 0), 8).ToArray();

            mesh.CalculateBounds();
            return mesh;
        }
    }
}