using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetGL.Core.Mathematics {
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3 : IEquatable<Vector3> {
        private static readonly Vector3 _zero = new Vector3();
        private static readonly Vector3 _one = new Vector3(1f, 1f, 1f);
        private static readonly Vector3 _unitX = new Vector3(1f, 0.0f, 0.0f);
        private static readonly Vector3 _unitY = new Vector3(0.0f, 1f, 0.0f);
        private static readonly Vector3 _unitZ = new Vector3(0.0f, 0.0f, 1f);
        private static readonly Vector3 _up = new Vector3(0.0f, 1f, 0.0f);
        private static readonly Vector3 _down = new Vector3(0.0f, -1f, 0.0f);
        private static readonly Vector3 _right = new Vector3(1f, 0.0f, 0.0f);
        private static readonly Vector3 _left = new Vector3(-1f, 0.0f, 0.0f);
        private static readonly Vector3 _forward = new Vector3(0.0f, 0.0f, -1f);
        private static readonly Vector3 _backward = new Vector3(0.0f, 0.0f, 1f);

        public float X;
        public float Y;
        public float Z;

        public static Vector3 Zero {
            get { return _zero; }
        }
        public static Vector3 One {
            get { return _one; }
        }
        public static Vector3 UnitX {
            get { return _unitX; }
        }
        public static Vector3 UnitY {
            get { return _unitY; }
        }
        public static Vector3 UnitZ {
            get { return _unitZ; }
        }
        public static Vector3 Up {
            get { return _up; }
        }
        public static Vector3 Down {
            get { return _down; }
        }
        public static Vector3 Right {
            get { return _right; }
        }
        public static Vector3 Left {
            get { return _left; }
        }
        public static Vector3 Forward {
            get { return _forward; }
        }
        public static Vector3 Backward {
            get { return _backward; }
        }

        public Vector3(double x, double y, double z) {
            this.X = (float)x;
            this.Y = (float)y;
            this.Z = (float)z;
        }
        public Vector3(float x, float y, float z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public Vector3(float value) {
            this.X = this.Y = this.Z = value;
        }
        public Vector3(Vector2 value) {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = 0;
        }
        public Vector3(Vector2 value, float z) {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }
        public Vector3(Vector4 value) {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }
        public Vector3(float[] values) {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 3)
                throw new ArgumentException("values.Length");

            X = values[0];
            Y = values[1];
            Z = values[2];
        }

        public Vector3 Normalized {
            
            get {
                var result = this;
                result.Normalize();
                return result;
            }
        }
        public float Length {
            
            get {
                return MathF.Sqrt(X * X + Y * Y + Z * Z);
            }
        }
        public float LengthSquared {
            
            get {
                return X * X + Y * Y + Z * Z;
            }
        }

        
        public static Vector3 operator -(Vector3 value) {
            Vector3 vector3;
            vector3.X = -value.X;
            vector3.Y = -value.Y;
            vector3.Z = -value.Z;
            return vector3;
        }
        
        public static bool operator ==(Vector3 value1, Vector3 value2) {
            return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z;
        }
        
        public static bool operator !=(Vector3 value1, Vector3 value2) {
            return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z;
        }
        
        public static Vector3 operator +(Vector3 value1, Vector3 value2) {
            Vector3 vector3;
            vector3.X = value1.X + value2.X;
            vector3.Y = value1.Y + value2.Y;
            vector3.Z = value1.Z + value2.Z;
            return vector3;
        }
        
        public static Vector3 operator +(Vector3 value1, Vector2 value2) {
            Vector3 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z;
            return result;
        }
        
        public static Vector3 operator -(Vector3 value1, Vector3 value2) {
            Vector3 vector3;
            vector3.X = value1.X - value2.X;
            vector3.Y = value1.Y - value2.Y;
            vector3.Z = value1.Z - value2.Z;
            return vector3;
        }
        
        public static Vector3 operator -(Vector3 value1, Vector2 value2) {
            Vector3 vector;
            vector.X = value1.X - value2.X;
            vector.Y = value1.Y - value2.Y;
            vector.Z = value1.Z;
            return vector;
        }
        
        public static Vector3 operator *(Vector3 value1, Vector3 value2) {
            Vector3 vector3;
            vector3.X = value1.X * value2.X;
            vector3.Y = value1.Y * value2.Y;
            vector3.Z = value1.Z * value2.Z;
            return vector3;
        }
        
        public static Vector3 operator *(Vector3 value, float scaleFactor) {
            Vector3 vector3;
            vector3.X = value.X * scaleFactor;
            vector3.Y = value.Y * scaleFactor;
            vector3.Z = value.Z * scaleFactor;
            return vector3;
        }
        
        public static Vector3 operator *(float scaleFactor, Vector3 value) {
            Vector3 vector3;
            vector3.X = value.X * scaleFactor;
            vector3.Y = value.Y * scaleFactor;
            vector3.Z = value.Z * scaleFactor;
            return vector3;
        }
        
        public static Vector3 operator /(Vector3 value1, Vector3 value2) {
            Vector3 vector3;
            vector3.X = value1.X / value2.X;
            vector3.Y = value1.Y / value2.Y;
            vector3.Z = value1.Z / value2.Z;
            return vector3;
        }
        
        public static Vector3 operator /(Vector3 value, float divider) {
            float num = 1f / divider;
            Vector3 vector3;
            vector3.X = value.X * num;
            vector3.Y = value.Y * num;
            vector3.Z = value.Z * num;
            return vector3;
        }

        
        public static explicit operator Vector3(Vector4 value) {
            return new Vector3(value);
        }
        
        public static explicit operator Vector3(Vector2 value) {
            return new Vector3(value);
        }

        public override string ToString() {
            return string.Format("[{0} {1} {2}]", X, Y, Z);
        }

        public bool Equals(Vector3 other) {
            return this == other;
        }
        public override bool Equals(object obj) {
            bool flag = false;
            if (obj is Vector3)
                flag = this.Equals((Vector3)obj);

            return flag;
        }
        public override int GetHashCode() {
            throw new InvalidOperationException();
            //return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }

        public void Normalize() {
            var num = 1 / MathF.Sqrt(X * X + Y * Y + Z * Z);
            X = X * num;
            Y = Y * num;
            Z = Z * num;
        }

        
        public static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 up) {
            if (v1 == Zero)
                return float.NaN;
            if (v2 == Zero)
                return float.NaN;
            if (up == Zero)
                return float.NaN;

            v1.Normalize();
            v2.Normalize();
            up.Normalize();

            var a = Angle(v1, v2);
            var normal = Cross(v1, v2);
            a *= Math.Sign(Dot(normal, up));
            return a;
        }
        
        public static float Angle(Vector3 v1, Vector3 v2) {
            if (v1 == Zero)
                return float.NaN;
            if (v2 == Zero)
                return float.NaN;

            var dot = Dot(v1.Normalized, v2.Normalized);
            return MathF.Acos(dot);
        }
        
        public static float Distance(Vector3 value1, Vector3 value2) {
            var num1 = value1.X - value2.X;
            var num2 = value1.Y - value2.Y;
            var num3 = value1.Z - value2.Z;
            return MathF.Sqrt(num1 * num1 + num2 * num2 + num3 * num3);
        }
        
        public static float DistanceSquared(Vector3 value1, Vector3 value2) {
            var num1 = value1.X - value2.X;
            var num2 = value1.Y - value2.Y;
            var num3 = value1.Z - value2.Z;
            return num1 * num1 + num2 * num2 + num3 * num3;
        }
        
        public static float Dot(Vector3 vector1, Vector3 vector2) {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }
        
        public static Vector3 Normalize(Vector3 value) {
            var num = 1 / MathF.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z);
            Vector3 vector3;
            vector3.X = value.X * num;
            vector3.Y = value.Y * num;
            vector3.Z = value.Z * num;
            return vector3;
        }
        
        public static Vector3 Cross(Vector3 vector1, Vector3 vector2) {
            Vector3 vector3;
            vector3.X = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            vector3.Y = vector1.Z * vector2.X - vector1.X * vector2.Z;
            vector3.Z = vector1.X * vector2.Y - vector1.Y * vector2.X;
            return vector3;
        }
        
        public static Vector3 Reflect(Vector3 vector, Vector3 normal) {
            float num = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;
            Vector3 vector3;
            vector3.X = vector.X - 2f * num * normal.X;
            vector3.Y = vector.Y - 2f * num * normal.Y;
            vector3.Z = vector.Z - 2f * num * normal.Z;
            return vector3;
        }
        
        public static Vector3 Min(Vector3 value1, Vector3 value2) {
            Vector3 vector3;
            vector3.X = value1.X < value2.X ? value1.X : value2.X;
            vector3.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
            vector3.Z = value1.Z < value2.Z ? value1.Z : value2.Z;
            return vector3;
        }
        
        public static Vector3 Max(Vector3 value1, Vector3 value2) {
            Vector3 vector3;
            vector3.X = value1.X > value2.X ? value1.X : value2.X;
            vector3.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
            vector3.Z = value1.Z > value2.Z ? value1.Z : value2.Z;
            return vector3;
        }
        
        public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max) {
            var num1 = value1.X;
            var num2 = num1 > max.X ? max.X : num1;
            var num3 = num2 < min.X ? min.X : num2;
            var num4 = value1.Y;
            var num5 = num4 > max.Y ? max.Y : num4;
            var num6 = num5 < min.Y ? min.Y : num5;
            var num7 = value1.Z;
            var num8 = num7 > max.Z ? max.Z : num7;
            var num9 = num8 < min.Z ? min.Z : num8;
            Vector3 vector3;
            vector3.X = num3;
            vector3.Y = num6;
            vector3.Z = num9;
            return vector3;
        }
        
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount) {
            Vector3 vector3;
            vector3.X = value1.X + (value2.X - value1.X) * amount;
            vector3.Y = value1.Y + (value2.Y - value1.Y) * amount;
            vector3.Z = value1.Z + (value2.Z - value1.Z) * amount;
            return vector3;
        }
        
        public static Vector3 Barycentric(Vector3 value1, Vector3 value2, Vector3 value3, float amount1, float amount2) {
            Vector3 vector3;
            vector3.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
            vector3.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
            vector3.Z = value1.Z + amount1 * (value2.Z - value1.Z) + amount2 * (value3.Z - value1.Z);
            return vector3;
        }
        
        public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount) {
            amount = amount > 1.0f ? 1f : (amount < 0.0f ? 0.0f : amount);
            amount = amount * amount * (3.0f - 2.0f * amount);
            Vector3 vector3;
            vector3.X = value1.X + (value2.X - value1.X) * amount;
            vector3.Y = value1.Y + (value2.Y - value1.Y) * amount;
            vector3.Z = value1.Z + (value2.Z - value1.Z) * amount;
            return vector3;
        }
        
        public static Vector3 CatmullRom(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount) {
            float num1 = amount * amount;
            float num2 = amount * num1;
            Vector3 vector3;
            vector3.X = 0.5f *
                     (2.0f * value2.X + (-value1.X + value3.X) * amount +
                      (2.0f * value1.X - 5.0f * value2.X + 4.0f * value3.X - value4.X) *
                      num1 +
                      (-value1.X + 3.0f * value2.X - 3.0f * value3.X + value4.X) *
                      num2);
            vector3.Y = 0.5f *
                     (2.0f * value2.Y + (-value1.Y + value3.Y) * amount +
                      (2.0f * value1.Y - 5.0f * value2.Y + 4.0f * value3.Y - value4.Y) *
                      num1 +
                      (-value1.Y + 3.0f * value2.Y - 3.0f * value3.Y + value4.Y) *
                      num2);
            vector3.Z = 0.5f *
                     (2.0f * value2.Z + (-value1.Z + value3.Z) * amount +
                      (2.0f * value1.Z - 5.0f * value2.Z + 4.0f * value3.Z - value4.Z) *
                      num1 +
                      (-value1.Z + 3.0f * value2.Z - 3.0f * value3.Z + value4.Z) *
                      num2);
            return vector3;
        }
        
        public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount) {
            float num1 = amount * amount;
            float num2 = amount * num1;
            float num3 = 2.0f * num2 - 3.0f * num1 + 1.0f;
            float num4 = -2.0f * num2 + 3.0f * num1;
            float num5 = num2 - 2f * num1 + amount;
            float num6 = num2 - num1;
            Vector3 vector3;
            vector3.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
            vector3.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
            vector3.Z = value1.Z * num3 + value2.Z * num4 + tangent1.Z * num5 + tangent2.Z * num6;
            return vector3;
        }
        
        public static Vector3 Transform(Vector3 position, Matrix matrix) {
            Vector3 result;
            result.X = position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41;
            result.Y = position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42;
            result.Z = position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43;
            return result;
        }
        
        public static Vector3 NormalTransform(Vector3 normal, Matrix matrix) {
            Vector3 result;
            result.X = normal.X * matrix.M11 + normal.Y * matrix.M21 + normal.Z * matrix.M31;
            result.Y = normal.X * matrix.M12 + normal.Y * matrix.M22 + normal.Z * matrix.M32;
            result.Z = normal.X * matrix.M13 + normal.Y * matrix.M23 + normal.Z * matrix.M33;
            return result;
        }
        
        public static Vector3 Transform(Vector3 value, Quaternion rotation) {
            var num1 = rotation.X + rotation.X;
            var num2 = rotation.Y + rotation.Y;
            var num3 = rotation.Z + rotation.Z;
            var num4 = rotation.W * num1;
            var num5 = rotation.W * num2;
            var num6 = rotation.W * num3;
            var num7 = rotation.X * num1;
            var num8 = rotation.X * num2;
            var num9 = rotation.X * num3;
            var num10 = rotation.Y * num2;
            var num11 = rotation.Y * num3;
            var num12 = rotation.Z * num3;
            var num13 = value.X * (1.0f - num10 - num12) + value.Y * (num8 - num6) + value.Z * (num9 + num5);
            var num14 = value.X * (num8 + num6) + value.Y * (1.0f - num7 - num12) + value.Z * (num11 - num4);
            var num15 = value.X * (num9 - num5) + value.Y * (num11 + num4) + value.Z * (1.0f - num7 - num10);
            Vector3 vector3;
            vector3.X = num13;
            vector3.Y = num14;
            vector3.Z = num15;
            return vector3;
        }
        
        public static Vector3 Negate(Vector3 value) {
            Vector3 vector3;
            vector3.X = -value.X;
            vector3.Y = -value.Y;
            vector3.Z = -value.Z;
            return vector3;
        }
        
        public static Vector3 Add(Vector3 value1, Vector3 value2) {
            Vector3 vector3;
            vector3.X = value1.X + value2.X;
            vector3.Y = value1.Y + value2.Y;
            vector3.Z = value1.Z + value2.Z;
            return vector3;
        }
        
        public static Vector3 Subtract(Vector3 value1, Vector3 value2) {
            Vector3 vector3;
            vector3.X = value1.X - value2.X;
            vector3.Y = value1.Y - value2.Y;
            vector3.Z = value1.Z - value2.Z;
            return vector3;
        }
        
        public static Vector3 Multiply(Vector3 value1, Vector3 value2) {
            Vector3 vector3;
            vector3.X = value1.X * value2.X;
            vector3.Y = value1.Y * value2.Y;
            vector3.Z = value1.Z * value2.Z;
            return vector3;
        }
        
        public static Vector3 Multiply(Vector3 value1, float scaleFactor) {
            Vector3 vector3;
            vector3.X = value1.X * scaleFactor;
            vector3.Y = value1.Y * scaleFactor;
            vector3.Z = value1.Z * scaleFactor;
            return vector3;
        }
        
        public static Vector3 Divide(Vector3 value1, Vector3 value2) {
            Vector3 vector3;
            vector3.X = value1.X / value2.X;
            vector3.Y = value1.Y / value2.Y;
            vector3.Z = value1.Z / value2.Z;
            return vector3;
        }
        
        public static Vector3 Divide(Vector3 value1, float value2) {
            float num = 1f / value2;
            Vector3 vector3;
            vector3.X = value1.X * num;
            vector3.Y = value1.Y * num;
            vector3.Z = value1.Z * num;
            return vector3;
        }
    }
}