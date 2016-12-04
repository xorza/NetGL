using System.Collections.Generic;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using System;

namespace NetGL.Core.Meshes {
    public class Icosahedron : Mesh {
        private static int GetMidpointIndex(List<Vector3> vertices, Int32 i0, Int32 i1) {
            var midpointIndex = -1;

            var v0 = vertices[i0];
            var v1 = vertices[i1];

            var midpoint = (v0 + v1) / 2f;

            if (vertices.Contains(midpoint))
                midpointIndex = vertices.IndexOf(midpoint);
            else {
                midpointIndex = vertices.Count;
                vertices.Add(midpoint);
            }

            return midpointIndex;
        }

        /// <remarks>
        ///     i0
        ///     /  \
        ///     m02-m01
        ///     /  \ /  \
        ///     i2---m12---i1
        /// </remarks>
        private static void Subdivide(List<Vector3> vertices, List<Int32> indices, bool removeSourceTriangles) {
            var newIndices = new List<Int32>(indices.Count * 4);

            if (!removeSourceTriangles)
                newIndices.AddRange(indices);

            for (var i = 0; i < indices.Count - 2; i += 3) {
                var i0 = indices[i];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];

                var m01 = GetMidpointIndex(vertices, i0, i1);
                var m12 = GetMidpointIndex(vertices, i1, i2);
                var m02 = GetMidpointIndex(vertices, i2, i0);

                newIndices.AddRange(new Int32[]
                {
                    i0, m01, m02
                    ,
                    i1, m12, m01
                    ,
                    i2, m02, m12
                    ,
                    m02, m01, m12
                });
            }

            indices.Clear();
            indices.AddRange(newIndices);
        }

        /// <summary>
        ///     create a regular icosahedron (20-sided polyhedron)
        /// </summary>
        /// <remarks>
        ///     You can create this programmatically instead of using the given vertex
        ///     and index list, but it's kind of a pain and rather pointless beyond a
        ///     learning exercise.
        /// </remarks>
        private static void Create(List<Vector3> vertices, List<Int32> indices) {
            indices.Clear();
            vertices.Clear();

            indices.AddRange(
                new int[]
                {
                    1, 4, 0,
                    4, 9, 0,
                    4, 5, 9,
                    8, 5, 4,
                    1, 8, 4,
                    1, 10, 8,
                    10, 3, 8,
                    8, 3, 5,
                    3, 2, 5,
                    3, 7, 2,
                    3, 10, 7,
                    10, 6, 7,
                    6, 11, 7,
                    6, 0, 11,
                    6, 1, 0,
                    10, 1, 6,
                    11, 0, 9,
                    2, 11, 9,
                    5, 2, 9,
                    11, 2, 7,
                });

            var X = 0.525731112119133606f;
            var Z = 0.850650808352039932f;

            vertices.AddRange(new[]
            {
                new Vector3(-X, 0f, Z),
                new Vector3(X, 0f, Z),
                new Vector3(-X, 0f, -Z),
                new Vector3(X, 0f, -Z),
                new Vector3(0f, Z, X),
                new Vector3(0f, Z, -X),
                new Vector3(0f, -Z, X),
                new Vector3(0f, -Z, -X),
                new Vector3(Z, X, 0f),
                new Vector3(-Z, X, 0f),
                new Vector3(Z, -X, 0f),
                new Vector3(-Z, -X, 0f)
            });
        }

        public static Mesh Create(float radius, int iterations = 0) {
            var vertices = new List<Vector3>();
            var indices = new List<Int32>();

            Create(vertices, indices);

            for (int i = 0; i < iterations; i++)
                Subdivide(vertices, indices, true);

            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = vertices[i].Normalized * radius;

            var result = new Mesh();

            result.Vertices = new Vector3Buffer(vertices);
            result.Normals = new Vector3Buffer(vertices);
            result.Indices = indices;

            //var texCoords = new Vector2Buffer(vertices.Count);
            //for (int i = 0; i < vertices.Count; i++) {
            //    var p = vertices[i];
            //    var u = Vector3.Angle(new Vector3(0, 1, 0), new Vector3(0, p.Y, 0));
            //    var v = Vector3.SignedAngle(new Vector3(1, 0, 0), new Vector3(p.X, 0, p.Z), new Vector3(0, 1, 0));

            //    if (float.IsNaN(u))
            //        u = 0;
            //    if (float.IsNaN(v))
            //        v = 0;

            //    texCoords[i] = new Vector2(u, v);
            //}

            result.Bounds = new BoundingVolume(Vector3.Zero, Vector3.One * (2 * radius), radius);
            return result;
        }
    }
}