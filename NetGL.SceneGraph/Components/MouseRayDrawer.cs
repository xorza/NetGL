using System.Runtime.InteropServices;
using System.Windows.Forms;
using NetGL.Core;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Meshes;
using NetGL.Core.Shaders;
using NetGL.Core.Types;
using NetGL.SceneGraph.Control;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Components {
    [Guid("360C0A77-0630-491A-8973-35070FC7C0AE")]
    public class MouseRayDrawer : Component {
        private INetGLControl _control;
        private MeshRenderable _renderer, _hitPointRenderer;
        private Material _selectedMaterial;

        private Transform _hitPointTransform;

        public MouseRayDrawer(Node owner) : base(owner) { }

        protected override void OnStart() {
            _control = Scene.Control;

            _control.MouseDown += Control_MouseDown;

            var material = new Material(MaterialType.FlatColor);
            material.Color = new Vector4(1, 1, 0, 1);
            _renderer = SceneObject.AddComponent<MeshRenderable>();
            _renderer.Material = material;
            _renderer.IsEnabled = false;

            var mesh = new Mesh();
            var buffer3 = new Vector3Buffer(2);
            buffer3.Usage = BufferUsage.DynamicDraw;
            mesh.Vertices = buffer3;
            mesh.Type = PrimitiveType.Lines;

            _renderer.Mesh = mesh;

            var hitPointSo = new Node(Scene);
            var sphere = Icosahedron.Create(0.01f, 1);
            _hitPointRenderer = hitPointSo.AddComponent<MeshRenderable>();
            _hitPointRenderer.IsEnabled = false;
            _hitPointRenderer.Mesh = sphere;
            _hitPointRenderer.SceneObject.Layer = 2;
            _hitPointRenderer.Material = material;

            _hitPointTransform = hitPointSo.Transform;

            _selectedMaterial = new Material(MaterialType.DiffuseColor);
            _selectedMaterial.Color = new Vector4(1, 0, 0, 1);
        }
        public void Update() {
            var ray = Scene.MainCamera.Unproject(_control.ScreenMousePosition);

            _renderer.Mesh.Vertices[0] = ray.Origin;
            _renderer.Mesh.Vertices[1] = ray.Direction + ray.Origin;
            _renderer.UpdateMesh();

            ray.Direction.Normalize();

            var hit = SceneObject.Scene.RaycastMeshes(ray);
            if (hit == null)
                return;

            if (hit.Renderer.Material != null)
                hit.Renderer.Material = _selectedMaterial;

            _hitPointTransform.LocalPosition = hit.Point;
            _renderer.IsEnabled = true;
            _hitPointRenderer.IsEnabled = true;
        }

        private void Control_MouseDown(object sender, MouseEventArgs e) {
            if (!IsEnabled)
                return;

            if ((_control.PressedMouseButtons & MouseButtons.Left) != MouseButtons.Left)
                return;

            Update();
        }

        protected override void OnDispose() {
            base.OnDispose();

            Disposer.Dispose(ref _selectedMaterial);
            Disposer.Dispose(_renderer.Material);

            if (_control != null)
                _control.MouseDown -= Control_MouseDown;
        }
    }
}