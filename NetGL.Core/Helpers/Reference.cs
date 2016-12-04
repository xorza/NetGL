using System;

namespace NetGL.Core.Helpers {
    public sealed class Reference<T> where T : struct {
        public T Value;

        public Reference(T value) {
            Value = value;
        }
        public Reference() {
            Value = default(T);
        }

        //public static implicit operator Reference<T>(T value)
        //{
        //    return new Reference<T>(value);
        //}
        public static implicit operator T(Reference<T> reference) {
            Assert.NotNull(reference);

            return reference.Value;
        }

        public static bool operator ==(Reference<T> reference, T value) {
            if (reference == null)
                return false;

            return reference.Value.Equals(value);
        }
        public static bool operator !=(Reference<T> reference, T value) {
            if (reference == null)
                return true;

            return !reference.Value.Equals(value);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
        public override bool Equals(object obj) {
            return ReferenceEquals(this, obj);
        }
    }
}
