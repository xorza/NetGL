using System;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;

namespace NetGL.Core.Meshes {
    public enum CloudShape {
        Sphere,
        Cube
    }

    public static class Cloud {
        public static Mesh Create(CloudShape shape, float diameter = 1, int vertexCount = 300000) {
            var vertices = new Vector3Buffer(vertexCount);
            var normals = new Vector3Buffer(vertexCount);
            var texCoords = new Vector2Buffer(vertices.Length);

            for (int i = 0; i < vertices.Length; i++) {
                Vector3 point;
                switch (shape) {
                    case CloudShape.Sphere:
                        point = RandomF.InsideUnitSphere();
                        break;
                    case CloudShape.Cube:
                        point = RandomF.InsideUnitCube();
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                vertices[i] = point * diameter;
                normals[i] = point.Normalized;
                texCoords[i] = new Vector2(vertices[i].X + 0.5f, -vertices[i].Y + 0.5f);
            }

            var result = new Mesh() {
                Vertices = vertices,
                TexCoords = texCoords,
                Normals = normals,
                Type = PrimitiveType.Points
            };
            result.CalculateBounds();

            return result;
        }
    }
}