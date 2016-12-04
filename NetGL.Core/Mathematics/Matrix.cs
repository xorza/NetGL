using System;
using System.Runtime.CompilerServices;

namespace NetGL.Core.Mathematics {
    public class Matrix : IEquatable<Matrix> {
        private float[] _4x4array;
        private float[] _3x3array;

        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public static Matrix Identity {
            get {
                return new Matrix(1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
            }
        }

        public Vector3 Translation {
            get {
                Vector3 vector3;
                vector3.X = this.M41;
                vector3.Y = this.M42;
                vector3.Z = this.M43;
                return vector3;
            }
            set {
                this.M41 = value.X;
                this.M42 = value.Y;
                this.M43 = value.Z;
            }
        }

        public Matrix() {
            LoadIdentity();
        }
        public Matrix(Matrix cloneFrom) {
            this.Load(cloneFrom);
        }
        public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44) {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;
            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        public Matrix(Vector3 position, Quaternion rotation, Vector3 scale) {
            LoadIdentity();

            Transform(position, rotation, scale);
        }

        public void LoadIdentity() {
            M11 = 1;
            M21 = 0;
            M31 = 0;
            M41 = 0;

            M12 = 0;
            M22 = 1;
            M32 = 0;
            M42 = 0;

            M13 = 0;
            M23 = 0;
            M33 = 1;
            M43 = 0;

            M14 = 0;
            M24 = 0;
            M34 = 0;
            M44 = 1;
        }
        public void Load(Matrix m) {
            M11 = m.M11;
            M21 = m.M21;
            M31 = m.M31;
            M41 = m.M41;

            M12 = m.M12;
            M22 = m.M22;
            M32 = m.M32;
            M42 = m.M42;

            M13 = m.M13;
            M23 = m.M23;
            M33 = m.M33;
            M43 = m.M43;

            M14 = m.M14;
            M24 = m.M24;
            M34 = m.M34;
            M44 = m.M44;
        }

        public void LoadPerspectiveFieldOfView(float fov, float aspect, float zNear, float zFar) {
            if (fov <= 0.0f || fov >= 3.14159274101257)
                throw new ArgumentOutOfRangeException("fov");
            if (zNear <= 0.0f)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0.0f)
                throw new ArgumentOutOfRangeException("zFar");

            LoadIdentity();

            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            var tan = Math.Tan(fov / 2.0);

            M11 = (float)(1.0 / (aspect * tan));
            M22 = (float)(1 / tan);
            M33 = (zFar + zNear) / (zNear - zFar);
            M34 = -1f;
            M43 = (2.0f * zFar * zNear) / (zNear - zFar);
            M44 = 0;
        }
        public void LoadOrthographic(float width, float height, float zNearPlane, float zFarPlane) {
            M11 = 2f / width;
            M12 = M13 = M14 = 0.0f;
            M22 = 2f / height;
            M21 = M23 = M24 = 0.0f;
            M33 = 1.0f / (zNearPlane - zFarPlane);
            M31 = M32 = M34 = 0.0f;
            M41 = M42 = 0.0f;
            M43 = zNearPlane / (zNearPlane - zFarPlane);
            M44 = 1f;
        }
        public void LoadOrthographicAspect(float size, float aspect, float zNearPlane, float zFarPlane) {
            LoadOrthographic(aspect * size, size, zNearPlane, zFarPlane);
        }

        public void MultiplyFromRight(Matrix matrix) {
            var m11 =
                (float)
                    (M11 * (double)matrix.M11 + M12 * (double)matrix.M21 +
                     M13 * (double)matrix.M31 + M14 * (double)matrix.M41);
            var m12 =
                (float)
                    (M11 * (double)matrix.M12 + M12 * (double)matrix.M22 +
                     M13 * (double)matrix.M32 + M14 * (double)matrix.M42);
            var m13 =
                (float)
                    (M11 * (double)matrix.M13 + M12 * (double)matrix.M23 +
                     M13 * (double)matrix.M33 + M14 * (double)matrix.M43);
            var m14 =
                (float)
                    (M11 * (double)matrix.M14 + M12 * (double)matrix.M24 +
                     M13 * (double)matrix.M34 + M14 * (double)matrix.M44);
            var m21 =
                (float)
                    (M21 * (double)matrix.M11 + M22 * (double)matrix.M21 +
                     M23 * (double)matrix.M31 + M24 * (double)matrix.M41);
            var m22 =
                (float)
                    (M21 * (double)matrix.M12 + M22 * (double)matrix.M22 +
                     M23 * (double)matrix.M32 + M24 * (double)matrix.M42);
            var m23 =
                (float)
                    (M21 * (double)matrix.M13 + M22 * (double)matrix.M23 +
                     M23 * (double)matrix.M33 + M24 * (double)matrix.M43);
            var m24 =
                (float)
                    (M21 * (double)matrix.M14 + M22 * (double)matrix.M24 +
                     M23 * (double)matrix.M34 + M24 * (double)matrix.M44);
            var m31 =
                (float)
                    (M31 * (double)matrix.M11 + M32 * (double)matrix.M21 +
                     M33 * (double)matrix.M31 + M34 * (double)matrix.M41);
            var m32 =
                (float)
                    (M31 * (double)matrix.M12 + M32 * (double)matrix.M22 +
                     M33 * (double)matrix.M32 + M34 * (double)matrix.M42);
            var m33 =
                (float)
                    (M31 * (double)matrix.M13 + M32 * (double)matrix.M23 +
                     M33 * (double)matrix.M33 + M34 * (double)matrix.M43);
            var m34 =
                (float)
                    (M31 * (double)matrix.M14 + M32 * (double)matrix.M24 +
                     M33 * (double)matrix.M34 + M34 * (double)matrix.M44);
            var m41 =
                (float)
                    (M41 * (double)matrix.M11 + M42 * (double)matrix.M21 +
                     M43 * (double)matrix.M31 + M44 * (double)matrix.M41);
            var m42 =
                (float)
                    (M41 * (double)matrix.M12 + M42 * (double)matrix.M22 +
                     M43 * (double)matrix.M32 + M44 * (double)matrix.M42);
            var m43 =
                (float)
                    (M41 * (double)matrix.M13 + M42 * (double)matrix.M23 +
                     M43 * (double)matrix.M33 + M44 * (double)matrix.M43);
            var m44 =
                (float)
                    (M41 * (double)matrix.M14 + M42 * (double)matrix.M24 +
                     M43 * (double)matrix.M34 + M44 * (double)matrix.M44);

            M11 = m11;
            M21 = m21;
            M31 = m31;
            M41 = m41;

            M12 = m12;
            M22 = m22;
            M32 = m32;
            M42 = m42;

            M13 = m13;
            M23 = m23;
            M33 = m33;
            M43 = m43;

            M14 = m14;
            M24 = m24;
            M34 = m34;
            M44 = m44;
        }
        public void MultiplyFromLeft(Matrix matrix) {
            var m11 =
                (float)
                    (matrix.M11 * (double)M11 + matrix.M12 * (double)M21 +
                     matrix.M13 * (double)M31 + matrix.M14 * (double)M41);
            var m12 =
                (float)
                    (matrix.M11 * (double)M12 + matrix.M12 * (double)M22 +
                     matrix.M13 * (double)M32 + matrix.M14 * (double)M42);
            var m13 =
                (float)
                    (matrix.M11 * (double)M13 + matrix.M12 * (double)M23 +
                     matrix.M13 * (double)M33 + matrix.M14 * (double)M43);
            var m14 =
                (float)
                    (matrix.M11 * (double)M14 + matrix.M12 * (double)M24 +
                     matrix.M13 * (double)M34 + matrix.M14 * (double)M44);
            var m21 =
                (float)
                    (matrix.M21 * (double)M11 + matrix.M22 * (double)M21 +
                     matrix.M23 * (double)M31 + matrix.M24 * (double)M41);
            var m22 =
                (float)
                    (matrix.M21 * (double)M12 + matrix.M22 * (double)M22 +
                     matrix.M23 * (double)M32 + matrix.M24 * (double)M42);
            var m23 =
                (float)
                    (matrix.M21 * (double)M13 + matrix.M22 * (double)M23 +
                     matrix.M23 * (double)M33 + matrix.M24 * (double)M43);
            var m24 =
                (float)
                    (matrix.M21 * (double)M14 + matrix.M22 * (double)M24 +
                     matrix.M23 * (double)M34 + matrix.M24 * (double)M44);
            var m31 =
                (float)
                    (matrix.M31 * (double)M11 + matrix.M32 * (double)M21 +
                     matrix.M33 * (double)M31 + matrix.M34 * (double)M41);
            var m32 =
                (float)
                    (matrix.M31 * (double)M12 + matrix.M32 * (double)M22 +
                     matrix.M33 * (double)M32 + matrix.M34 * (double)M42);
            var m33 =
                (float)
                    (matrix.M31 * (double)M13 + matrix.M32 * (double)M23 +
                     matrix.M33 * (double)M33 + matrix.M34 * (double)M43);
            var m34 =
                (float)
                    (matrix.M31 * (double)M14 + matrix.M32 * (double)M24 +
                     matrix.M33 * (double)M34 + matrix.M34 * (double)M44);
            var m41 =
                (float)
                    (matrix.M41 * (double)M11 + matrix.M42 * (double)M21 +
                     matrix.M43 * (double)M31 + matrix.M44 * (double)M41);
            var m42 =
                (float)
                    (matrix.M41 * (double)M12 + matrix.M42 * (double)M22 +
                     matrix.M43 * (double)M32 + matrix.M44 * (double)M42);
            var m43 =
                (float)
                    (matrix.M41 * (double)M13 + matrix.M42 * (double)M23 +
                     matrix.M43 * (double)M33 + matrix.M44 * (double)M43);
            var m44 =
                (float)
                    (matrix.M41 * (double)M14 + matrix.M42 * (double)M24 +
                     matrix.M43 * (double)M34 + matrix.M44 * (double)M44);

            M11 = m11;
            M21 = m21;
            M31 = m31;
            M41 = m41;

            M12 = m12;
            M22 = m22;
            M32 = m32;
            M42 = m42;

            M13 = m13;
            M23 = m23;
            M33 = m33;
            M43 = m43;

            M14 = m14;
            M24 = m24;
            M34 = m34;
            M44 = m44;
        }

        
        public void Translate(Vector3 translation) {
            if (translation == Vector3.Zero)
                return;

            M41 = M11 * translation.X + M21 * translation.Y + M31 * translation.Z + M41;
            M42 = M12 * translation.X + M22 * translation.Y + M32 * translation.Z + M42;
            M43 = M13 * translation.X + M23 * translation.Y + M33 * translation.Z + M43;
        }
        
