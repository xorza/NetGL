using NetGL.Core.Helpers;

namespace NetGL.Core.Mathematics
{
    public class Triangle
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;

        public bool Raycast(Ray ray, ref Vector3 intersection)
        {
            var e1 = B - A;
            var e2 = C - A;
            var pvec = Vector3.Cross(ray.Direction, e2);
            var det = Vector3.Dot(e1, pvec);

            bool IsSingleSided = true;
            if (IsSingleSided)
            {
                if (det < float.Epsilon)
                    return false;
            }
            else
            {
                if (det > -float.Epsilon && det < float.Epsilon)
                    return false;
            }

            var invDet = 1 / det;
            var tvec = ray.Origin - A;

            var u = Vector3.Dot(tvec, pvec) * invDet;
            if (u < 0 || u > 1)
                return false;

            var qvec = Vector3.Cross(tvec, e1);
            var v = Vector3.Dot(ray.Direction, qvec) * invDet;
            if (v < 0 || v > 1 || u + v > 1)
                return false;

            var t = Vector3.Dot(e2, qvec) * invDet;

            intersection.X = u;
            intersection.Y = v;
            intersection.Z = t;
            return true;
        }
        public Reference<Vector3> Raycast(Ray ray)
        {
            var e1 = B - A;
            var e2 = C - A;
            var pvec = Vector3.Cross(ray.Direction, e2);
            var det = Vector3.Dot(e1, pvec);

            bool IsSingleSided = true;
            if (IsSingleSided)
            {
                if (det < float.Epsilon)
                    return null;
            }
            else
            {
                if (det > -float.Epsilon && det < float.Epsilon)
                    return null;
            }

            var invDet = 1 / det;
            var tvec = ray.Origin - A;

            var u = Vector3.Dot(tvec, pvec) * invDet;
            if (u < 0 || u > 1)
                return null;

            var qvec = Vector3.Cross(tvec, e1);
            var v = Vector3.Dot(ray.Direction, qvec) * invDet;
            if (v < 0 || v > 1 || u + v > 1)
                return null;

            var t = Vector3.Dot(e2, qvec) * invDet;

            var result = new Reference<Vector3>();
            result.Value.X = u;
            result.Value.Y = v;
            result.Value.Z = t;
            return result;
        }
    }
}