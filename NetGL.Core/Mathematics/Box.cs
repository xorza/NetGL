using System;

namespace NetGL.Core.Mathematics {
    public class Box {
        public Vector3 Center;
        public Vector3 Size;

        public Box() { }
        public Box(Vector3 center, Vector3 size) {
            this.Center = center;
            this.Size = size;
        }

        public RaycastHit Raycast(Ray ray) {
            var min = Center - 0.5f * Size;
            var max = Center + 0.5f * Size;

            var dirfrac = Vector3.One / ray.Direction;

            // lb is the corner of AABB with minimal coordinates - left bottom, rt is maximal corner
            // ray.Origin is origin of ray
            var orig = ray.Origin;

            var t1 = (min.X - orig.X) * dirfrac.X;            
            var t2 = (max.X - orig.X) * dirfrac.X;
            var t3 = (min.Y - orig.Y) * dirfrac.Y;
            var t4 = (max.Y - orig.Y) * dirfrac.Y;
            var t5 = (min.Z - orig.Z) * dirfrac.Z;
            var t6 = (max.Z - orig.Z) * dirfrac.Z;

            var tmin = Math.Max(Math.Max(Math.Min(t1, t2), Math.Min(t3, t4)), Math.Min(t5, t6));
            var tmax = Math.Min(Math.Min(Math.Max(t1, t2), Math.Max(t3, t4)), Math.Max(t5, t6));

            if (float.IsNaN(tmin) || float.IsNaN(tmax))
                return null;

            // if tmax < 0, ray (line) is intersecting AABB, but whole AABB is behing us
            if (tmax < 0)
                return null;

            // if tmin > tmax, ray doesn't intersect AABB
            if (tmin > tmax)
                return null;

            var result = new RaycastHit();
            result.Distance = tmin;
            result.Point = tmin * ray.Direction + ray.Origin;
            return result;
        }

        private void MaxIfNaN(ref float f) {
            if (float.IsNaN(f))
                f = float.MaxValue;
        }
    }
}