        public void Scale(Vector3 scale) {
            if (scale == Vector3.One)
                return;

            M11 *= scale.X;
            M21 *= scale.Y;
            M31 *= scale.Z;

            M12 *= scale.X;
            M22 *= scale.Y;
            M32 *= scale.Z;

            M13 *= scale.X;
            M23 *= scale.Y;
            M33 *= scale.Z;
        }
        
        public void Rotate(Quaternion rotation) {
            if (rotation == Quaternion.Identity)
                return;

            var num1 = rotation.X * rotation.X;
            var num2 = rotation.Y * rotation.Y;
            var num3 = rotation.Z * rotation.Z;
            var num4 = rotation.X * rotation.Y;
            var num5 = rotation.Z * rotation.W;
            var num6 = rotation.Z * rotation.X;
            var num7 = rotation.Y * rotation.W;
            var num8 = rotation.Y * rotation.Z;
            var num9 = rotation.X * rotation.W;

            var rotationM11 = 1f - 2f * (num2 + num3);
            var rotationM12 = 2f * (num4 + num5);
            var rotationM13 = 2f * (num6 - num7);
            var rotationM21 = 2f * (num4 - num5);
            var rotationM22 = 1f - 2f * (num3 + num1);
            var rotationM23 = 2f * (num8 + num9);
            var rotationM31 = 2f * (num6 + num7);
            var rotationM32 = 2f * (num8 - num9);
            var rotationM33 = 1f - 2f * (num2 + num1);

            var m11 = rotationM11 * this.M11 + rotationM12 * this.M21 + rotationM13 * this.M31;
            var m12 = rotationM11 * this.M12 + rotationM12 * this.M22 + rotationM13 * this.M32;
            var m13 = rotationM11 * this.M13 + rotationM12 * this.M23 + rotationM13 * this.M33;
            var m14 = rotationM11 * this.M14 + rotationM12 * this.M24 + rotationM13 * this.M34;
            var m21 = rotationM21 * this.M11 + rotationM22 * this.M21 + rotationM23 * this.M31;
            var m22 = rotationM21 * this.M12 + rotationM22 * this.M22 + rotationM23 * this.M32;
            var m23 = rotationM21 * this.M13 + rotationM22 * this.M23 + rotationM23 * this.M33;
            var m24 = rotationM21 * this.M14 + rotationM22 * this.M24 + rotationM23 * this.M34;
            var m31 = rotationM31 * this.M11 + rotationM32 * this.M21 + rotationM33 * this.M31;
            var m32 = rotationM31 * this.M12 + rotationM32 * this.M22 + rotationM33 * this.M32;
            var m33 = rotationM31 * this.M13 + rotationM32 * this.M23 + rotationM33 * this.M33;
            var m34 = rotationM31 * this.M14 + rotationM32 * this.M24 + rotationM33 * this.M34;

            this.M11 = m11;
            this.M21 = m21;
            this.M31 = m31;

            this.M12 = m12;
            this.M22 = m22;
            this.M32 = m32;

            this.M13 = m13;
            this.M23 = m23;
            this.M33 = m33;

            this.M14 = m14;
            this.M24 = m24;
            this.M34 = m34;
        }
        
        public void Transform(Vector3 position, Quaternion rotation, Vector3 scale) {
            Translate(position);
            Rotate(rotation);
            Scale(scale);
        }

        [Obsolete("not working yet")]
        public void ApplyShadowBias() {
            var half = 0.5f;

            M11 *= half;
            M12 *= half;
            M13 *= half;

            M21 *= half;
            M22 *= half;
            M23 *= half;

            M31 *= half;
            M32 *= half;
            M33 *= half;

            M14 += half;
            M24 += half;
            M34 += half;
        }

