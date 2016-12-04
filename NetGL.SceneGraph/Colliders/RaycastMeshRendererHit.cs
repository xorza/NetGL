using System;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Components;

namespace NetGL.SceneGraph.Colliders {
    public class RaycastMeshRendererHit : IComparable<RaycastMeshRendererHit> {
        public MeshRenderable Renderer { get; set; }
        public Vector3 Point { get; set; }
        public float Distance { get; set; }

        public int CompareTo(RaycastMeshRendererHit other) {
            Assert.NotNull(other);

            return Distance.CompareTo(other.Distance);
        }
    }
}