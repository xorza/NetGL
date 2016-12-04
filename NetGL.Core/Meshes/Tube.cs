using System;
using System.Collections.Generic;
using System.Linq;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;

namespace NetGL.Core.Meshes
{
    public static class Tube
    {
        private static IEnumerable<int> GetIndices(int sideSegmentCount)
        {
            #region outer side

            int i = 0;
            for (; i < sideSegmentCount*2 - 1; i += 2)
            {
                yield return i;
                yield return i + 1;
                yield return i + 3;

                yield return i;
                yield return i + 3;
                yield return i + 2;
            }

            yield return i;
            yield return i + 1;
            yield return 1;

            yield return i;
            yield return 1;
            yield return 0;
            i += 2;

            #endregion

            #region inner side

            int k = i;
            for (; i < sideSegmentCount*2 - 1 + k; i += 2)
            {
                yield return i + 3;
                yield return i + 1;
                yield return i;

                yield return i + 2;
                yield return i + 3;
                yield return i;
            }

            yield return k + 1;
            yield return i + 1;
            yield return i;

            yield return k;
            yield return k + 1;
            yield return i;
            i += 2;

            #endregion

            #region top

            k = i;
            for (; i < sideSegmentCount*2 - 1 + k; i += 2)
            {
                yield return i;
                yield return i + 1;
                yield return i + 3;

                yield return i;
                yield return i + 3;
                yield return i + 2;
            }

            yield return i;
            yield return i + 1;
            yield return k + 1;

            yield return i;
            yield return k + 1;
            yield return k;
            i += 2;

            #endregion

            #region bottom

            k = i;
            for (; i < sideSegmentCount*2 - 1 + k; i += 2)
            {
                yield return i + 3;
                yield return i + 1;
                yield return i;

                yield return i + 2;
                yield return i + 3;
                yield return i;
            }

            yield return k + 1;
            yield return i + 1;
            yield return i;

            yield return k;
            yield return k + 1;
            yield return i;

            #endregion
        }

        private static Vector3[] GetNormals(int sideSegmentCount)
        {
            var normals = new Vector3[sideSegmentCount*8 + 8];
            double fi;
            int i;
            int j = 0;

            #region outer side

            for (i = 0, fi = 0; i <= sideSegmentCount; i++, fi = Math.PI*2*i/sideSegmentCount)
            {
                var normal = new Vector3(
                    (float) (Math.Cos(fi)),
                    0f,
                    (float) (Math.Sin(fi))
                    );

                normals[j++] = normal;
                normals[j++] = normal;
            }

            #endregion

            #region inner side

            for (i = 0, fi = 0; i <= sideSegmentCount; i++, fi = Math.PI*2*i/sideSegmentCount)
            {
                var normal = new Vector3(
                    -(float) (Math.Cos(fi)),
                    0f,
                    -(float) (Math.Sin(fi))
                    );

                normals[j++] = normal;
                normals[j++] = normal;
            }

            #endregion

            #region top

            for (i = 0, fi = 0; i <= sideSegmentCount; i++, fi = Math.PI*2*i/sideSegmentCount)
            {
                var normal = new Vector3(0, 1, 0);
                normals[j++] = normal;
                normals[j++] = normal;
            }

            #endregion

            #region bottom

            for (i = 0, fi = 0; i <= sideSegmentCount; i++, fi = Math.PI*2*i/sideSegmentCount)
            {
                var normal = new Vector3(0, -1, 0);
                normals[j++] = normal;
                normals[j++] = normal;
            }

            #endregion

            return normals;
        }

