
using System;
namespace NetGL.Core.Types {
    public struct GLVersion : IComparable<GLVersion> {
        public int Major { get; private set; }
        public int Minor { get; private set; }

        internal GLVersion(int major, int minor)
            : this() {
            if (major <= 0)
                throw new ArgumentException("major");
            if (minor < 0)
                throw new ArgumentException("minor");

            Major = major;
            Minor = minor;
        }

        public override string ToString() {
            return string.Format("{0}.{1}", Major, Minor);
        }

        public int CompareTo(GLVersion other) {
            var i = Major.CompareTo(other.Major);
            if (i != 0)
                return i;

            return Minor.CompareTo(other.Minor);
        }

        public override int GetHashCode() {
            return Major ^ Minor; ;
        }
        public override bool Equals(object obj) {
            if (obj == null)
                throw new ArgumentNullException("obj");
            return this == ((GLVersion)obj);
        }

        public static bool operator ==(GLVersion v1, GLVersion v2) {
            return v1.Major == v2.Major && v1.Minor == v2.Minor;
        }
        public static bool operator !=(GLVersion v1, GLVersion v2) {
            return v1.Major != v2.Major || v1.Minor != v2.Minor;
        }
        public static bool operator >(GLVersion v1, GLVersion v2) {
            if (v1.Major == v2.Major)
                return v1.Minor > v2.Minor;
            else
                return v1.Major > v2.Major;
        }
        public static bool operator >=(GLVersion v1, GLVersion v2) {
            if (v1.Major == v2.Major)
                return v1.Minor >= v2.Minor;
            else
                return v1.Major >= v2.Major;
        }
        public static bool operator <(GLVersion v1, GLVersion v2) {
            if (v1.Major == v2.Major)
                return v1.Minor < v2.Minor;
            else
                return v1.Major < v2.Major;
        }
        public static bool operator <=(GLVersion v1, GLVersion v2) {
            if (v1.Major == v2.Major)
                return v1.Minor <= v2.Minor;
            else
                return v1.Major <= v2.Major;
        }
    }
}
