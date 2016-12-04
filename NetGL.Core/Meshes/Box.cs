using System;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using System.Linq;

namespace NetGL.Core.Meshes {
    public static class Box {

        public static Mesh CreateLines() {
            return CreateLines(Vector3.One);
        }
        public static Mesh CreateLines(Vector3 size) {
            var result = new Mesh();
            var halfSize = size * 0.5f;

            var vertices = new Vector3Buffer(8);
            vertices[0] = halfSize * new Vector3(-1, -1, -1);
            vertices[1] = halfSize * new Vector3(-1, 1, -1);
            vertices[2] = halfSize * new Vector3(1, 1, -1);
            vertices[3] = halfSize * new Vector3(1, -1, -1);

            vertices[4] = halfSize * new Vector3(-1, -1, 1);
            vertices[5] = halfSize * new Vector3(-1, 1, 1);
            vertices[6] = halfSize * new Vector3(1, 1, 1);
            vertices[7] = halfSize * new Vector3(1, -1, 1);

            UInt32Buffer indices = new UInt32[24]
            {
                0, 1,
                1, 2,
                2, 3,
                3, 0,
                4, 5,
                5, 6,
                6, 7,
                7, 4,
                0, 4,
                1, 5,
                2, 6,
                3, 7
            };

            result.Vertices = vertices;
            result.Indices = indices;
            result.Type = PrimitiveType.Lines;
            result.CalculateBounds();
            return result;
        }
    }
}