        public void Invert() {
            var M11 = this.M11;
            var M12 = this.M12;
            var M13 = this.M13;
            var M14 = this.M14;
            var M21 = this.M21;
            var M22 = this.M22;
            var M23 = this.M23;
            var M24 = this.M24;
            var M31 = this.M31;
            var num10 = this.M32;
            var num11 = this.M33;
            var num12 = this.M34;
            var num13 = this.M41;
            var num14 = this.M42;
            var num15 = this.M43;
            var num16 = this.M44;

            var num17 = num11 * num16 - num12 * num15;
            var num18 = num10 * num16 - num12 * num14;
            var num19 = num10 * num15 - num11 * num14;
            var num20 = M31 * num16 - num12 * num13;
            var num21 = M31 * num15 - num11 * num13;
            var num22 = M31 * num14 - num10 * num13;
            var num23 = M22 * num17 - M23 * num18 + M24 * num19;
            var num24 = -(M21 * num17 - M23 * num20 + M24 * num21);
            var num25 = M21 * num18 - M22 * num20 + M24 * num22;
            var num26 = -(M21 * num19 - M22 * num21 + M23 * num22);
            var num27 = 1f / (M11 * num23 + M12 * num24 + M13 * num25 + M14 * num26);
            var num28 = M23 * num16 - M24 * num15;
            var num29 = M22 * num16 - M24 * num14;
            var num30 = M22 * num15 - M23 * num14;
            var num31 = M21 * num16 - M24 * num13;
            var num32 = M21 * num15 - M23 * num13;
            var num33 = M21 * num14 - M22 * num13;
            var num34 = M23 * num12 - M24 * num11;
            var num35 = M22 * num12 - M24 * num10;
            var num36 = M22 * num11 - M23 * num10;
            var num37 = M21 * num12 - M24 * M31;
            var num38 = M21 * num11 - M23 * M31;
            var num39 = M21 * num10 - M22 * M31;

            this.M11 = num23 * num27;
            this.M21 = num24 * num27;
            this.M31 = num25 * num27;
            this.M41 = num26 * num27;
            this.M12 = -(M12 * num17 - M13 * num18 + M14 * num19) * num27;
            this.M22 = (M11 * num17 - M13 * num20 + M14 * num21) * num27;
            this.M32 = -(M11 * num18 - M12 * num20 + M14 * num22) * num27;
            this.M42 = (M11 * num19 - M12 * num21 + M13 * num22) * num27;
            this.M13 = (M12 * num28 - M13 * num29 + M14 * num30) * num27;
            this.M23 = -(M11 * num28 - M13 * num31 + M14 * num32) * num27;
            this.M33 = (M11 * num29 - M12 * num31 + M14 * num33) * num27;
            this.M43 = -(M11 * num30 - M12 * num32 + M13 * num33) * num27;
            this.M14 = -(M12 * num34 - M13 * num35 + M14 * num36) * num27;
            this.M24 = (M11 * num34 - M13 * num37 + M14 * num38) * num27;
            this.M34 = -(M11 * num35 - M12 * num37 + M14 * num39) * num27;
            this.M44 = (M11 * num36 - M12 * num38 + M13 * num39) * num27;
        }
        public void Transpose() {
            var M11 = this.M11;
            var M12 = this.M21;
            var M13 = this.M31;
            var M14 = this.M41;
            var M21 = this.M12;
            var M22 = this.M22;
            var M23 = this.M32;
            var M24 = this.M42;
            var M31 = this.M13;
            var M32 = this.M23;
            var M33 = this.M33;
            var M34 = this.M43;
            var M41 = this.M14;
            var M42 = this.M24;
            var M43 = this.M34;
            var M44 = this.M44;

            this.M11 = M11;
            this.M21 = M21;
            this.M31 = M31;
            this.M41 = M41;
            this.M12 = M12;
            this.M22 = M22;
            this.M32 = M32;
            this.M42 = M42;
            this.M13 = M13;
            this.M23 = M23;
            this.M33 = M33;
            this.M43 = M43;
            this.M14 = M14;
            this.M24 = M24;
            this.M34 = M34;
            this.M44 = M44;
        }

        public bool Equals(Matrix other) {
            if (other == null)
                return false;

            return (this.M11 == other.M11 && this.M22 == other.M22 &&
                this.M33 == other.M33 && this.M44 == other.M44 &&
                this.M12 == other.M12 && this.M13 == other.M13 &&
                this.M14 == other.M14 && this.M21 == other.M21 &&
                this.M23 == other.M23 && this.M24 == other.M24 &&
                this.M31 == other.M31 && this.M32 == other.M32 &&
                this.M34 == other.M34 && this.M41 == other.M41 &&
                this.M42 == other.M42 && this.M43 == other.M43);
        }
        public override bool Equals(object obj) {
            var other = obj as Matrix;
            if (other != null)
                return Equals(other);
            return false;
        }
        public override int GetHashCode() {
            //return this.M11.GetHashCode() ^ this.M12.GetHashCode() ^ this.M13.GetHashCode() ^ this.M14.GetHashCode() ^
            //       this.M21.GetHashCode() ^ this.M22.GetHashCode() ^ this.M23.GetHashCode() ^ this.M24.GetHashCode() ^
            //       this.M31.GetHashCode() ^ this.M32.GetHashCode() ^ this.M33.GetHashCode() ^ this.M34.GetHashCode() ^
            //       this.M41.GetHashCode() ^ this.M42.GetHashCode() ^ this.M43.GetHashCode() ^ this.M44.GetHashCode();
            return base.GetHashCode();
        }

        internal float[] As4x4Array() {
            if (_4x4array == null)
                _4x4array = new float[16];

            _4x4array[0] = M11;
            _4x4array[1] = M12;
            _4x4array[2] = M13;
            _4x4array[3] = M14;

            _4x4array[4] = M21;
            _4x4array[5] = M22;
            _4x4array[6] = M23;
            _4x4array[7] = M24;

            _4x4array[8] = M31;
            _4x4array[9] = M32;
            _4x4array[10] = M33;
            _4x4array[11] = M34;

            _4x4array[12] = M41;
            _4x4array[13] = M42;
            _4x4array[14] = M43;
            _4x4array[15] = M44;

            return _4x4array;
        }
        internal float[] As3x3Array() {
            if (_3x3array == null)
                _3x3array = new float[9];

            _3x3array[0] = M11;
            _3x3array[1] = M12;
            _3x3array[2] = M13;

            _3x3array[3] = M21;
            _3x3array[4] = M22;
            _3x3array[5] = M23;

            _3x3array[6] = M31;
            _3x3array[7] = M32;
            _3x3array[8] = M33;

            return _3x3array;
        }

