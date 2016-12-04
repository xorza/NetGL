using System;
using System.Runtime.InteropServices;

namespace NetGL.Core.Mathematics {
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion : IEquatable<Quaternion> {
        private static readonly Quaternion _identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

        public float X;
        public float Y;
        public float Z;
        public float W;

        public static Quaternion Identity {
            get { return _identity; }
        }

        public Quaternion Conjugate {
            get {
                var result = new Quaternion(this);
                result.X = -this.X;
                result.Y = -this.Y;
                result.Z = -this.Z;
                result.W = this.W;

                return result;
            }
        }
        public float LengthSquared {
            get {
                return X * X + Y * Y + Z * Z + W * W;
            }
        }
        public float Length {
            get {
                return MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);
            }
        }

        public Quaternion(float x, float y, float z, float w) {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
        public Quaternion(Vector3 axis, float angle) {
            var halfAngle = angle * 0.5f;

            var sin = MathF.Sin(halfAngle);
            var cos = MathF.Cos(halfAngle);

            X = axis.X * sin;
            Y = axis.Y * sin;
            Z = axis.Z * sin;
            W = cos;
        }
        public Quaternion(Quaternion clone) {
            this.X = clone.X;
            this.Y = clone.Y;
            this.Z = clone.Z;
            this.W = clone.W;
        }

        public static Quaternion operator -(Quaternion quaternion) {
            Quaternion quaternion1;
            quaternion1.X = -quaternion.X;
            quaternion1.Y = -quaternion.Y;
            quaternion1.Z = -quaternion.Z;
            quaternion1.W = -quaternion.W;
            return quaternion1;
        }
        public static bool operator ==(Quaternion quaternion1, Quaternion quaternion2) {
            if (quaternion1.X == (double)quaternion2.X && quaternion1.Y == (double)quaternion2.Y &&
                quaternion1.Z == (double)quaternion2.Z)
                return quaternion1.W == (double)quaternion2.W;
            else
                return false;
        }
        public static bool operator !=(Quaternion quaternion1, Quaternion quaternion2) {
            if (quaternion1.X == (double)quaternion2.X && quaternion1.Y == (double)quaternion2.Y &&
                quaternion1.Z == (double)quaternion2.Z)
                return quaternion1.W != (double)quaternion2.W;
            else
                return true;
        }
        public static Quaternion operator +(Quaternion quaternion1, Quaternion quaternion2) {
            Quaternion quaternion;
            quaternion.X = quaternion1.X + quaternion2.X;
            quaternion.Y = quaternion1.Y + quaternion2.Y;
            quaternion.Z = quaternion1.Z + quaternion2.Z;
            quaternion.W = quaternion1.W + quaternion2.W;
            return quaternion;
        }
        public static Quaternion operator -(Quaternion quaternion1, Quaternion quaternion2) {
            Quaternion quaternion;
            quaternion.X = quaternion1.X - quaternion2.X;
            quaternion.Y = quaternion1.Y - quaternion2.Y;
            quaternion.Z = quaternion1.Z - quaternion2.Z;
            quaternion.W = quaternion1.W - quaternion2.W;
            return quaternion;
        }
        public static Quaternion operator *(Quaternion quaternion1, Quaternion quaternion2) {
            var num1 = quaternion1.X;
            var num2 = quaternion1.Y;
            var num3 = quaternion1.Z;
            var num4 = quaternion1.W;
            var num5 = quaternion2.X;
            var num6 = quaternion2.Y;
            var num7 = quaternion2.Z;
            var num8 = quaternion2.W;
            var num9 = num2 * num7 - num3 * num6;
            var num10 = num3 * num5 - num1 * num7;
            var num11 = num1 * num6 - num2 * num5;
            var num12 = num1 * num5 + num2 * num6 + num3 * num7;

            Quaternion quaternion;
            quaternion.X = num1 * num8 + num5 * num4 + num9;
            quaternion.Y = num2 * num8 + num6 * num4 + num10;
            quaternion.Z = num3 * num8 + num7 * num4 + num11;
            quaternion.W = num4 * num8 - num12;

            return quaternion;
        }
        public static Quaternion operator *(Quaternion quaternion1, float scaleFactor) {
            Quaternion quaternion;
            quaternion.X = quaternion1.X * scaleFactor;
            quaternion.Y = quaternion1.Y * scaleFactor;
            quaternion.Z = quaternion1.Z * scaleFactor;
            quaternion.W = quaternion1.W * scaleFactor;
            return quaternion;
        }
        public static Quaternion operator /(Quaternion quaternion1, Quaternion quaternion2) {
            float num1 = quaternion1.X;
            float num2 = quaternion1.Y;
            float num3 = quaternion1.Z;
            float num4 = quaternion1.W;
            float num5 = 1f /
                         (float)
                             (quaternion2.X * (double)quaternion2.X +
                              quaternion2.Y * (double)quaternion2.Y +
                              quaternion2.Z * (double)quaternion2.Z +
                              quaternion2.W * (double)quaternion2.W);
            float num6 = -quaternion2.X * num5;
            float num7 = -quaternion2.Y * num5;
            float num8 = -quaternion2.Z * num5;
            float num9 = quaternion2.W * num5;
            float num10 = (float)(num2 * (double)num8 - num3 * (double)num7);
            float num11 = (float)(num3 * (double)num6 - num1 * (double)num8);
            float num12 = (float)(num1 * (double)num7 - num2 * (double)num6);
            float num13 =
                (float)(num1 * (double)num6 + num2 * (double)num7 + num3 * (double)num8);
            Quaternion quaternion;
            quaternion.X = (float)(num1 * (double)num9 + num6 * (double)num4) + num10;
            quaternion.Y = (float)(num2 * (double)num9 + num7 * (double)num4) + num11;
            quaternion.Z = (float)(num3 * (double)num9 + num8 * (double)num4) + num12;
            quaternion.W = num4 * num9 - num13;
            return quaternion;
        }

        public override string ToString() {
            return string.Format("[{0} {1} {2} {3}]", X, Y, Z, W);
        }
        public bool Equals(Quaternion other) {
            if (this.X == (double)other.X && this.Y == (double)other.Y &&
                this.Z == (double)other.Z)
                return this.W == (double)other.W;
            else
                return false;
        }
        public override bool Equals(object obj) {
            bool flag = false;
            if (obj is Quaternion)
                flag = this.Equals((Quaternion)obj);
            return flag;
        }
        public override int GetHashCode() {
            throw new InvalidOperationException();
        }

        public static Quaternion CreateFromYawPitchRoll(Vector3 rotation) {
            var rollOver2 = rotation.Z * 0.5;
            var sinRollOver2 = Math.Sin(rollOver2);
            var cosRollOver2 = Math.Cos(rollOver2);
            var pitchOver2 = rotation.X * 0.5;
            var sinPitchOver2 = Math.Sin(pitchOver2);
            var cosPitchOver2 = Math.Cos(pitchOver2);
            var yawOver2 = rotation.Y * 0.5;
            var sinYawOver2 = Math.Sin(yawOver2);
            var cosYawOver2 = Math.Cos(yawOver2);

            Quaternion result;
            result.W = (float)(cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2);
            result.X = (float)(cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2);
            result.Y = (float)(sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2);
            result.Z = (float)(cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2);

            return result;
        }
        public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll) {
            return CreateFromYawPitchRoll(new Vector3(pitch, yaw, roll));
        }
        public static Quaternion FromToRotation(Vector3 from, Vector3 to) {
            from.Normalize();
            to.Normalize();

            var cos = Vector3.Dot(from, to);
            cos = MathF.Clamp(cos, -1, 1);
            var angle = MathF.Acos(cos);
            if (angle == 0)
                return Quaternion.Identity;

            var axis = Vector3.Cross(from, to).Normalized;
            return new Quaternion(axis, angle);
        }
        public static Quaternion LookRotation(Vector3 forward, Vector3 up) {
            forward.Normalize();
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, forward));
            Vector3 vector3 = Vector3.Cross(forward, vector2);
            var m00 = vector2.X;
            var m01 = vector2.Y;
            var m02 = vector2.Z;
            var m10 = vector3.X;
            var m11 = vector3.Y;
            var m12 = vector3.Z;
            var m20 = forward.X;
            var m21 = forward.Y;
            var m22 = forward.Z;
            var num8 = (m00 + m11) + m22;

            var quaternion = new Quaternion();
            if (num8 > 0f) {
                var num = MathF.Sqrt(num8 + 1f);
                quaternion.W = num * 0.5f;
                num = 0.5f / num;
                quaternion.X = (m12 - m21) * num;
                quaternion.Y = (m20 - m02) * num;
                quaternion.Z = (m01 - m10) * num;
                return quaternion;
            }
            if ((m00 >= m11) && (m00 >= m22)) {
                var num7 = MathF.Sqrt(((1f + m00) - m11) - m22);
                var num4 = 0.5f / num7;
                quaternion.X = 0.5f * num7;
                quaternion.Y = (m01 + m10) * num4;
                quaternion.Z = (m02 + m20) * num4;
                quaternion.W = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22) {
                var num6 = MathF.Sqrt(((1f + m11) - m00) - m22);
                var num3 = 0.5f / num6;
                quaternion.X = (m10 + m01) * num3;
                quaternion.Y = 0.5f * num6;
                quaternion.Z = (m21 + m12) * num3;
                quaternion.W = (m20 - m02) * num3;
                return quaternion;
            }
            var num5 = MathF.Sqrt(((1f + m22) - m00) - m11);
            var num2 = 0.5f / num5;
            quaternion.X = (m20 + m02) * num2;
            quaternion.Y = (m21 + m12) * num2;
            quaternion.Z = 0.5f * num5;
            quaternion.W = (m01 - m10) * num2;
            return quaternion;
        }

        public Vector3 ToYawPitchRoll() {
            var sqw = W * (double)W;
            var sqx = X * (double)X;
            var sqy = Y * (double)Y;
            var sqz = Z * (double)Z;
            var unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            var test = X * (double)W - Y * (double)Z;

            Vector3 v;
            if (test > 0.4995 * unit) {
                // singularity at north pole
                v.Y = (float)(2 * Math.Atan2(Y, X));
                v.X = (float)(Math.PI / 2);
                v.Z = 0;
                return v;
            }
            if (test < -0.4995 * unit) {
                // singularity at south pole
                v.Y = (float)(-2 * Math.Atan2(Y, X));
                v.X = (float)(-Math.PI / 2);
                v.Z = 0;
                return v;
            }

            Quaternion q = new Quaternion(W, Z, X, Y);
            v.Y = (float)Math.Atan2(2.0 * q.X * q.W + 2.0 * q.Y * q.Z, 1 - 2.0 * (q.Z * (double)q.Z + q.W * (double)q.W)); // Yaw
            v.X = (float)Math.Asin(2.0 * (q.X * q.Z - q.W * q.Y)); // Pitch
            v.Z = (float)Math.Atan2(2.0 * q.X * q.Y + 2.0 * q.Z * q.W, 1 - 2.0 * (q.Y * (double)q.Y + q.Z * (double)q.Z)); // Roll
            return v;
        }
        public void Normalize() {
            var num = 1.0f / MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);
            this.X = this.X * num;
            this.Y = this.Y * num;
            this.Z = this.Z * num;
            this.W = this.W * num;
        }

        public Quaternion Inverse() {
            var num = 1.0f / (X * X + Y * Y + Z * Z + W * W);
            Quaternion result;
            result.X = -X * num;
            result.Y = -Y * num;
            result.Z = -Z * num;
            result.W = W * num;
            return result;
        }
        public static float Dot(Quaternion quaternion1, Quaternion quaternion2) {
            return (float)(quaternion1.X * (double)quaternion2.X + quaternion1.Y * (double)quaternion2.Y + quaternion1.Z * (double)quaternion2.Z + quaternion1.W * (double)quaternion2.W);
        }
        public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, float amount) {
            double num1 = amount;
            double num2 =
                (quaternion1.X * (double)quaternion2.X + quaternion1.Y * (double)quaternion2.Y +
                quaternion1.Z * (double)quaternion2.Z + quaternion1.W * (double)quaternion2.W);
            bool flag = false;
            if (num2 < 0.0) {
                flag = true;
                num2 = -num2;
            }
            double num3;
            double num4;
            if (num2 > 0.999998986721039) {
                num3 = 1 - num1;
                num4 = flag ? -num1 : num1;
            }
            else {
                var num5 = Math.Acos(num2);
                var num6 = 1.0 / Math.Sin(num5);
                num3 = Math.Sin((1.0 - num1) * num5) * num6;
                num4 = flag
                    ? -Math.Sin(num1 * num5) * num6
                    : Math.Sin(num1 * num5) * num6;
            }
            Quaternion quaternion;
            quaternion.X = (float)(num3 * quaternion1.X + num4 * quaternion2.X);
            quaternion.Y = (float)(num3 * quaternion1.Y + num4 * quaternion2.Y);
            quaternion.Z = (float)(num3 * quaternion1.Z + num4 * quaternion2.Z);
            quaternion.W = (float)(num3 * quaternion1.W + num4 * quaternion2.W);
            return quaternion;
        }
        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount) {
            var num1 = (double)amount;
            var num2 = 1 - num1;
            Quaternion quaternion = new Quaternion();
            if (quaternion1.X * (double)quaternion2.X + quaternion1.Y * (double)quaternion2.Y +
                quaternion1.Z * (double)quaternion2.Z + quaternion1.W * (double)quaternion2.W >= 0.0) {
                quaternion.X = (float)(num2 * quaternion1.X + num1 * quaternion2.X);
                quaternion.Y = (float)(num2 * quaternion1.Y + num1 * quaternion2.Y);
                quaternion.Z = (float)(num2 * quaternion1.Z + num1 * quaternion2.Z);
                quaternion.W = (float)(num2 * quaternion1.W + num1 * quaternion2.W);
            }
            else {
                quaternion.X = (float)(num2 * quaternion1.X - num1 * quaternion2.X);
                quaternion.Y = (float)(num2 * quaternion1.Y - num1 * quaternion2.Y);
                quaternion.Z = (float)(num2 * quaternion1.Z - num1 * quaternion2.Z);
                quaternion.W = (float)(num2 * quaternion1.W - num1 * quaternion2.W);
            }
            var num3 = 1 /
                Math.Sqrt(quaternion.X * (double)quaternion.X +
                quaternion.Y * (double)quaternion.Y +
                quaternion.Z * (double)quaternion.Z +
                quaternion.W * (double)quaternion.W);

            quaternion.X = (float)(quaternion.X * num3);
            quaternion.Y = (float)(quaternion.Y * num3);
            quaternion.Z = (float)(quaternion.Z * num3);
            quaternion.W = (float)(quaternion.W * num3);
            return quaternion;
        }
        public static Quaternion Concatenate(Quaternion value1, Quaternion value2) {
            double num1 = value2.X;
            double num2 = value2.Y;
            double num3 = value2.Z;
            double num4 = value2.W;
            double num5 = value1.X;
            double num6 = value1.Y;
            double num7 = value1.Z;
            double num8 = value1.W;
            double num9 = num2 * num7 - num3 * num6;
            double num10 = num3 * num5 - num1 * num7;
            double num11 = num1 * num6 - num2 * num5;
            double num12 = num1 * num5 + num2 * num6 + num3 * num7;

            Quaternion quaternion;
            quaternion.X = (float)(num1 * num8 + num5 * num4 + num9);
            quaternion.Y = (float)(num2 * num8 + num6 * num4 + num10);
            quaternion.Z = (float)(num3 * num8 + num7 * num4 + num11);
            quaternion.W = (float)(num4 * num8 - num12);
            return quaternion;
        }

        //public static Quaternion CreateFromRotationMatrix(Matrix4 matrix)
        //{
        //    float num1 = matrix.M11 + matrix.M22 + matrix.M33;
        //    Quaternion quaternion = new Quaternion();
        //    if ((double)num1 > 0.0)
        //    {
        //        float num2 = (float)Math.Sqrt((double)num1 + 1.0);
        //        quaternion.W = num2 * 0.5f;
        //        float num3 = 0.5f / num2;
        //        quaternion.X = (matrix.M23 - matrix.M32) * num3;
        //        quaternion.Y = (matrix.M31 - matrix.M13) * num3;
        //        quaternion.Z = (matrix.M12 - matrix.M21) * num3;
        //    }
        //    else if ((double)matrix.M11 >= (double)matrix.M22 && (double)matrix.M11 >= (double)matrix.M33)
        //    {
        //        float num2 = (float)Math.Sqrt(1.0 + (double)matrix.M11 - (double)matrix.M22 - (double)matrix.M33);
        //        float num3 = 0.5f / num2;
        //        quaternion.X = 0.5f * num2;
        //        quaternion.Y = (matrix.M12 + matrix.M21) * num3;
        //        quaternion.Z = (matrix.M13 + matrix.M31) * num3;
        //        quaternion.W = (matrix.M23 - matrix.M32) * num3;
        //    }
        //    else if ((double)matrix.M22 > (double)matrix.M33)
        //    {
        //        float num2 = (float)Math.Sqrt(1.0 + (double)matrix.M22 - (double)matrix.M11 - (double)matrix.M33);
        //        float num3 = 0.5f / num2;
        //        quaternion.X = (matrix.M21 + matrix.M12) * num3;
        //        quaternion.Y = 0.5f * num2;
        //        quaternion.Z = (matrix.M32 + matrix.M23) * num3;
        //        quaternion.W = (matrix.M31 - matrix.M13) * num3;
        //    }
        //    else
        //    {
        //        float num2 = (float)Math.Sqrt(1.0 + (double)matrix.M33 - (double)matrix.M11 - (double)matrix.M22);
        //        float num3 = 0.5f / num2;
        //        quaternion.X = (matrix.M31 + matrix.M13) * num3;
        //        quaternion.Y = (matrix.M32 + matrix.M23) * num3;
        //        quaternion.Z = 0.5f * num2;
        //        quaternion.W = (matrix.M12 - matrix.M21) * num3;
        //    }
        //    return quaternion;
        //}    
    }
}