        private static Vector3[] GetVertices(float innerRadius, float outerRadius, float height, int sideSegmentCount)
        {
            var vertices = new Vector3[sideSegmentCount*8 + 8];

            double fi;
            int i;
            int j = 0;

            #region outer side

            for (i = 0, fi = 0; i <= sideSegmentCount; i++, fi = Math.PI*2*i/sideSegmentCount)
            {
                vertices[j].X = (float) (outerRadius*Math.Cos(fi));
                vertices[j].Y = -height/2;
                vertices[j].Z = (float) (outerRadius*Math.Sin(fi));
                j++;

                vertices[j].X = (float) (outerRadius*Math.Cos(fi));
                vertices[j].Y = height/2;
                vertices[j].Z = (float) (outerRadius*Math.Sin(fi));
                j++;
            }

            #endregion

            #region inner side

            for (i = 0, fi = 0; i <= sideSegmentCount; i++, fi = Math.PI*2*i/sideSegmentCount)
            {
                vertices[j].X = (float) (innerRadius*Math.Cos(fi));
                vertices[j].Y = -height/2;
                vertices[j].Z = (float) (innerRadius*Math.Sin(fi));
                j++;

                vertices[j].X = (float) (innerRadius*Math.Cos(fi));
                vertices[j].Y = height/2;
                vertices[j].Z = (float) (innerRadius*Math.Sin(fi));
                j++;
            }

            #endregion

            #region top

            for (i = 0, fi = 0; i <= sideSegmentCount; i++, fi = Math.PI*2*i/sideSegmentCount)
            {
                vertices[j].X = (float) (outerRadius*Math.Cos(fi));
                vertices[j].Y = height/2;
                vertices[j].Z = (float) (outerRadius*Math.Sin(fi));
                j++;

                vertices[j].X = (float) (innerRadius*Math.Cos(fi));
                vertices[j].Y = height/2;
                vertices[j].Z = (float) (innerRadius*Math.Sin(fi));
                j++;
            }

            #endregion

            #region bottom

            for (i = 0, fi = 0; i <= sideSegmentCount; i++, fi = Math.PI*2*i/sideSegmentCount)
            {
                vertices[j].X = (float) (outerRadius*Math.Cos(fi));
                vertices[j].Y = -height/2;
                vertices[j].Z = (float) (outerRadius*Math.Sin(fi));
                j++;

                vertices[j].X = (float) (innerRadius*Math.Cos(fi));
                vertices[j].Y = -height/2;
                vertices[j].Z = (float) (innerRadius*Math.Sin(fi));
                j++;
            }

            #endregion

            return vertices;
        }

        private static Vector2[] GetTexCoords(float innerRadius, float outerRadius, int sideSegmentCount)
        {
            var uvs = new Vector2[sideSegmentCount*8 + 8];
            var innerOuterRatio = innerRadius/outerRadius;

            int i;
            int j = 0;
            double phi;

            #region outer side

            for (i = 0, phi = 0; i <= sideSegmentCount; i++, phi = Math.PI*2*i/sideSegmentCount)
            {
                uvs[j].X = ((float) Math.Cos(phi) + 1)/2;
                uvs[j].Y = ((float) Math.Sin(phi) + 1)/2;
                j++;

                uvs[j].X = ((float) Math.Cos(phi) + 1)/2;
                uvs[j].Y = ((float) Math.Sin(phi) + 1)/2;
                j++;
            }

            #endregion

            #region inner side

            for (i = 0, phi = 0; i <= sideSegmentCount; i++, phi = Math.PI*2*i/sideSegmentCount)
            {
                uvs[j].X = ((float) Math.Cos(phi))*0.5f*innerOuterRatio + 0.5f;
                uvs[j].Y = ((float) Math.Sin(phi))*0.5f*innerOuterRatio + 0.5f;
                j++;

                uvs[j].X = ((float) Math.Cos(phi))*0.5f*innerOuterRatio + 0.5f;
                uvs[j].Y = ((float) Math.Sin(phi))*0.5f*innerOuterRatio + 0.5f;
                j++;
            }

            #endregion

            #region top

            for (i = 0, phi = 0; i <= sideSegmentCount; i++, phi = Math.PI*2*i/sideSegmentCount)
            {
                uvs[j].X = ((float) Math.Cos(phi) + 1)/2;
                uvs[j].Y = ((float) Math.Sin(phi) + 1)/2;
                j++;

                uvs[j].X = ((float) Math.Cos(phi))*0.5f*innerOuterRatio + 0.5f;
                uvs[j].Y = ((float) Math.Sin(phi))*0.5f*innerOuterRatio + 0.5f;
                j++;
            }

            #endregion

            #region bottom

            for (i = 0, phi = 0; i <= sideSegmentCount; i++, phi = Math.PI*2*i/sideSegmentCount)
            {
                uvs[j].X = ((float) Math.Cos(phi) + 1)/2;
                uvs[j].Y = ((float) Math.Sin(phi) + 1)/2;
                j++;

                uvs[j].X = ((float) Math.Cos(phi))*0.5f*innerOuterRatio + 0.5f;
                uvs[j].Y = ((float) Math.Sin(phi))*0.5f*innerOuterRatio + 0.5f;
                j++;
            }

            #endregion

            return uvs;
        }

        public static Mesh Create(float innerRadius, float outerRadius, float height, int sideSegmentCount = 20)
        {
            var mesh = new Mesh();

            mesh.Vertices = GetVertices(innerRadius, outerRadius, height, sideSegmentCount);
            mesh.Normals = GetNormals(sideSegmentCount);
            mesh.TexCoords = GetTexCoords(innerRadius, outerRadius, sideSegmentCount);
            mesh.Indices = GetIndices(sideSegmentCount).ToArray();
            mesh.CalculateBounds();

            mesh.FrontFace = FrontFaceDirection.CounterClockwise;

            return mesh;
        }
    }
}