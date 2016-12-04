using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NetGL.Core.Mathematics {
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2 : IEquatable<Vector2> {
        public static readonly Vector2 Zero = new Vector2(0, 0);
        public static readonly Vector2 One = new Vector2(1, 1);

        public float X;
        public float Y;

        public float Length {
            get {
                return
                    (float)
                        Math.Sqrt(this.X * this.X + this.Y * this.Y);
            }
        }
        public float LengthSquared {
            get {
                return this.X * this.X + this.Y * this.Y;
            }
        }
        public Vector2 Normalized {
            get {
                var result = this;
                result.Normalize();
                return result;
            }
        }

        public Vector2(float x, float y) {
            this.X = x;
            this.Y = y;
        }
        public Vector2(double x, double y) {
            this.X = (float)x;
            this.Y = (float)y;
        }
        public Vector2(Vector4 v) {
            this.X = v.X;
            this.Y = v.Y;
        }
        public Vector2(Vector3 v) {
            this.X = v.X;
            this.Y = v.Y;
        }
        public Vector2(float[] values) {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 2)
                throw new ArgumentException("values.Length");

            X = values[0];
            Y = values[1];
        }
        public Vector2(float v) {
            this.X = v;
            this.Y = v;
        }
        public Vector2(Point p) {
            this.X = p.X;
            this.Y = p.Y;
        }
        public Vector2(Size size) {
            this.X = size.Width;
            this.Y = size.Height;
        }

        public Point ToPoint() {
            return new Point((int)X, (int)Y);
        }
        public void Normalize() {
            var num = 1 / Math.Sqrt(X * X + Y * Y);
            X = (float)(X * num);
            Y = (float)(Y * num);
        }

        public static float Distance(Vector2 value1, Vector2 value2) {
            float num1 = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            return (float)Math.Sqrt(num1 * num1 + num2 * num2);
        }
        public static float DistanceSquared(Vector2 value1, Vector2 value2) {
            var num1 = value1.X - value2.X;
            var num2 = value1.Y - value2.Y;
            return num1 * num1 + num2 * num2;
        }
        public static float Dot(Vector2 vector1, Vector2 vector2) {
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }

        public static Vector2 operator +(Vector2 value1, Vector2 value2) {
            Vector2 vector;
            vector.X = value1.X + value2.X;
            vector.Y = value1.Y + value2.Y;
            return vector;
        }
        public static Vector2 operator *(Vector2 value, float scaleFactor) {
            Vector2 vector2;
            vector2.X = value.X * scaleFactor;
            vector2.Y = value.Y * scaleFactor;
            return vector2;
        }
        public static Vector2 operator *(float scaleFactor, Vector2 value) {
            Vector2 vector2;
            vector2.X = value.X * scaleFactor;
            vector2.Y = value.Y * scaleFactor;
            return vector2;
        }
        public static Vector2 operator *(Vector2 value1, Vector2 value2) {
            Vector2 vector;
            vector.X = value1.X * value2.X;
            vector.Y = value1.Y * value2.Y;
            return vector;
        }
        public static Vector2 operator /(Vector2 v1, Vector2 v2) {
            return new Vector2(v1.X / v2.X, v1.Y / v2.Y);
        }
        public static Vector2 operator /(Vector2 value, float divider) {
            float num = 1f / divider;
            Vector2 vector2;
            vector2.X = value.X * num;
            vector2.Y = value.Y * num;
            return vector2;
        }
        public static Vector2 operator -(Vector2 value1, Vector2 value2) {
            Vector2 vector;
            vector.X = value1.X - value2.X;
            vector.Y = value1.Y - value2.Y;
            return vector;
        }
        public static bool operator ==(Vector2 v1, Vector2 v2) {
            return v1.X == v2.X && v1.Y == v2.Y;
        }
        public static bool operator !=(Vector2 v1, Vector2 v2) {
            return v1.X != v2.X || v1.Y != v2.Y;
        }

        public static explicit operator Vector2(Vector3 v) {
            return new Vector2(v);
        }

        public override string ToString() {
            return string.Format("[{0} {1}]", X, Y);
        }
        public override bool Equals(object obj) {
            if (obj is Vector2)
                return Equals((Vector2)obj);
            else
                return false;
        }
        public bool Equals(Vector2 other) {
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode() {
            //return this.X.GetHashCode() ^ this.Y.GetHashCode();
            throw new InvalidOperationException();
        }
    }
}