using System;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;

namespace NetGL.Core.Meshes
{
    public static class Torus
    {
        public static Mesh Create(float majorRadius, float minorRadius, int torusPrecision = 48)
        {
            var numVertices = (torusPrecision + 1)*(torusPrecision + 1);
            var numIndices = 2*torusPrecision*torusPrecision*3;

            var vertices = new Vector3Buffer(numVertices);
            var normals = new Vector3Buffer(numVertices);

            for (int i = 0; i < torusPrecision + 1; i++)
            {
                var sTangent = new Vector3(0.0f, 0.0f, -1.0f);
                var tTangent = (new Vector3(0.0f, -1.0f, 0.0f)).GetRotatedZ(i*360.0f/torusPrecision);

                var position = (new Vector3(minorRadius, 0.0f, 0.0f)).GetRotatedZ(i*360.0f/torusPrecision) +
                               new Vector3(majorRadius, 0.0f, 0.0f);
                var normal = Vector3.Cross(tTangent, sTangent);

                vertices[i] = position;
                normals[i] = normal;
                //vertices[i].s = 0.0f;
                //vertices[i].t = (float)i / torusPrecision;
            }

            for (int ring = 1; ring < torusPrecision + 1; ring++)
            {
                for (int i = 0; i < torusPrecision + 1; i++)
                {
                    vertices[ring*(torusPrecision + 1) + i] = vertices[i].GetRotatedY(ring*360.0f/torusPrecision);
                    normals[ring*(torusPrecision + 1) + i] = normals[i].GetRotatedY(ring*360.0f/torusPrecision);

                    //vertices[ring * (torusPrecision + 1) + i].s = 2.0f * ring / torusPrecision;
                    //vertices[ring * (torusPrecision + 1) + i].t = vertices[i].t;

                    //vertices[ring * (torusPrecision + 1) + i].sTangent =
                    //    vertices[i].sTangent.GetRotatedY(ring * 360.0f / torusPrecision);
                    //vertices[ring * (torusPrecision + 1) + i].tTangent =
                    //    vertices[i].tTangent.GetRotatedY(ring * 360.0f / torusPrecision);
                }
            }

            var indices = new UInt32Buffer(numIndices);
            //  Calculate the indices
            for (int ring = 0; ring < torusPrecision; ring++)
            {
                for (int i = 0; i < torusPrecision; i++)
                {
                    indices[((ring * torusPrecision + i) * 2) * 3 + 0] = (UInt32)(ring * (torusPrecision + 1) + i);
                    indices[((ring * torusPrecision + i) * 2) * 3 + 1] = (UInt32)((ring + 1) * (torusPrecision + 1) + i);
                    indices[((ring * torusPrecision + i) * 2) * 3 + 2] = (UInt32)(ring * (torusPrecision + 1) + i + 1);
                    indices[((ring * torusPrecision + i) * 2 + 1) * 3 + 0] = (UInt32)(ring * (torusPrecision + 1) + i + 1);
                    indices[((ring * torusPrecision + i) * 2 + 1) * 3 + 1] = (UInt32)((ring + 1) * (torusPrecision + 1) + i);
                    indices[((ring*torusPrecision + i)*2 + 1)*3 + 2] =
                        (UInt16) ((ring + 1)*(torusPrecision + 1) + i + 1);
                }
            }


            var result = new Mesh();
            result.Indices = indices;
            result.Vertices = vertices;
            result.Normals = normals;
            result.CalculateBounds();

            return result;
        }

        private static Vector3 GetRotatedX(this Vector3 me, float angle)
        {
            if (angle == 0.0f)
                return me;

            float sinAngle = (float) Math.Sin(Math.PI*angle/180);
            float cosAngle = (float) Math.Cos(Math.PI*angle/180);

            return new Vector3(me.X,
                me.Y*cosAngle - me.Z*sinAngle,
                me.Y*sinAngle + me.Z*cosAngle);
        }

        private static Vector3 GetRotatedY(this Vector3 me, float angle)
        {
            if (angle == 0.0f)
                return me;

            float sinAngle = (float) Math.Sin(Math.PI*angle/180);
            float cosAngle = (float) Math.Cos(Math.PI*angle/180);

            return new Vector3(me.X*cosAngle + me.Z*sinAngle,
                me.Y,
                -me.X*sinAngle + me.Z*cosAngle);
        }

        private static Vector3 GetRotatedZ(this Vector3 me, float angle)
        {
            if (angle == 0.0f)
                return me;

            float sinAngle = (float) Math.Sin(Math.PI*angle/180);
            float cosAngle = (float) Math.Cos(Math.PI*angle/180);

            return new Vector3(me.X*cosAngle - me.Y*sinAngle,
                me.X*sinAngle + me.Y*cosAngle,
                me.Z);
        }
    }
}