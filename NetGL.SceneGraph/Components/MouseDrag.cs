using System.Windows.Forms;
using NetGL.Core.Mathematics;
using NetGL.Core.Meshes;
using NetGL.Core.Shaders;
using NetGL.SceneGraph.Colliders;
using NetGL.SceneGraph.Scene;
using NetGL.Core.Infrastructure;

namespace NetGL.SceneGraph.Components {
    public class MouseDrag : Component, IUpdatable {
        private SphereCollider _collider;
        private MeshRenderable _pickerRenderer;
        private ObjectSelector _selector;

        private Transform _target = null;

        private Vector2 _mousePosition;
        private bool _wasPressed = false, _isDragging = false;
        private Vector3 _screenPoint;
        private Vector3 _offset;
        private Ray _ray;


        public MouseDrag(Node owner) : base(owner) { }

        protected override void OnStart() {
            base.OnStart();

            _collider = this.SceneObject.AddComponent<SphereCollider>();
            _collider.Radius = 0.1f;

            _pickerRenderer = this.SceneObject.AddComponent<MeshRenderable>();
            _pickerRenderer.Mesh = Icosahedron.Create(0.02f, 1);
            _pickerRenderer.IsEnabled = false;
            _pickerRenderer.Material = new Material(MaterialType.FlatColor);
            _pickerRenderer.Material.Color = new Vector4(1, 1, 0, 1);
            _pickerRenderer.IsRaytracable = false;

            _selector = SceneObject.GetComponent<ObjectSelector>();
            if (_selector != null)
                _selector.Selected += ObjectSelected;
        }
        protected override void OnDispose() {
            base.OnDispose();
            Disposer.Dispose(_pickerRenderer.Material);
        }

        private void ObjectSelected(MeshRenderable renderer) {
            if (renderer == null) {
                _target = null;
                _pickerRenderer.IsEnabled = false;
            }
            else {
                _target = renderer.Transform;
                this.Transform.WorldPosition = _target.WorldPosition;
                _pickerRenderer.IsEnabled = true;
            }
        }

        void IUpdatable.Update() {
            if (SceneObject.Scene.MainCamera == null)
                return;

            var pressed = (Scene.Control.PressedMouseButtons & MouseButtons.Left) == MouseButtons.Left;
            var deltaPos = Scene.Control.ScreenMousePosition - _mousePosition;
            _mousePosition = Scene.Control.ScreenMousePosition;
            _ray = Scene.MainCamera.Unproject(Scene.Control.ScreenMousePosition);

            if (!pressed) {
                _wasPressed = false;
                _isDragging = false;
                return;
            }

            if (_wasPressed) {
                if (deltaPos.LengthSquared > 0)
                    OnMouseMove();
            }
            else {
                _wasPressed = true;
                OnMouseDown();
            }
        }

        private void OnMouseDown() {
            if (_target == null)
                return;

            if (_collider.Raycast(_ray) == null)
                return;

            _isDragging = true;
            _screenPoint = Scene.MainCamera.WorldToScreen(Transform.WorldPosition);
            _offset = Transform.WorldPosition - Scene.MainCamera.ScreenToWorld(new Vector3(_mousePosition.X, _mousePosition.Y, _screenPoint.Z));
        }
        private void OnMouseMove() {
            if (_target == null)
                return;
            if (_isDragging == false)
                return;

            var curScreenPoint = new Vector3(_mousePosition.X, _mousePosition.Y, _screenPoint.Z);

            var position = Scene.MainCamera.ScreenToWorld(curScreenPoint) + _offset;
            Transform.WorldPosition = position;

            _target.WorldPosition = position;
        }

        private void PickTarget() {
            var picked = Scene.RaycastMeshes(_ray);
            if (picked == null)
                return;

            _target = picked.Renderer.Transform;
            this.Transform.WorldPosition = _target.WorldPosition;
            _pickerRenderer.IsEnabled = true;
        }
    }
}
