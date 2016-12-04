using System;
namespace NetGL.Core.Mathematics {
    public class BoundingVolume {
        public Vector3 Center { get; private set; }
        public Vector3 Size { get; private set; }
        public float Radius { get; private set; }

        public Sphere Sphere {
            get { return new Sphere() { Center = Center, Radius = Radius }; }
        }

        public Box AxisAlignedBox {
            get { return new Box() { Center = Center, Size = Size }; }
        }

        public BoundingVolume(Vector3 center, Vector3 size, float radius) {
            this.Center = center;
            this.Size = size;
            this.Radius = radius;
        }
    }
}