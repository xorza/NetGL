using System;

namespace NetGL.Core.Mathematics {
    public static class RandomF {
        private static readonly Random _r = new Random(Environment.TickCount);

        public static float Float() {
            return (float)_r.NextDouble();
        }
        public static int Sign() {
            return _r.Next(2) == 1 ? 1 : -1;
        }
        public static float Float(float min, float max) {
            return MathF.Lerp(min, max, Float());
        }

        public static Vector3 Vector3(Vector3 min, Vector3 max) {
            return new Vector3(Float(min.X, max.X), Float(min.Y, max.Y), Float(min.Z, max.Z));
        }
        public static Vector3 InsideUnitCube() {
            return new Vector3(Float() * Sign(), Float() * Sign(), Float() * Sign()) / 2;
        }
        public static Vector3 InsideUnitSphere() {
            Vector3 result;
            do {
                result = InsideUnitCube();
            } while (result.LengthSquared > 0.5 * 0.5);
            return result;
        }

        public static Vector2 Vector2(Vector2 min, Vector2 max) {
            return new Vector2(Float(min.X, max.X), Float(min.Y, max.Y));
        }
    }
}