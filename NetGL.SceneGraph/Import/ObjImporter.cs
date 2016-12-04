using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NetGL.SceneGraph.Import {
    public class ObjImporter {
        internal struct Point {
            public int Vertex;
            public int Normal;
            public int Texture;

            public Point(int vertex, int normal, int texture) {
                this.Vertex = vertex;
                this.Normal = normal;
                this.Texture = texture;
            }
        }

        public Mesh LoadStream(Stream stream) {
            var reader = new StreamReader(stream);
            using (reader) {
                var vertices = new List<Vector3>();
                var normals = new List<Vector3>();
                var texCoords = new List<Vector2>();
                var points = new List<Point>();

                string line;
                char[] splitChars = { ' ' };
                while ((line = reader.ReadLine()) != null) {
                    line = line.Trim(splitChars);
                    line = line.Replace("  ", " ");

                    var parameters = line.Split(splitChars);

                    switch (parameters[0]) {
                        case "p": // Point
                            break;

                        case "v": // Vertex
                            float x = ParseFloat(parameters[1]);
                            float y = ParseFloat(parameters[2]);
                            float z = ParseFloat(parameters[3]);
                            vertices.Add(new Vector3(x, y, z));
                            break;

                        case "vt": // TexCoord
                            float u = ParseFloat(parameters[1]);
                            float v = ParseFloat(parameters[2]);
                            texCoords.Add(new Vector2(u, v));
                            break;

                        case "vn": // Normal
                            float nx = ParseFloat(parameters[1]);
                            float ny = ParseFloat(parameters[2]);
                            float nz = ParseFloat(parameters[3]);
                            normals.Add(new Vector3(nx, ny, nz));
                            break;

                        case "f": // Face
                            points.AddRange(ParseFace(parameters));
                            break;
                    }
                }


                var vertexBuffer = new Vector3Buffer(points.Count);
                var normalBuffer = new Vector3Buffer(points.Count);
                var textCoordBuffer = new Vector2Buffer(points.Count);

                for (int i = 0; i < points.Count; i++) {
                    var p = points[i];

                    vertexBuffer[i] = vertices[p.Vertex];
                    normalBuffer[i] = normals[p.Normal];
                    textCoordBuffer[i] = texCoords[p.Texture];
                }


                var result = new Mesh();
                result.Vertices = vertexBuffer;
                result.TexCoords = textCoordBuffer;
                result.CalculateBounds();

                if (normalBuffer.Length == 0)
                    result.CalculateNormals();
                else
                    result.Normals = normalBuffer;

                return result;
            }
        }

        public Mesh LoadFile(string file) {
            using (FileStream s = File.Open(file, FileMode.Open))
                return LoadStream(s);
        }

        private static float ParseFloat(string str) {
            str = str.Replace(',', '.');
            return float.Parse(str, CultureInfo.InvariantCulture);
        }

        private static Point[] ParseFace(string[] indices) {
            var points = new Point[indices.Length - 1];
            for (int i = 0; i < points.Length; i++)
                points[i] = ParsePoint(indices[i + 1]);

            return points;
        }

        private static Point ParsePoint(string s) {
            char[] splitChars = { '/' };
            string[] parameters = s.Split(splitChars);

            int vert, tex = 0, norm = 0;
            vert = int.Parse(parameters[0]) - 1;
            if (parameters.Length > 1)
                tex = int.Parse(parameters[1]) - 1;
            if (parameters.Length > 2)
                norm = int.Parse(parameters[2]) - 1;
            return new Point(vert, norm, tex);
        }
    }
}