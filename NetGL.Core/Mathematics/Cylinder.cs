namespace NetGL.Core.Mathematics {
    public class Cylinder {
        public Vector3 Base;
        public float Radius, Height;

        public RaycastHit Raycast(Ray ray) {
            #region line circle intersection
            var p = new Vector2(ray.Origin.X, ray.Origin.Z) - new Vector2(Base.X, Base.Z);
            var d = new Vector2(ray.Direction.X, ray.Direction.Z);

            var circleCenter = new Vector2(Base.X, Base.Z);

            var a = d.LengthSquared;
            var b = 2 * (d.X * p.X + d.Y * p.Y);
            var c = p.LengthSquared - Radius * Radius;

            var delta = b * b - 4 * a * c;

            if (delta < 0)
                return null;

            var s = MathF.Sqrt(delta);
            var t = (-b - s) / (2 * a);
            var intersection = p + d * t;
            #endregion

            #region line tube intersection
            t = (intersection.X - ray.Origin.X) / ray.Direction.X;
            var point = ray.Origin + ray.Direction * t;

            if (point.Y >= Base.Y && point.Y <= Height + Base.Y) {
                var result = new RaycastHit();
                result.Point = point;
                result.Distance = t;
                return result;
            }
            #endregion

            #region line caps intersection
            
            #endregion

            return null;
        }


        //        a = (P2MinusP1.X) * (P2MinusP1.X) + (P2MinusP1.Y) * (P2MinusP1.Y)
        //b = 2 * ((P2MinusP1.X * LocalP1.X) + (P2MinusP1.Y * LocalP1.Y))
        //c = (LocalP1.X * LocalP1.X) + (LocalP1.Y * LocalP1.Y) – (Radius * Radius)
        //delta = b * b – (4 * a * c)
        //if (delta < 0) // No intersection
        //    return null;
        //else if (delta == 0) // One intersection
        //    u = -b / (2 * a)
        //    return LineP1 + (u * P2MinusP1)
        //    /* Use LineP1 instead of LocalP1 because we want our answer in global
        //       space, not the circle's local space */
        //else if (delta > 0) // Two intersections
        //    SquareRootDelta = sqrt(delta)

        //    u1 = (-b + SquareRootDelta) / (2 * a)
        //    u2 = (-b - SquareRootDelta) / (2 * a)

        //    return { LineP1 + (u1 * P2MinusP1) ; LineP1 + (u2 * P2MinusP1)}
    }
}
