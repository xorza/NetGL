using System.Runtime.CompilerServices;
namespace NetGL.Core.Mathematics {
    public class Plane {
        public float A, B, C, D;

        public Vector3 Normal {
            get {
                return new Vector3(A, B, C);
            }
            set {
                A = value.X;
                B = value.Y;
                C = value.Z;
            }
        }

        public Plane() { }
        public Plane(Vector3 normal, float d) {
            A = normal.X;
            B = normal.Y;
            C = normal.Z;
            D = d;
        }
        
        
        public float Distance(Vector3 v) {
            return (A * v.X + B * v.Y + C * v.Z + D) / (MathF.Sqrt(A * A + B * B + C * C));
        }
        public void Normalize() {
            var n = MathF.Sqrt(A * A + B * B + C * C);

            A /= n;
            B /= n;
            C /= n;
            D /= n;
        }
        public RaycastHit Raycast(Ray ray) {
            var planeNormal = Normal;

            var t = -(Vector3.Dot(ray.Origin, planeNormal) + D) / (Vector3.Dot(ray.Direction, planeNormal));
            if (t < 0)
                return null;
            if (float.IsNaN(t))
                return null;

            var hit = new RaycastHit();

            hit.Distance = t;
            hit.Point = ray.Origin + t * ray.Direction;

            return hit;
        }

        //https://www.cs.princeton.edu/courses/archive/fall00/cs426/lectures/raycast/sld017.htm
    }
}