using System;
using NetGL.Core.Mathematics;

namespace NetGL.SceneGraph.Colliders {
    public class RaycastColliderHit : IComparable<RaycastColliderHit> {
        public Collider Collider { get; set; }
        public Vector3 Point { get; set; }
        public float Distance { get; set; }

        public int CompareTo(RaycastColliderHit other) {
            Assert.NotNull(other);

            return Distance.CompareTo(other.Distance);
        }
    }
}