        public static void Multiply(Matrix matrix1, Matrix matrix2, Matrix resultTo) {
            resultTo.M11 = matrix2.M11 * matrix1.M11 + matrix2.M12 * matrix1.M21 + matrix2.M13 * matrix1.M31 + matrix2.M14 * matrix1.M41;
            resultTo.M12 = matrix2.M11 * matrix1.M12 + matrix2.M12 * matrix1.M22 + matrix2.M13 * matrix1.M32 + matrix2.M14 * matrix1.M42;
            resultTo.M13 = matrix2.M11 * matrix1.M13 + matrix2.M12 * matrix1.M23 + matrix2.M13 * matrix1.M33 + matrix2.M14 * matrix1.M43;
            resultTo.M14 = matrix2.M11 * matrix1.M14 + matrix2.M12 * matrix1.M24 + matrix2.M13 * matrix1.M34 + matrix2.M14 * matrix1.M44;
            resultTo.M21 = matrix2.M21 * matrix1.M11 + matrix2.M22 * matrix1.M21 + matrix2.M23 * matrix1.M31 + matrix2.M24 * matrix1.M41;
            resultTo.M22 = matrix2.M21 * matrix1.M12 + matrix2.M22 * matrix1.M22 + matrix2.M23 * matrix1.M32 + matrix2.M24 * matrix1.M42;
            resultTo.M23 = matrix2.M21 * matrix1.M13 + matrix2.M22 * matrix1.M23 + matrix2.M23 * matrix1.M33 + matrix2.M24 * matrix1.M43;
            resultTo.M24 = matrix2.M21 * matrix1.M14 + matrix2.M22 * matrix1.M24 + matrix2.M23 * matrix1.M34 + matrix2.M24 * matrix1.M44;
            resultTo.M31 = matrix2.M31 * matrix1.M11 + matrix2.M32 * matrix1.M21 + matrix2.M33 * matrix1.M31 + matrix2.M34 * matrix1.M41;
            resultTo.M32 = matrix2.M31 * matrix1.M12 + matrix2.M32 * matrix1.M22 + matrix2.M33 * matrix1.M32 + matrix2.M34 * matrix1.M42;
            resultTo.M33 = matrix2.M31 * matrix1.M13 + matrix2.M32 * matrix1.M23 + matrix2.M33 * matrix1.M33 + matrix2.M34 * matrix1.M43;
            resultTo.M34 = matrix2.M31 * matrix1.M14 + matrix2.M32 * matrix1.M24 + matrix2.M33 * matrix1.M34 + matrix2.M34 * matrix1.M44;
            resultTo.M41 = matrix2.M41 * matrix1.M11 + matrix2.M42 * matrix1.M21 + matrix2.M43 * matrix1.M31 + matrix2.M44 * matrix1.M41;
            resultTo.M42 = matrix2.M41 * matrix1.M12 + matrix2.M42 * matrix1.M22 + matrix2.M43 * matrix1.M32 + matrix2.M44 * matrix1.M42;
            resultTo.M43 = matrix2.M41 * matrix1.M13 + matrix2.M42 * matrix1.M23 + matrix2.M43 * matrix1.M33 + matrix2.M44 * matrix1.M43;
            resultTo.M44 = matrix2.M41 * matrix1.M14 + matrix2.M42 * matrix1.M24 + matrix2.M43 * matrix1.M34 + matrix2.M44 * matrix1.M44;
        }
        public static void Invert(Matrix source, Matrix resultTo) {
            var num1 = source.M11;
            var num2 = source.M12;
            var num3 = source.M13;
            var num4 = source.M14;
            var num5 = source.M21;
            var num6 = source.M22;
            var num7 = source.M23;
            var num8 = source.M24;
            var num9 = source.M31;
            var num10 = source.M32;
            var num11 = source.M33;
            var num12 = source.M34;
            var num13 = source.M41;
            var num14 = source.M42;
            var num15 = source.M43;
            var num16 = source.M44;

            var num17 = num11 * num16 - num12 * num15;
            var num18 = num10 * num16 - num12 * num14;
            var num19 = num10 * num15 - num11 * num14;
            var num20 = num9 * num16 - num12 * num13;
            var num21 = num9 * num15 - num11 * num13;
            var num22 = num9 * num14 - num10 * num13;
            var num23 = num6 * num17 - num7 * num18 + num8 * num19;
            var num24 = -(num5 * num17 - num7 * num20 + num8 * num21);
            var num25 = num5 * num18 - num6 * num20 + num8 * num22;
            var num26 = -(num5 * num19 - num6 * num21 + num7 * num22);
            var num27 = 1f / (num1 * num23 + num2 * num24 + num3 * num25 + num4 * num26);
            var num28 = num7 * num16 - num8 * num15;
            var num29 = num6 * num16 - num8 * num14;
            var num30 = num6 * num15 - num7 * num14;
            var num31 = num5 * num16 - num8 * num13;
            var num32 = num5 * num15 - num7 * num13;
            var num33 = num5 * num14 - num6 * num13;
            var num34 = num7 * num12 - num8 * num11;
            var num35 = num6 * num12 - num8 * num10;
            var num36 = num6 * num11 - num7 * num10;
            var num37 = num5 * num12 - num8 * num9;
            var num38 = num5 * num11 - num7 * num9;
            var num39 = num5 * num10 - num6 * num9;

            resultTo.M11 = num23 * num27;
            resultTo.M21 = num24 * num27;
            resultTo.M31 = num25 * num27;
            resultTo.M41 = num26 * num27;
            resultTo.M12 = -(num2 * num17 - num3 * num18 + num4 * num19) * num27;
            resultTo.M22 = (num1 * num17 - num3 * num20 + num4 * num21) * num27;
            resultTo.M32 = -(num1 * num18 - num2 * num20 + num4 * num22) * num27;
            resultTo.M42 = (num1 * num19 - num2 * num21 + num3 * num22) * num27;
            resultTo.M13 = (num2 * num28 - num3 * num29 + num4 * num30) * num27;
            resultTo.M23 = -(num1 * num28 - num3 * num31 + num4 * num32) * num27;
            resultTo.M33 = (num1 * num29 - num2 * num31 + num4 * num33) * num27;
            resultTo.M43 = -(num1 * num30 - num2 * num32 + num3 * num33) * num27;
            resultTo.M14 = -(num2 * num34 - num3 * num35 + num4 * num36) * num27;
            resultTo.M24 = (num1 * num34 - num3 * num37 + num4 * num38) * num27;
            resultTo.M34 = -(num1 * num35 - num2 * num37 + num4 * num39) * num27;
            resultTo.M44 = (num1 * num36 - num2 * num38 + num3 * num39) * num27;
        }

