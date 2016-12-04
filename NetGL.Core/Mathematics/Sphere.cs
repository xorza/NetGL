namespace NetGL.Core.Mathematics {
    public class Sphere {
        public Vector3 Center;
        public float Radius;

        public Sphere() { }
        public Sphere(Vector3 center, float radius) {
            Assert.True(radius >= 0);

            this.Center = center;
            this.Radius = radius;
        }

        public bool FastRaycast(Ray ray) {
            var q = Center - ray.Origin;
            var c = q.LengthSquared;
            var v = Vector3.Dot(q, ray.Direction);
            var d = Radius * Radius - (c - v * v);

            return d >= 0f;
        }
        public RaycastHit Raycast(Ray ray) {
            var q = ray.Origin - Center;

            var a = ray.Direction.LengthSquared;
            var b = 2 * Vector3.Dot(ray.Direction, q);
            var c = q.LengthSquared - Radius * Radius;

            float t1 = 0f, t2 = 0f;
            if (!MathF.Quadric(a, b, c, ref t1, ref t2))
                return null;
            
            var hit = new RaycastHit();

            hit.Point = ray.Origin + ray.Direction * t1;
            hit.Distance = MathF.Sqrt(a) * t1;

            return hit;
        }
    }
}