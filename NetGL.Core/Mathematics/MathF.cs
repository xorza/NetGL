using System;
using System.Runtime.CompilerServices;

namespace NetGL.Core.Mathematics {
    public static class MathF {
        public const float PI = (float)Math.PI;
        public const float PIOver2 = (float)(Math.PI / 2f);
        public const float Deg2Rad = (float)(Math.PI / 180f);
        public const float Rad2Deg = (float)(180f / Math.PI);

        public static float Sqrt(float value) {
            return (float)Math.Sqrt(value);
        }

        public static float Clamp(float value, float min, float max) {
            return Math.Max(Math.Min(value, max), min);
        }

        public static float Clamp01(float value) {
            return Math.Max(Math.Min(value, 1), 0);
        }

        public static float Frac(float v) {
            if (v < 0)
                throw new NotImplementedException();

            return (float)(v - Math.Floor(v));
        }

        public static float Floor(float v) {
            return (float)Math.Floor(v);
        }

        public static float Sin(float v) {
            return (float)Math.Sin(v);
        }

        public static float Cos(float v) {
            return (float)Math.Cos(v);
        }

        public static float Atan2(float a, float b) {
            return (float)Math.Atan2(a, b);
        }

        public static float Lerp(float from, float to, float amount) {
            return (1.0f - amount) * from + amount * to;
        }

        public static float Acos(float value) {
            return (float)Math.Acos(value);
        }

        public static float Pow(float x, float y) {
            return (float)Math.Pow(x, y);
        }


        public static Vector3 Noise3(float time) {
            var result = new Vector3();

            result.X = SimplexNoise.Generate(time, 15.73f);
            result.Y = SimplexNoise.Generate(time, 63.94f);
            result.Z = SimplexNoise.Generate(time, 0.2f);

            return result;
        }


        public static bool Quadric(float a, float b, float c, ref float t1, ref float t2) {
            var d = b * b - 4 * a * c;
            if (d < 0)
                return false;

            if (d == 0) {
                t1 = t2 = -b / (2 * a);
                return true;
            }

            var sd = Sqrt(d);
            t1 = (-b - sd) / (2 * a);
            t2 = (-b + sd) / (2 * a);
            return true;
        }


        public static bool InRangeInclusive(this float value, float lower, float upper) {
            if (lower > upper)
                throw new ArgumentOutOfRangeException("lower should be lesser tha upper");

            return value >= lower && value <= upper;
        }

        public static bool InRangeExclusive(this float value, float lower, float upper) {
            if (lower > upper)
                throw new ArgumentOutOfRangeException("lower should be lesser tha upper");

            return value > lower && value < upper;
        }

        public static bool InRangeInclusive(this int value, int lower, int upper) {
            if (lower > upper)
                throw new ArgumentOutOfRangeException("lower should be lesser tha upper");

            return value >= lower && value <= upper;
        }

        public static bool InRangeExclusive(this int value, int lower, int upper) {
            if (lower > upper)
                throw new ArgumentOutOfRangeException("lower should be lesser tha upper");

            return value > lower && value < upper;
        }


        public static float Max(float p1, float p2, float p3) {
            return Math.Max(p1, Math.Max(p2, p3));
        }

        public static float Max(float p1, float p2, float p3, float p4) {
            return Math.Max(p1, Math.Max(p2, Math.Max(p3, p4)));
        }
    }
}