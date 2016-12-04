using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Colliders {
    public class SphereCollider : Collider {
        private readonly Sphere _sphere = new Sphere();

        public float Radius { get; set; }

        //private SceneObject _picker;
        //protected override void OnStart() {
        //    this.SceneObject.Scene.Control.MouseDown += Control_MouseDown;

        //    _picker = new SceneObject(this.SceneObject.Scene, "picker");

        //    var renderer = _picker.AddComponent<MeshRenderer>();
        //    renderer.Mesh = Icosahedron.Create(0.01f, 1);
        //}
        //private void Control_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
        //    var ray = SceneObject.Scene.MainCamera.Unproject(SceneObject.Scene.Control.ScreenMousePosition, SceneObject.Scene.Control.ClientSize);
        //    var hit = Raycast(ray);
        //    if (hit == null)
        //        return;

        //    _picker.Transform.LocalPosition = hit.Point;
        //}

        public SphereCollider(Node owner) : base(owner) { }

        public override RaycastColliderHit Raycast(Ray ray) {
            _sphere.Center = Vector3.Zero;
            _sphere.Radius = Radius;

            var modelSpaceRay = GetModelSpaceRay(ray);
            var hit = _sphere.Raycast(modelSpaceRay);
            if (hit != null) {
                var raycastHit = new RaycastColliderHit();
                raycastHit.Collider = this;
                raycastHit.Point = Vector3.Transform(hit.Point, Transform.ModelMatrix);
                raycastHit.Distance = Vector3.Distance(ray.Origin, raycastHit.Point);
                return raycastHit;
            }
            return null;
        }
    }
}