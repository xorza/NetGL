using System.Windows.Forms;
using NetGL.Core.Mathematics;
using NetGL.Core.Meshes;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Colliders {
    public class CylinderCollider : Collider {
        private readonly Cylinder _cylinder = new Cylinder();

        public Vector3 Offset { get; set; }
        public float Radius { get; set; }
        public float Height { get; set; }

        public CylinderCollider(Node owner)
            : base(owner) { 
            Radius = 0.5f;
            Height = 1;
            Offset = new Vector3(0, -0.5f, 0);
        }

        public override RaycastColliderHit Raycast(Ray ray) {
            _cylinder.Base = Offset;
            _cylinder.Radius = Radius;
            _cylinder.Height = Height;

            var modelSpaceRay = GetModelSpaceRay(ray);

            var hit = _cylinder.Raycast(modelSpaceRay);
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
        //    Scene.Control.MouseDown += Control_MouseDown;

        //    _picker = new SceneObject(Scene, "picker");

        //    var renderer = _picker.AddComponent<Renderer>();
        //    renderer.Mesh = Icosahedron.Create(0.01f, 1);
        //}
        //private void Control_MouseDown(object sender, MouseEventArgs e) {
        //    var ray = Scene.MainCamera.Unproject(Scene.Control.ScreenMousePosition);
        //    var hit = Raycast(ray);
        //    if (hit == null)
        //        return;

        //    _picker.Transform.LocalPosition = hit.Point;
        //}
    }
}
