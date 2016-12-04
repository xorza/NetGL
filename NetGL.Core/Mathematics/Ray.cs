namespace NetGL.Core.Mathematics {
    public class Ray {
        public Vector3 Origin;
        public Vector3 Direction;

        public Ray() {
        }

        public Ray(Vector3 origin, Vector3 direction) {
            Origin = origin;
            Direction = direction;
        }

        public float Distance(Vector3 point) {
            //http://answers.unity3d.com/questions/62644/distance-between-a-ray-and-a-point.html

            return Vector3.Cross(Direction, point - Origin).Length;
        }

        public static Ray Transform(Ray ray, Matrix matrix) {
            var result = new Ray();

            result.Origin = Vector3
                .Transform(ray.Origin, matrix);
            result.Direction = Vector3
                .NormalTransform(ray.Direction, matrix)
                .Normalized;

            return result;
        }
    }
}