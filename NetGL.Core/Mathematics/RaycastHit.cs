
using System;

namespace NetGL.Core.Mathematics {
    public class RaycastHit : IComparable<RaycastHit> {
        public Vector3 Point;
        public float Distance;

        public int CompareTo(RaycastHit other) {
            return this.Distance.CompareTo(other.Distance);
        }
    }
}
