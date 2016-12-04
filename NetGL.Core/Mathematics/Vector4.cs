using System;
using System.Runtime.InteropServices;

namespace NetGL.Core.Mathematics {
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4 : IEquatable<Vector4> {
        public static readonly Vector4 Zero = new Vector4(0, 0, 0, 0);
        public static readonly Vector4 One = new Vector4(1, 1, 1, 1);

        public float X;
        public float Y;
        public float Z;
        public float W;

        public float Length {
            get { return MathF.Sqrt(X * X + Y * Y + Z * Z + W * W); }
        }

        public Vector4 Normalized {
            get {
                var l = Length;
                return new Vector4(X / l, Y / l, Z / l, W / l);
            }
        }

        public Vector4(float xyzw) {
            this.X = this.Y = this.Z = this.W = xyzw;
        }
        public Vector4(float x, float y, float z, float w) {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
        public Vector4(double x, double y, double z, double w) {
            this.X = (float)x;
            this.Y = (float)y;
            this.Z = (float)z;
            this.W = (float)w;
        }
        public Vector4(string text) {
            var s = text.Split(new[] { ',', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length != 3) throw new ArgumentException("cannot parse " + text);

            this.X = float.Parse(s[0]);
            this.Y = float.Parse(s[1]);
            this.Z = float.Parse(s[2]);
            this.W = float.Parse(s[3]);
        }
        public Vector4(byte[] bytes, int offset) {
            if (bytes.Length - offset < 16)
                throw new ArgumentException("bytes length should be 12 bytes");

            X = BitConverter.ToSingle(bytes, 0 + offset);
            Y = BitConverter.ToSingle(bytes, 4 + offset);
            Z = BitConverter.ToSingle(bytes, 8 + offset);
            W = BitConverter.ToSingle(bytes, 12 + offset);
        }
        public Vector4(byte[] bytes) {
            if (bytes.Length != 16)
                throw new ArgumentException("bytes length should be 12 bytes");

            X = BitConverter.ToSingle(bytes, 0);
            Y = BitConverter.ToSingle(bytes, 4);
            Z = BitConverter.ToSingle(bytes, 8);
            W = BitConverter.ToSingle(bytes, 12);
        }
        public Vector4(Vector4 v) {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = v.W;
        }
        public Vector4(Vector3 v) {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = 0;
        }
        public Vector4(Vector2 v) {
            X = v.X;
            Y = v.Y;
            Z = 0;
            W = 0;
        }
        public Vector4(Vector3 v, float w) {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }
        public Vector4(Vector2 v, float z, float w) {
            X = v.X;
            Y = v.Y;
            Z = z;
            W = w;
        }  
        public Vector4(float[] values) {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 4)
                throw new ArgumentException("values.Length");

            X = values[0];
            Y = values[1];
            Z = values[2];
            W = values[3];
        }

        public float this[int index] {
            get {
                switch (index) {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                switch (index) {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override string ToString() {
            return string.Format("[{0}, {1}, {2}, {3}]", X, Y, Z, W);
        }
        public byte[] ToBytes() {
            var bytes = new byte[16];
            BitConverter.GetBytes(X).CopyTo(bytes, 0);
            BitConverter.GetBytes(Y).CopyTo(bytes, 4);
            BitConverter.GetBytes(Z).CopyTo(bytes, 8);
            BitConverter.GetBytes(W).CopyTo(bytes, 12);
            return bytes;
        }

        public override bool Equals(object obj) {
            if (!(obj is Vector4)) return false;

            var v = (Vector4)obj;

            return this.Equals(v);
        }
        public bool Equals(Vector4 other) {
            return this.X == other.X
                   && this.Y == other.Y
                   && this.Z == other.Z
                   && this.W == other.W;
        }
        public override int GetHashCode() {
            throw new InvalidOperationException();
        }

        public static bool operator ==(Vector4 lhs, Vector4 rhs) {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(Vector4 lhs, Vector4 rhs) {
            return !lhs.Equals(rhs);
        }
        public static Vector4 operator -(Vector4 a) {
            return new Vector4(-a.X, -a.Y, -a.Z, -a.W);
        }
        public static Vector4 operator -(Vector4 a, Vector4 b) {
            return new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }
        public static Vector4 operator *(float d, Vector4 a) {
            return new Vector4(a.X * d, a.Y * d, a.Z * d, a.W * d);
        }
        public static Vector4 operator *(Vector4 a, float d) {
            return new Vector4(a.X * d, a.Y * d, a.Z * d, a.W * d);
        }
        public static Vector4 operator *(double d, Vector4 a) {
            return new Vector4(a.X * d, a.Y * d, a.Z * d, a.W * d);
        }
        public static Vector4 operator *(Vector4 a, double d) {
            return new Vector4(a.X * d, a.Y * d, a.Z * d, a.W * d);
        }
        public static Vector4 operator /(Vector4 a, float d) {
            return new Vector4(a.X / d, a.Y / d, a.Z / d, a.W / d);
        }
        public static Vector4 operator +(Vector4 a, Vector4 b) {
            return new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static explicit operator Vector4(Vector3 v) {
            return new Vector4(v);
        }

        public static Vector4 Transform(Vector4 v, Matrix matrix) {
            var result = Zero;
            result.X =
                (float)
                    (v.X * (double)matrix.M11 + v.Y * (double)matrix.M21 +
                     v.Z * (double)matrix.M31 + (double)v.W * matrix.M41);
            result.Y =
                (float)
                    (v.X * (double)matrix.M12 + v.Y * (double)matrix.M22 +
                     v.Z * (double)matrix.M32 + (double)v.W * matrix.M42);
            result.Z =
                (float)
                    (v.X * (double)matrix.M13 + v.Y * (double)matrix.M23 +
                     v.Z * (double)matrix.M33 + (double)v.W * matrix.M43);
            result.W =
                (float)
                    (v.X * (double)matrix.M14 + v.Y * (double)matrix.M24 +
                     v.Z * (double)matrix.M34 + (double)v.W * matrix.M44);
            return result;
        }
    }
}