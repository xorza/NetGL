using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Colliders {
    public class BoxCollider : Collider {
        private readonly Box _box = new Box();

        public Vector3 Size { get; set; }
        public Vector3 Offset { get; set; }

        public BoxCollider(Node owner)
            : base(owner) {
            Size = Vector3.One;
        }

        public override RaycastColliderHit Raycast(Ray ray) {
            _box.Center = Offset;
            _box.Size = Size;

            var modelSpaceRay = GetModelSpaceRay(ray);

            var hit = _box.Raycast(modelSpaceRay);
            if (hit != null) {
                var raycastHit = new RaycastColliderHit();
                raycastHit.Collider = this;
                raycastHit.Point = Vector3.Transform(hit.Point, Transform.ModelMatrix);
                raycastHit.Distance = Vector3.Distance(ray.Origin, raycastHit.Point);
                return raycastHit;
            }
            return null;
        }


        //private SceneObject _picker;
        //protected override void OnStart() {
        //    this.SceneObject.Scene.Control.MouseMove += Control_MouseDown;

        //    _picker = new SceneObject(this.SceneObject.Scene, "picker");

        //    var renderer = _picker.AddComponent<MeshRenderer>();
        //    renderer.Mesh = Icosahedron.Create(0.01f, 1);
        //}
        //private void Control_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
        //    var ray = SceneObject.Scene.MainCamera.Unproject(SceneObject.Scene.Control.ScreenMousePosition);
        //    var hit = Raycast(ray);
        //    if (hit == null)
        //        return;

        //    _picker.Transform.LocalPosition = hit.Point;
        //}
    }
}
