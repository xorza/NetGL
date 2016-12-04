using System.Linq;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using System;

namespace NetGL.Core.Meshes {
    public class Grid {
        public static Mesh Create(int gridCount, float width, bool generateTexCoords = false) {
            var result = new Mesh();

            var vertices = new Vector3Buffer(gridCount * 4);
            result.Type = PrimitiveType.Lines;
            result.Vertices = vertices;
            result.Normals = Enumerable
                .Repeat(new Vector3(0, 1, 0), gridCount * 4)
                .ToArray();

            for (int i = 0; i < gridCount; i++) {
                var offset = i / (gridCount - 1f) - 0.5f;

                vertices[i * 2] = new Vector3(offset * width, 0, -width / 2f);
                vertices[i * 2 + 1] = new Vector3(offset * width, 0, width / 2f);

                vertices[i * 2 + gridCount * 2] = new Vector3(-width / 2f, 0, offset * width);
                vertices[i * 2 + 1 + gridCount * 2] = new Vector3(width / 2f, 0, offset * width);
            }

            if (generateTexCoords) {
                var texCoords = new Vector2Buffer(gridCount * 4);
                for (int i = 0; i < gridCount; i++) {
                    var offset = i / (gridCount - 1f);

                    texCoords[i * 2] = new Vector2(offset, 0);
                    texCoords[i * 2 + 1] = new Vector2(offset, 1);

                    texCoords[i * 2 + gridCount * 2] = new Vector2(0, offset);
                    texCoords[i * 2 + 1 + gridCount * 2] = new Vector2(1, offset);
                }

                result.TexCoords = texCoords;
            }

            result.CalculateBounds();
            return result;
        }

        public static Mesh Create1(int gridCount, float width) {
            var result = new Mesh();

            var vertices = new Vector3Buffer(gridCount * gridCount);
            var texCoords = new Vector2Buffer(gridCount * gridCount);
            result.Type = PrimitiveType.Lines;
            result.TexCoords = texCoords;
            result.Vertices = vertices;
            result.Normals = Enumerable
                .Repeat(new Vector3(0, 1, 0), gridCount * gridCount)
                .ToArray();

            for (int i = 0; i < gridCount; i++)
                for (int j = 0; j < gridCount; j++)
                    vertices[i * gridCount + j] = 0.5f * width *
                                                new Vector3(i / (gridCount - 1f) - 0.5f, 0, j / (gridCount - 1f) - 0.5f);
            for (int i = 0; i < gridCount; i++)
                for (int j = 0; j < gridCount; j++)
                    texCoords[i * gridCount + j] = new Vector2(i / (gridCount - 1f), j / (gridCount - 1f));

            int index = 0;
            var indices = new UInt32Buffer(4 * gridCount * (gridCount - 1));
            for (int j = 0; j < gridCount; j++)
                for (int i = 0; i < gridCount - 1; i++) {
                    indices[index++] = (UInt32)(i + j * gridCount);
                    indices[index++] = (UInt32)(i + 1 + j * gridCount);
                }

            for (int j = 0; j < gridCount; j++)
                for (int i = 0; i < gridCount - 1; i++) {
                    indices[index++] = (UInt32)(j + i * gridCount);
                    indices[index++] = (UInt32)(j + (i + 1) * gridCount);
                }
            result.Indices = indices;

            result.CalculateBounds();
            return result;
        }
    }
}