        public override string ToString() {
            var f =
                "{0:F2} {1:F2} {2:F2} {3:F2}\n" +
                "{4:F2} {5:F2} {6:F2} {7:F2}\n" +
                "{8:F2} {9:F2} {10:F2} {11:F2}\n" +
                "{12:F2} {13:F2} {14:F2} {15:F2}";

            return string.Format(f, M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
        }

        //public Vector3 Up
        //{
        //    get
        //    {
        //        Vector3 vector3;
        //        vector3.X = this.M21;
        //        vector3.Y = this.M22;
        //        vector3.Z = this.M23;
        //        return vector3;
        //    }
        //    set
        //    {
        //        this.M21 = value.X;
        //        this.M22 = value.Y;
        //        this.M23 = value.Z;
        //    }
        //}
        //public Vector3 Down
        //{
        //    get
        //    {
        //        Vector3 vector3;
        //        vector3.X = -this.M21;
        //        vector3.Y = -this.M22;
        //        vector3.Z = -this.M23;
        //        return vector3;
        //    }
        //    set
        //    {
        //        this.M21 = -value.X;
        //        this.M22 = -value.Y;
        //        this.M23 = -value.Z;
        //    }
        //}
        //public Vector3 Right
        //{
        //    get
        //    {
        //        Vector3 vector3;
        //        vector3.X = this.M11;
        //        vector3.Y = this.M12;
        //        vector3.Z = this.M13;
        //        return vector3;
        //    }
        //    set
        //    {
        //        this.M11 = value.X;
        //        this.M12 = value.Y;
        //        this.M13 = value.Z;
        //    }
        //}
        //public Vector3 Left
        //{
        //    get
        //    {
        //        Vector3 vector3;
        //        vector3.X = -this.M11;
        //        vector3.Y = -this.M12;
        //        vector3.Z = -this.M13;
        //        return vector3;
        //    }
        //    set
        //    {
        //        this.M11 = -value.X;
        //        this.M12 = -value.Y;
        //        this.M13 = -value.Z;
        //    }
        //}
        //public Vector3 Forward
        //{
        //    get
        //    {
        //        Vector3 vector3;
        //        vector3.X = -this.M31;
        //        vector3.Y = -this.M32;
        //        vector3.Z = -this.M33;
        //        return vector3;
        //    }
        //    set
        //    {
        //        this.M31 = -value.X;
        //        this.M32 = -value.Y;
        //        this.M33 = -value.Z;
        //    }
        //}
        //public Vector3 Backward
        //{
        //    get
        //    {
        //        Vector3 vector3;
        //        vector3.X = this.M31;
        //        vector3.Y = this.M32;
        //        vector3.Z = this.M33;
        //        return vector3;
        //    }
        //    set
        //    {
        //        this.M31 = value.X;
        //        this.M32 = value.Y;
        //        this.M33 = value.Z;
        //    }
        //}
        //public float Determinant()
        //{
        //    float num1 = this.M11;
        //    float num2 = this.M12;
        //    float num3 = this.M13;
        //    float num4 = this.M14;
        //    float num5 = this.M21;
        //    float num6 = this.M22;
        //    float num7 = this.M23;
        //    float num8 = this.M24;
        //    float num9 = this.M31;
        //    float num10 = this.M32;
        //    float num11 = this.M33;
        //    float num12 = this.M34;
        //    float num13 = this.M41;
        //    float num14 = this.M42;
        //    float num15 = this.M43;
        //    float num16 = this.M44;
        //    float num17 = (float)((double)num11 * (double)num16 - (double)num12 * (double)num15);
        //    float num18 = (float)((double)num10 * (double)num16 - (double)num12 * (double)num14);
        //    float num19 = (float)((double)num10 * (double)num15 - (double)num11 * (double)num14);
        //    float num20 = (float)((double)num9 * (double)num16 - (double)num12 * (double)num13);
        //    float num21 = (float)((double)num9 * (double)num15 - (double)num11 * (double)num13);
        //    float num22 = (float)((double)num9 * (double)num14 - (double)num10 * (double)num13);
        //    return (float)((double)num1 * ((double)num6 * (double)num17 - (double)num7 * (double)num18 + (double)num8 * (double)num19) - (double)num2 * ((double)num5 * (double)num17 - (double)num7 * (double)num20 + (double)num8 * (double)num21) + (double)num3 * ((double)num5 * (double)num18 - (double)num6 * (double)num20 + (double)num8 * (double)num22) - (double)num4 * ((double)num5 * (double)num19 - (double)num6 * (double)num21 + (double)num7 * (double)num22));
        //}

        //public static Matrix Lerp(Matrix matrix1, Matrix matrix2, float amount)
        //{
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = matrix1.M11 + (matrix2.M11 - matrix1.M11) * amount;
        //    matrix.M12 = matrix1.M12 + (matrix2.M12 - matrix1.M12) * amount;
        //    matrix.M13 = matrix1.M13 + (matrix2.M13 - matrix1.M13) * amount;
        //    matrix.M14 = matrix1.M14 + (matrix2.M14 - matrix1.M14) * amount;
        //    matrix.M21 = matrix1.M21 + (matrix2.M21 - matrix1.M21) * amount;
        //    matrix.M22 = matrix1.M22 + (matrix2.M22 - matrix1.M22) * amount;
        //    matrix.M23 = matrix1.M23 + (matrix2.M23 - matrix1.M23) * amount;
        //    matrix.M24 = matrix1.M24 + (matrix2.M24 - matrix1.M24) * amount;
        //    matrix.M31 = matrix1.M31 + (matrix2.M31 - matrix1.M31) * amount;
        //    matrix.M32 = matrix1.M32 + (matrix2.M32 - matrix1.M32) * amount;
        //    matrix.M33 = matrix1.M33 + (matrix2.M33 - matrix1.M33) * amount;
        //    matrix.M34 = matrix1.M34 + (matrix2.M34 - matrix1.M34) * amount;
        //    matrix.M41 = matrix1.M41 + (matrix2.M41 - matrix1.M41) * amount;
        //    matrix.M42 = matrix1.M42 + (matrix2.M42 - matrix1.M42) * amount;
        //    matrix.M43 = matrix1.M43 + (matrix2.M43 - matrix1.M43) * amount;
        //    matrix.M44 = matrix1.M44 + (matrix2.M44 - matrix1.M44) * amount;
        //    return matrix;
        //}

        //public static Matrix Negate(Matrix matrix)
        //{
        //    Matrix matrix1 = new Matrix();
        //    matrix1.M11 = -matrix.M11;
        //    matrix1.M12 = -matrix.M12;
        //    matrix1.M13 = -matrix.M13;
        //    matrix1.M14 = -matrix.M14;
        //    matrix1.M21 = -matrix.M21;
        //    matrix1.M22 = -matrix.M22;
        //    matrix1.M23 = -matrix.M23;
        //    matrix1.M24 = -matrix.M24;
        //    matrix1.M31 = -matrix.M31;
        //    matrix1.M32 = -matrix.M32;
        //    matrix1.M33 = -matrix.M33;
        //    matrix1.M34 = -matrix.M34;
        //    matrix1.M41 = -matrix.M41;
        //    matrix1.M42 = -matrix.M42;
        //    matrix1.M43 = -matrix.M43;
        //    matrix1.M44 = -matrix.M44;
        //    return matrix1;
        //}

        //public static Matrix Add(Matrix matrix1, Matrix matrix2)
        //{
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = matrix1.M11 + matrix2.M11;
        //    matrix.M12 = matrix1.M12 + matrix2.M12;
        //    matrix.M13 = matrix1.M13 + matrix2.M13;
        //    matrix.M14 = matrix1.M14 + matrix2.M14;
        //    matrix.M21 = matrix1.M21 + matrix2.M21;
        //    matrix.M22 = matrix1.M22 + matrix2.M22;
        //    matrix.M23 = matrix1.M23 + matrix2.M23;
        //    matrix.M24 = matrix1.M24 + matrix2.M24;
        //    matrix.M31 = matrix1.M31 + matrix2.M31;
        //    matrix.M32 = matrix1.M32 + matrix2.M32;
        //    matrix.M33 = matrix1.M33 + matrix2.M33;
        //    matrix.M34 = matrix1.M34 + matrix2.M34;
        //    matrix.M41 = matrix1.M41 + matrix2.M41;
        //    matrix.M42 = matrix1.M42 + matrix2.M42;
        //    matrix.M43 = matrix1.M43 + matrix2.M43;
        //    matrix.M44 = matrix1.M44 + matrix2.M44;
        //    return matrix;
        //}

        //public static Matrix Subtract(Matrix matrix1, Matrix matrix2)
        //{
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = matrix1.M11 - matrix2.M11;
        //    matrix.M12 = matrix1.M12 - matrix2.M12;
        //    matrix.M13 = matrix1.M13 - matrix2.M13;
        //    matrix.M14 = matrix1.M14 - matrix2.M14;
        //    matrix.M21 = matrix1.M21 - matrix2.M21;
        //    matrix.M22 = matrix1.M22 - matrix2.M22;
        //    matrix.M23 = matrix1.M23 - matrix2.M23;
        //    matrix.M24 = matrix1.M24 - matrix2.M24;
        //    matrix.M31 = matrix1.M31 - matrix2.M31;
        //    matrix.M32 = matrix1.M32 - matrix2.M32;
        //    matrix.M33 = matrix1.M33 - matrix2.M33;
        //    matrix.M34 = matrix1.M34 - matrix2.M34;
        //    matrix.M41 = matrix1.M41 - matrix2.M41;
        //    matrix.M42 = matrix1.M42 - matrix2.M42;
        //    matrix.M43 = matrix1.M43 - matrix2.M43;
        //    matrix.M44 = matrix1.M44 - matrix2.M44;
        //    return matrix;
        //}        

        //public static Matrix Multiply(Matrix matrix1, float scaleFactor)
        //{
        //    float num = scaleFactor;
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = matrix1.M11 * num;
        //    matrix.M12 = matrix1.M12 * num;
        //    matrix.M13 = matrix1.M13 * num;
        //    matrix.M14 = matrix1.M14 * num;
        //    matrix.M21 = matrix1.M21 * num;
        //    matrix.M22 = matrix1.M22 * num;
        //    matrix.M23 = matrix1.M23 * num;
        //    matrix.M24 = matrix1.M24 * num;
        //    matrix.M31 = matrix1.M31 * num;
        //    matrix.M32 = matrix1.M32 * num;
        //    matrix.M33 = matrix1.M33 * num;
        //    matrix.M34 = matrix1.M34 * num;
        //    matrix.M41 = matrix1.M41 * num;
        //    matrix.M42 = matrix1.M42 * num;
        //    matrix.M43 = matrix1.M43 * num;
        //    matrix.M44 = matrix1.M44 * num;
        //    return matrix;
        //}

        //public static Matrix Divide(Matrix matrix1, Matrix matrix2)
        //{
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = matrix1.M11 / matrix2.M11;
        //    matrix.M12 = matrix1.M12 / matrix2.M12;
        //    matrix.M13 = matrix1.M13 / matrix2.M13;
        //    matrix.M14 = matrix1.M14 / matrix2.M14;
        //    matrix.M21 = matrix1.M21 / matrix2.M21;
        //    matrix.M22 = matrix1.M22 / matrix2.M22;
        //    matrix.M23 = matrix1.M23 / matrix2.M23;
        //    matrix.M24 = matrix1.M24 / matrix2.M24;
        //    matrix.M31 = matrix1.M31 / matrix2.M31;
        //    matrix.M32 = matrix1.M32 / matrix2.M32;
        //    matrix.M33 = matrix1.M33 / matrix2.M33;
        //    matrix.M34 = matrix1.M34 / matrix2.M34;
        //    matrix.M41 = matrix1.M41 / matrix2.M41;
        //    matrix.M42 = matrix1.M42 / matrix2.M42;
        //    matrix.M43 = matrix1.M43 / matrix2.M43;
        //    matrix.M44 = matrix1.M44 / matrix2.M44;
        //    return matrix;
        //}

        //public static Matrix Divide(Matrix matrix1, float divider)
        //{
        //    float num = 1f / divider;
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = matrix1.M11 * num;
        //    matrix.M12 = matrix1.M12 * num;
        //    matrix.M13 = matrix1.M13 * num;
        //    matrix.M14 = matrix1.M14 * num;
        //    matrix.M21 = matrix1.M21 * num;
        //    matrix.M22 = matrix1.M22 * num;
        //    matrix.M23 = matrix1.M23 * num;
        //    matrix.M24 = matrix1.M24 * num;
        //    matrix.M31 = matrix1.M31 * num;
        //    matrix.M32 = matrix1.M32 * num;
        //    matrix.M33 = matrix1.M33 * num;
        //    matrix.M34 = matrix1.M34 * num;
        //    matrix.M41 = matrix1.M41 * num;
        //    matrix.M42 = matrix1.M42 * num;
        //    matrix.M43 = matrix1.M43 * num;
        //    matrix.M44 = matrix1.M44 * num;
        //    return matrix;
        //}

        //public static Matrix operator -(Matrix matrix1)
        //{
        //    var matrix = new Matrix();
        //    matrix.M11 = -matrix1.M11;
        //    matrix.M12 = -matrix1.M12;
        //    matrix.M13 = -matrix1.M13;
        //    matrix.M14 = -matrix1.M14;
        //    matrix.M21 = -matrix1.M21;
        //    matrix.M22 = -matrix1.M22;
        //    matrix.M23 = -matrix1.M23;
        //    matrix.M24 = -matrix1.M24;
        //    matrix.M31 = -matrix1.M31;
        //    matrix.M32 = -matrix1.M32;
        //    matrix.M33 = -matrix1.M33;
        //    matrix.M34 = -matrix1.M34;
        //    matrix.M41 = -matrix1.M41;
        //    matrix.M42 = -matrix1.M42;
        //    matrix.M43 = -matrix1.M43;
        //    matrix.M44 = -matrix1.M44;
        //    return matrix;
        //}

        //public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        //{
        //    var matrix = new Matrix();
        //    matrix.M11 = matrix1.M11 + matrix2.M11;
        //    matrix.M12 = matrix1.M12 + matrix2.M12;
        //    matrix.M13 = matrix1.M13 + matrix2.M13;
        //    matrix.M14 = matrix1.M14 + matrix2.M14;
        //    matrix.M21 = matrix1.M21 + matrix2.M21;
        //    matrix.M22 = matrix1.M22 + matrix2.M22;
        //    matrix.M23 = matrix1.M23 + matrix2.M23;
        //    matrix.M24 = matrix1.M24 + matrix2.M24;
        //    matrix.M31 = matrix1.M31 + matrix2.M31;
        //    matrix.M32 = matrix1.M32 + matrix2.M32;
        //    matrix.M33 = matrix1.M33 + matrix2.M33;
        //    matrix.M34 = matrix1.M34 + matrix2.M34;
        //    matrix.M41 = matrix1.M41 + matrix2.M41;
        //    matrix.M42 = matrix1.M42 + matrix2.M42;
        //    matrix.M43 = matrix1.M43 + matrix2.M43;
        //    matrix.M44 = matrix1.M44 + matrix2.M44;
        //    return matrix;
        //}

        //public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        //{
        //    var matrix = new Matrix();
        //    matrix.M11 = matrix1.M11 - matrix2.M11;
        //    matrix.M12 = matrix1.M12 - matrix2.M12;
        //    matrix.M13 = matrix1.M13 - matrix2.M13;
        //    matrix.M14 = matrix1.M14 - matrix2.M14;
        //    matrix.M21 = matrix1.M21 - matrix2.M21;
        //    matrix.M22 = matrix1.M22 - matrix2.M22;
        //    matrix.M23 = matrix1.M23 - matrix2.M23;
        //    matrix.M24 = matrix1.M24 - matrix2.M24;
        //    matrix.M31 = matrix1.M31 - matrix2.M31;
        //    matrix.M32 = matrix1.M32 - matrix2.M32;
        //    matrix.M33 = matrix1.M33 - matrix2.M33;
        //    matrix.M34 = matrix1.M34 - matrix2.M34;
        //    matrix.M41 = matrix1.M41 - matrix2.M41;
        //    matrix.M42 = matrix1.M42 - matrix2.M42;
        //    matrix.M43 = matrix1.M43 - matrix2.M43;
        //    matrix.M44 = matrix1.M44 - matrix2.M44;
        //    return matrix;
        //}

        //public static Matrix operator *(Matrix matrix, float scaleFactor)
        //{
        //    float num = scaleFactor;
        //    var matrix1 = new Matrix();
        //    matrix1.M11 = matrix.M11 * num;
        //    matrix1.M12 = matrix.M12 * num;
        //    matrix1.M13 = matrix.M13 * num;
        //    matrix1.M14 = matrix.M14 * num;
        //    matrix1.M21 = matrix.M21 * num;
        //    matrix1.M22 = matrix.M22 * num;
        //    matrix1.M23 = matrix.M23 * num;
        //    matrix1.M24 = matrix.M24 * num;
        //    matrix1.M31 = matrix.M31 * num;
        //    matrix1.M32 = matrix.M32 * num;
        //    matrix1.M33 = matrix.M33 * num;
        //    matrix1.M34 = matrix.M34 * num;
        //    matrix1.M41 = matrix.M41 * num;
        //    matrix1.M42 = matrix.M42 * num;
        //    matrix1.M43 = matrix.M43 * num;
        //    matrix1.M44 = matrix.M44 * num;
        //    return matrix1;
        //}

        //public static Matrix operator /(Matrix matrix1, Matrix matrix2)
        //{
        //    var matrix = new Matrix();
        //    matrix.M11 = matrix1.M11 / matrix2.M11;
        //    matrix.M12 = matrix1.M12 / matrix2.M12;
        //    matrix.M13 = matrix1.M13 / matrix2.M13;
        //    matrix.M14 = matrix1.M14 / matrix2.M14;
        //    matrix.M21 = matrix1.M21 / matrix2.M21;
        //    matrix.M22 = matrix1.M22 / matrix2.M22;
        //    matrix.M23 = matrix1.M23 / matrix2.M23;
        //    matrix.M24 = matrix1.M24 / matrix2.M24;
        //    matrix.M31 = matrix1.M31 / matrix2.M31;
        //    matrix.M32 = matrix1.M32 / matrix2.M32;
        //    matrix.M33 = matrix1.M33 / matrix2.M33;
        //    matrix.M34 = matrix1.M34 / matrix2.M34;
        //    matrix.M41 = matrix1.M41 / matrix2.M41;
        //    matrix.M42 = matrix1.M42 / matrix2.M42;
        //    matrix.M43 = matrix1.M43 / matrix2.M43;
        //    matrix.M44 = matrix1.M44 / matrix2.M44;
        //    return matrix;
        //}

        //public static Matrix operator /(Matrix matrix1, float divider)
        //{
        //    float num = 1f / divider;
        //    var matrix = new Matrix();
        //    matrix.M11 = matrix1.M11 * num;
        //    matrix.M12 = matrix1.M12 * num;
        //    matrix.M13 = matrix1.M13 * num;
        //    matrix.M14 = matrix1.M14 * num;
        //    matrix.M21 = matrix1.M21 * num;
        //    matrix.M22 = matrix1.M22 * num;
        //    matrix.M23 = matrix1.M23 * num;
        //    matrix.M24 = matrix1.M24 * num;
        //    matrix.M31 = matrix1.M31 * num;
        //    matrix.M32 = matrix1.M32 * num;
        //    matrix.M33 = matrix1.M33 * num;
        //    matrix.M34 = matrix1.M34 * num;
        //    matrix.M41 = matrix1.M41 * num;
        //    matrix.M42 = matrix1.M42 * num;
        //    matrix.M43 = matrix1.M43 * num;
        //    matrix.M44 = matrix1.M44 * num;
        //    return matrix;
        //}

        //public static Matrix CreateBillboard(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3? cameraForwardVector)
        //{
        //    Vector3 result1;
        //    result1.X = objectPosition.X - cameraPosition.X;
        //    result1.Y = objectPosition.Y - cameraPosition.Y;
        //    result1.Z = objectPosition.Z - cameraPosition.Z;
        //    float num = result1.LengthSquared();
        //    if ((double)num < 9.99999974737875E-05)
        //        result1 = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
        //    else
        //        Vector3.Multiply(ref result1, 1f / (float)Math.Sqrt((double)num), out result1);
        //    Vector3 result2;
        //    Vector3.Cross(ref cameraUpVector, ref result1, out result2);
        //    result2.Normalize();
        //    Vector3 result3;
        //    Vector3.Cross(ref result1, ref result2, out result3);
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = result2.X;
        //    matrix.M12 = result2.Y;
        //    matrix.M13 = result2.Z;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = result3.X;
        //    matrix.M22 = result3.Y;
        //    matrix.M23 = result3.Z;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = result1.X;
        //    matrix.M32 = result1.Y;
        //    matrix.M33 = result1.Z;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = objectPosition.X;
        //    matrix.M42 = objectPosition.Y;
        //    matrix.M43 = objectPosition.Z;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateConstrainedBillboard(Vector3 objectPosition, Vector3 cameraPosition, Vector3 rotateAxis, Vector3? cameraForwardVector, Vector3? objectForwardVector)
        //{
        //    Vector3 result1;
        //    result1.X = objectPosition.X - cameraPosition.X;
        //    result1.Y = objectPosition.Y - cameraPosition.Y;
        //    result1.Z = objectPosition.Z - cameraPosition.Z;
        //    float num = result1.LengthSquared();
        //    if ((double)num < 9.99999974737875E-05)
        //        result1 = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
        //    else
        //        Vector3.Multiply(ref result1, 1f / (float)Math.Sqrt((double)num), out result1);
        //    Vector3 vector2 = rotateAxis;
        //    float result2;
        //    Vector3.Dot(ref rotateAxis, ref result1, out result2);
        //    Vector3 result3;
        //    Vector3 result4;
        //    if ((double)Math.Abs(result2) > 0.998254656791687)
        //    {
        //        if (objectForwardVector.HasValue)
        //        {
        //            result3 = objectForwardVector.Value;
        //            Vector3.Dot(ref rotateAxis, ref result3, out result2);
        //            if ((double)Math.Abs(result2) > 0.998254656791687)
        //                result3 = (double)Math.Abs((float)((double)rotateAxis.X * (double)Vector3.Forward.X + (double)rotateAxis.Y * (double)Vector3.Forward.Y + (double)rotateAxis.Z * (double)Vector3.Forward.Z)) > 0.998254656791687 ? Vector3.Right : Vector3.Forward;
        //        }
        //        else
        //            result3 = (double)Math.Abs((float)((double)rotateAxis.X * (double)Vector3.Forward.X + (double)rotateAxis.Y * (double)Vector3.Forward.Y + (double)rotateAxis.Z * (double)Vector3.Forward.Z)) > 0.998254656791687 ? Vector3.Right : Vector3.Forward;
        //        Vector3.Cross(ref rotateAxis, ref result3, out result4);
        //        result4.Normalize();
        //        Vector3.Cross(ref result4, ref rotateAxis, out result3);
        //        result3.Normalize();
        //    }
        //    else
        //    {
        //        Vector3.Cross(ref rotateAxis, ref result1, out result4);
        //        result4.Normalize();
        //        Vector3.Cross(ref result4, ref vector2, out result3);
        //        result3.Normalize();
        //    }
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = result4.X;
        //    matrix.M12 = result4.Y;
        //    matrix.M13 = result4.Z;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = vector2.X;
        //    matrix.M22 = vector2.Y;
        //    matrix.M23 = vector2.Z;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = result3.X;
        //    matrix.M32 = result3.Y;
        //    matrix.M33 = result3.Z;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = objectPosition.X;
        //    matrix.M42 = objectPosition.Y;
        //    matrix.M43 = objectPosition.Z;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateTranslation(Vector3 position)
        //{
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = 1f;
        //    matrix.M12 = 0.0f;
        //    matrix.M13 = 0.0f;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = 0.0f;
        //    matrix.M22 = 1f;
        //    matrix.M23 = 0.0f;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = 0.0f;
        //    matrix.M32 = 0.0f;
        //    matrix.M33 = 1f;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = position.X;
        //    matrix.M42 = position.Y;
        //    matrix.M43 = position.Z;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateScale(float xScale, float yScale, float zScale)
        //{
        //    float num1 = xScale;
        //    float num2 = yScale;
        //    float num3 = zScale;
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = num1;
        //    matrix.M12 = 0.0f;
        //    matrix.M13 = 0.0f;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = 0.0f;
        //    matrix.M22 = num2;
        //    matrix.M23 = 0.0f;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = 0.0f;
        //    matrix.M32 = 0.0f;
        //    matrix.M33 = num3;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = 0.0f;
        //    matrix.M42 = 0.0f;
        //    matrix.M43 = 0.0f;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateScale(Vector3 scales)
        //{
        //    float num1 = scales.X;
        //    float num2 = scales.Y;
        //    float num3 = scales.Z;
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = num1;
        //    matrix.M12 = 0.0f;
        //    matrix.M13 = 0.0f;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = 0.0f;
        //    matrix.M22 = num2;
        //    matrix.M23 = 0.0f;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = 0.0f;
        //    matrix.M32 = 0.0f;
        //    matrix.M33 = num3;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = 0.0f;
        //    matrix.M42 = 0.0f;
        //    matrix.M43 = 0.0f;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateScale(float scale)
        //{
        //    float num = scale;
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = num;
        //    matrix.M12 = 0.0f;
        //    matrix.M13 = 0.0f;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = 0.0f;
        //    matrix.M22 = num;
        //    matrix.M23 = 0.0f;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = 0.0f;
        //    matrix.M32 = 0.0f;
        //    matrix.M33 = num;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = 0.0f;
        //    matrix.M42 = 0.0f;
        //    matrix.M43 = 0.0f;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateRotationX(float radians)
        //{
        //    float num1 = (float)Math.Cos((double)radians);
        //    float num2 = (float)Math.Sin((double)radians);
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = 1f;
        //    matrix.M12 = 0.0f;
        //    matrix.M13 = 0.0f;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = 0.0f;
        //    matrix.M22 = num1;
        //    matrix.M23 = num2;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = 0.0f;
        //    matrix.M32 = -num2;
        //    matrix.M33 = num1;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = 0.0f;
        //    matrix.M42 = 0.0f;
        //    matrix.M43 = 0.0f;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateRotationY(float radians)
        //{
        //    float num1 = (float)Math.Cos((double)radians);
        //    float num2 = (float)Math.Sin((double)radians);
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = num1;
        //    matrix.M12 = 0.0f;
        //    matrix.M13 = -num2;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = 0.0f;
        //    matrix.M22 = 1f;
        //    matrix.M23 = 0.0f;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = num2;
        //    matrix.M32 = 0.0f;
        //    matrix.M33 = num1;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = 0.0f;
        //    matrix.M42 = 0.0f;
        //    matrix.M43 = 0.0f;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateRotationZ(float radians)
        //{
        //    float num1 = (float)Math.Cos((double)radians);
        //    float num2 = (float)Math.Sin((double)radians);
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = num1;
        //    matrix.M12 = num2;
        //    matrix.M13 = 0.0f;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = -num2;
        //    matrix.M22 = num1;
        //    matrix.M23 = 0.0f;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = 0.0f;
        //    matrix.M32 = 0.0f;
        //    matrix.M33 = 1f;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = 0.0f;
        //    matrix.M42 = 0.0f;
        //    matrix.M43 = 0.0f;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateFromAxisAngle(Vector3 axis, float angle)
        //{
        //    float num1 = axis.X;
        //    float num2 = axis.Y;
        //    float num3 = axis.Z;
        //    float num4 = (float)Math.Sin((double)angle);
        //    float num5 = (float)Math.Cos((double)angle);
        //    float num6 = num1 * num1;
        //    float num7 = num2 * num2;
        //    float num8 = num3 * num3;
        //    float num9 = num1 * num2;
        //    float num10 = num1 * num3;
        //    float num11 = num2 * num3;
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = num6 + num5 * (1f - num6);
        //    matrix.M12 = (float)((double)num9 - (double)num5 * (double)num9 + (double)num4 * (double)num3);
        //    matrix.M13 = (float)((double)num10 - (double)num5 * (double)num10 - (double)num4 * (double)num2);
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = (float)((double)num9 - (double)num5 * (double)num9 - (double)num4 * (double)num3);
        //    matrix.M22 = num7 + num5 * (1f - num7);
        //    matrix.M23 = (float)((double)num11 - (double)num5 * (double)num11 + (double)num4 * (double)num1);
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = (float)((double)num10 - (double)num5 * (double)num10 + (double)num4 * (double)num2);
        //    matrix.M32 = (float)((double)num11 - (double)num5 * (double)num11 - (double)num4 * (double)num1);
        //    matrix.M33 = num8 + num5 * (1f - num8);
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = 0.0f;
        //    matrix.M42 = 0.0f;
        //    matrix.M43 = 0.0f;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
        //{
        //    if ((double)nearPlaneDistance <= 0.0)
        //        throw new ArgumentOutOfRangeException("nearPlaneDistance");
        //    else if ((double)farPlaneDistance <= 0.0)
        //    {
        //        throw new ArgumentOutOfRangeException("farPlaneDistance");
        //    }
        //    else
        //    {
        //        if ((double)nearPlaneDistance >= (double)farPlaneDistance)
        //            throw new ArgumentOutOfRangeException("nearPlaneDistance");
        //        Matrix matrix = new Matrix();
        //        matrix.M11 = (float)(2.0 * (double)nearPlaneDistance / ((double)right - (double)left));
        //        matrix.M12 = matrix.M13 = matrix.M14 = 0.0f;
        //        matrix.M22 = (float)(2.0 * (double)nearPlaneDistance / ((double)top - (double)bottom));
        //        matrix.M21 = matrix.M23 = matrix.M24 = 0.0f;
        //        matrix.M31 = (float)(((double)left + (double)right) / ((double)right - (double)left));
        //        matrix.M32 = (float)(((double)top + (double)bottom) / ((double)top - (double)bottom));
        //        matrix.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        //        matrix.M34 = -1f;
        //        matrix.M43 = (float)((double)nearPlaneDistance * (double)farPlaneDistance / ((double)nearPlaneDistance - (double)farPlaneDistance));
        //        matrix.M41 = matrix.M42 = matrix.M44 = 0.0f;
        //        return matrix;
        //    }
        //}

        //public static Matrix CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
        //{
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = (float)(2.0 / ((double)right - (double)left));
        //    matrix.M12 = matrix.M13 = matrix.M14 = 0.0f;
        //    matrix.M22 = (float)(2.0 / ((double)top - (double)bottom));
        //    matrix.M21 = matrix.M23 = matrix.M24 = 0.0f;
        //    matrix.M33 = (float)(1.0 / ((double)zNearPlane - (double)zFarPlane));
        //    matrix.M31 = matrix.M32 = matrix.M34 = 0.0f;
        //    matrix.M41 = (float)(((double)left + (double)right) / ((double)left - (double)right));
        //    matrix.M42 = (float)(((double)top + (double)bottom) / ((double)bottom - (double)top));
        //    matrix.M43 = zNearPlane / (zNearPlane - zFarPlane);
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        //{
        //    Vector3 vector3_1 = Vector3.Normalize(cameraPosition - cameraTarget);
        //    Vector3 vector3_2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector3_1));
        //    Vector3 vector1 = Vector3.Cross(vector3_1, vector3_2);
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = vector3_2.X;
        //    matrix.M12 = vector1.X;
        //    matrix.M13 = vector3_1.X;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = vector3_2.Y;
        //    matrix.M22 = vector1.Y;
        //    matrix.M23 = vector3_1.Y;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = vector3_2.Z;
        //    matrix.M32 = vector1.Z;
        //    matrix.M33 = vector3_1.Z;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = -Vector3.Dot(vector3_2, cameraPosition);
        //    matrix.M42 = -Vector3.Dot(vector1, cameraPosition);
        //    matrix.M43 = -Vector3.Dot(vector3_1, cameraPosition);
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
        //{
        //    Vector3 vector3_1 = Vector3.Normalize(-forward);
        //    Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector3_1));
        //    Vector3 vector3_2 = Vector3.Cross(vector3_1, vector2);
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = vector2.X;
        //    matrix.M12 = vector2.Y;
        //    matrix.M13 = vector2.Z;
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = vector3_2.X;
        //    matrix.M22 = vector3_2.Y;
        //    matrix.M23 = vector3_2.Z;
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = vector3_1.X;
        //    matrix.M32 = vector3_1.Y;
        //    matrix.M33 = vector3_1.Z;
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = position.X;
        //    matrix.M42 = position.Y;
        //    matrix.M43 = position.Z;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateFromQuaternion(Quaternion quaternion)
        //{
        //    float num1 = quaternion.X * quaternion.X;
        //    float num2 = quaternion.Y * quaternion.Y;
        //    float num3 = quaternion.Z * quaternion.Z;
        //    float num4 = quaternion.X * quaternion.Y;
        //    float num5 = quaternion.Z * quaternion.W;
        //    float num6 = quaternion.Z * quaternion.X;
        //    float num7 = quaternion.Y * quaternion.W;
        //    float num8 = quaternion.Y * quaternion.Z;
        //    float num9 = quaternion.X * quaternion.W;
        //    Matrix matrix = new Matrix();
        //    matrix.M11 = (float)(1.0 - 2.0 * ((double)num2 + (double)num3));
        //    matrix.M12 = (float)(2.0 * ((double)num4 + (double)num5));
        //    matrix.M13 = (float)(2.0 * ((double)num6 - (double)num7));
        //    matrix.M14 = 0.0f;
        //    matrix.M21 = (float)(2.0 * ((double)num4 - (double)num5));
        //    matrix.M22 = (float)(1.0 - 2.0 * ((double)num3 + (double)num1));
        //    matrix.M23 = (float)(2.0 * ((double)num8 + (double)num9));
        //    matrix.M24 = 0.0f;
        //    matrix.M31 = (float)(2.0 * ((double)num6 + (double)num7));
        //    matrix.M32 = (float)(2.0 * ((double)num8 - (double)num9));
        //    matrix.M33 = (float)(1.0 - 2.0 * ((double)num2 + (double)num1));
        //    matrix.M34 = 0.0f;
        //    matrix.M41 = 0.0f;
        //    matrix.M42 = 0.0f;
        //    matrix.M43 = 0.0f;
        //    matrix.M44 = 1f;
        //    return matrix;
        //}

        //public static Matrix CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        //{
        //    var q = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
        //    return Matrix.CreateFromQuaternion(q);
        //}

        //public static Matrix CreateShadow(Vector3 lightDirection, Plane plane)
        //{
        //    Plane result;
        //    Plane.Normalize(ref plane, out result);
        //    float num1 = (float)((double)result.Normal.X * (double)lightDirection.X + (double)result.Normal.Y * (double)lightDirection.Y + (double)result.Normal.Z * (double)lightDirection.Z);
        //    float num2 = -result.Normal.X;
        //    float num3 = -result.Normal.Y;
        //    float num4 = -result.Normal.Z;
        //    float num5 = -result.D;
        //    Matrix matrix;
        //    matrix.M11 = num2 * lightDirection.X + num1;
        //    matrix.M21 = num3 * lightDirection.X;
        //    matrix.M31 = num4 * lightDirection.X;
        //    matrix.M41 = num5 * lightDirection.X;
        //    matrix.M12 = num2 * lightDirection.Y;
        //    matrix.M22 = num3 * lightDirection.Y + num1;
        //    matrix.M32 = num4 * lightDirection.Y;
        //    matrix.M42 = num5 * lightDirection.Y;
        //    matrix.M13 = num2 * lightDirection.Z;
        //    matrix.M23 = num3 * lightDirection.Z;
        //    matrix.M33 = num4 * lightDirection.Z + num1;
        //    matrix.M43 = num5 * lightDirection.Z;
        //    matrix.M14 = 0.0f;
        //    matrix.M24 = 0.0f;
        //    matrix.M34 = 0.0f;
        //    matrix.M44 = num1;
        //    return matrix;
        //}
    }
}