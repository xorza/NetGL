using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using System;

namespace NetGL.Core.Meshes {
    public static class Ellipse {
        public static Mesh Create() {
            return Create(Quaternion.Identity, Vector2.One);
        }

        public static Mesh Create(Quaternion rotation, Vector2 radiuses, int segments = 20) {
            var vertices = new Vector3Buffer(segments);
            var normals = new Vector3Buffer(segments);
            var indices = new UInt32Buffer(2 * segments);

            for (int i = 0; i < segments; i++) {
                var angle = MathF.PI * 2 * (i / (float)segments);
                var normal = Vector3.Transform(new Vector3(MathF.Sin(angle), MathF.Cos(angle), 0), rotation);
                var point = Vector3.Transform((Vector3)(new Vector2(MathF.Sin(angle), MathF.Cos(angle)) * radiuses), rotation);

                vertices[i] = point;
                indices[2 * i] = (UInt32)i;
                indices[2 * i + 1] = (UInt32)((i + 1) % segments);
                normals[i] = normal;
            }

            return new Mesh() { Normals = normals, Vertices = vertices, Indices = indices, Type = PrimitiveType.Lines };
        }
    }
}
