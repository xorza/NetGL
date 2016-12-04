using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Colliders {
    public abstract class Collider : Component {
        private readonly Ray _modelSpaceRay = new Ray();

        public abstract RaycastColliderHit Raycast(Ray ray);

        public Collider(Node owner) : base(owner) { }

        protected Ray GetModelSpaceRay(Ray worldSpaceRay) {
            var matrix = Transform.InvertedModelMatrix;

            _modelSpaceRay.Origin = Vector3
                .Transform(worldSpaceRay.Origin, matrix);
            _modelSpaceRay.Direction = Vector3
                .NormalTransform(worldSpaceRay.Direction, matrix)
                .Normalized;

            return _modelSpaceRay;
        }
    }
}