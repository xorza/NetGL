using System;
using System.Windows.Forms;
using NetGL.Core.Infrastructure;
using NetGL.SceneGraph.Control;
using NetGL.SceneGraph.Scene;
using NetGL.SceneGraph.Serialization;

namespace NetGL.SceneGraph.Components {
    [NotSerialized]
    public class ObjectSelector : Component {
        private INetGLControl _control;
        private Camera _camera;
        private BoundingBox _boundingBox;

        public event Action<MeshRenderable> Selected;

        public ObjectSelector(Node owner) : base(owner) { }

        protected override void OnInit() {
            base.OnInit();

            _boundingBox = SceneObject.AddComponent<BoundingBox>();
        }
        protected override void OnStart() {
            base.OnStart();

            _control = Scene.Control;
            _camera = Scene.MainCamera;

            _control.MouseDown += Control_MouseDown;
        }

        public void Select(Node sceneObject) {
            if (IsDisposed) {
                Log.Error("object disposed");
                return;
            }

            MeshRenderable renderer = null;
            if (sceneObject != null)
                renderer = sceneObject.GetComponent<MeshRenderable>();

            Select(renderer);
        }
        public void Select(MeshRenderable renderer) {
            _boundingBox.SetTarget(renderer);

            if (Selected != null)
                Selected(renderer);
        }

        private void Control_MouseDown(object sender, MouseEventArgs e) {
            if (_boundingBox == null)
                return;

            if ((_control.PressedMouseButtons & MouseButtons.Left) != MouseButtons.Left)
                return;

            var ray = _camera.Unproject(_control.ScreenMousePosition);
            var hit = _control.Scene.RaycastMeshes(ray);
            MeshRenderable renderer = null;
            if (hit != null)
                renderer = hit.Renderer;
            Select(renderer);
        }

        protected override void OnDispose() {
            _control.MouseDown -= Control_MouseDown;
            base.OnDispose();
        }
    }
}
