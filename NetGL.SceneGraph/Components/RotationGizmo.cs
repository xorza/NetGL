using System;
using System.Windows.Forms;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Meshes;
using NetGL.Core.Shaders;
using NetGL.Core.Types;
using NetGL.SceneGraph.Scene;
using NetGL.SceneGraph.Serialization;
using Plane = NetGL.Core.Mathematics.Plane;
using Sphere = NetGL.Core.Mathematics.Sphere;
using NetGL.Core;
using NetGL.SceneGraph.OpenGL;

namespace NetGL.SceneGraph.Components {
    [NotSerialized]
    public class RotationGizmo : Component {
        private const float MinDistance = 0.025f;
        private const int CircleSegmentCount = 40;
        private const float Radius = 0.5f;

        private Material _material;
        private MeshRenderable _renderer;
        private Plane _xPlane, _yPlane, _zPlane;
        private Sphere _sphere;
        private Axis _selectedAxis = Axis.None, _pressedAxis = Axis.None;

        private Vector2 _screenTangent;

        private Vector3 _startDragPositionMS;
        private Quaternion _startRotation;
        private readonly Matrix _invertedMatrix = new Matrix();

        public RotationGizmo(Node owner) : base(owner) { }

        protected override void OnInit() {
            base.OnInit();

            _material = new Material(MaterialType.FlatVertexColor);

            var frag = @"
void getFragment(inout fragment_info fragment) {
    fragment.normal = frag_in.normal;

	if(dot(normalize(fragment.normal), normalize(-frag_in.view_pos)) > -0.25)
		fragment.emission = frag_in.color.xyz;
	else
		fragment.emission = vec3(0.75, 0.75, 0.75);
    fragment.albedo = vec4(0.0, 0.0, 0.0, 1.0);
}
";

            _renderer = SceneObject.AddComponent<MeshRenderable>();
            _renderer.Material = new Material(null, frag, RenderQueue.Opaque);

            var circle = Ellipse.Create(Quaternion.Identity, Vector2.One * Radius, CircleSegmentCount);

            var meshBuilder = new MeshBuilder();
            meshBuilder.AddMesh(circle);
            meshBuilder.AddMesh(circle, Quaternion.CreateFromYawPitchRoll(0, MathF.PIOver2, 0));
            meshBuilder.AddMesh(circle, Quaternion.CreateFromYawPitchRoll(MathF.PIOver2, 0, 0));

            var mesh = meshBuilder.GetMesh();
            mesh.DrawStyle = PolygonMode.Line;
            mesh.Type = PrimitiveType.Lines;
            mesh.Colors = new Vector3Buffer(CircleSegmentCount * 3);
            _renderer.Mesh = mesh;

            RestoreColors();

            _sphere = new Sphere(Vector3.Zero, Radius);
            _xPlane = new Plane(new Vector3(1, 0, 0), 0);
            _yPlane = new Plane(new Vector3(0, 1, 0), 0);
            _zPlane = new Plane(new Vector3(0, 0, 1), 0);
        }
        protected override void OnStart() {
            base.OnStart();

            Scene.Control.MouseDown += Control_MouseDown;
            Scene.Control.MouseMove += Control_MouseMove;
            Scene.Control.MouseUp += Control_MouseUp;
        }
        protected override void OnDispose() {
            base.OnDispose();

            Scene.Control.MouseDown -= Control_MouseDown;
            Scene.Control.MouseMove -= Control_MouseMove;
            Scene.Control.MouseUp -= Control_MouseUp;

            Disposer.Dispose(ref _material);
            Disposer.Dispose(_renderer.Material);
            Disposer.Dispose(ref _renderer);
        }

        private void Control_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left)
                return;
            if (_selectedAxis == Axis.None)
                return;

            _pressedAxis = _selectedAxis;
            StartDrag();
        }
        private void Control_MouseMove(object sender, MouseEventArgs e) {
            if (_pressedAxis == Axis.None)
                Select();
            else
                Drag();
        }
        private void Control_MouseUp(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left) {
                Unhighlight();
                _selectedAxis = Axis.None;
                _pressedAxis = Axis.None;
            }
        }

        private RaycastHit RaycastAxisPlane(Ray ray, Plane plane) {
            var hit = plane.Raycast(ray);
            if (hit == null)
                return null;

            var distanceToCenter = hit.Point.Length;
            if (distanceToCenter > Radius - MinDistance && distanceToCenter < Radius + MinDistance)
                return hit;
            else
                return null;
        }

        private void RestoreColors() {
            var mesh = _renderer.Mesh;

            for (int i = 0; i < CircleSegmentCount; i++)
                mesh.Colors[i] = new Vector3(0, 0, 1);
            for (int i = CircleSegmentCount; i < 2 * CircleSegmentCount; i++)
                mesh.Colors[i] = new Vector3(0, 1, 0);
            for (int i = 2 * CircleSegmentCount; i < 3 * CircleSegmentCount; i++)
                mesh.Colors[i] = new Vector3(1, 0, 0);
        }
        private void Select() {
            var ray = Scene.MainCamera.Unproject(Scene.Control.ScreenMousePosition);
            ray = Ray.Transform(ray, Transform.InvertedModelMatrix);

            var closestHit = new RaycastHit() { Distance = float.MaxValue };
            var axis = Axis.None;

            var sphereHit = _sphere.Raycast(ray);
            if (sphereHit != null) {
                if (Math.Abs(sphereHit.Point.X) < MinDistance) {
                    if (closestHit.Distance > sphereHit.Distance) {
                        axis = Axis.X;
                        closestHit = sphereHit;
                    }
                }
                if (Math.Abs(sphereHit.Point.Y) < MinDistance) {
                    if (closestHit.Distance > sphereHit.Distance) {
                        axis = Axis.Y;
                        closestHit = sphereHit;
                    }
                }
                if (Math.Abs(sphereHit.Point.Z) < MinDistance) {
                    if (closestHit.Distance > sphereHit.Distance) {
                        axis = Axis.Z;
                        closestHit = sphereHit;
                    }
                }
            }

            RaycastHit planeHit;
            if ((planeHit = RaycastAxisPlane(ray, _xPlane)) != null) {
                if (closestHit.Distance > planeHit.Distance) {
                    axis = Axis.X;
                    closestHit = planeHit;
                }
            }
            if ((planeHit = RaycastAxisPlane(ray, _yPlane)) != null) {
                if (closestHit.Distance > planeHit.Distance) {
                    axis = Axis.Y;
                    closestHit = planeHit;
                }
            }
            if ((planeHit = RaycastAxisPlane(ray, _zPlane)) != null) {
                if (closestHit.Distance > planeHit.Distance) {
                    axis = Axis.Z;
                    closestHit = planeHit;
                }
            }

            if (axis == Axis.None && sphereHit != null) {
                axis = Axis.All;
                closestHit = sphereHit;
            }

            if (axis != Axis.None) {
                _startDragPositionMS = closestHit.Point;

                switch (axis) {
                    case Axis.X:
                        _startDragPositionMS.X = 0;
                        break;
                    case Axis.Y:
                        _startDragPositionMS.Y = 0;
                        break;
                    case Axis.Z:
                        _startDragPositionMS.Z = 0;
                        break;
                    case Axis.All:
                        break;

                    default:
                        throw new NotImplementedException();
                }
                _startDragPositionMS.Normalize();

                if (_selectedAxis == axis)
                    return;

                Unhighlight();
                _selectedAxis = axis;
                Highlight();
            }
            else {
                if (_selectedAxis == Axis.None)
                    return;

                Unhighlight();
                _selectedAxis = Axis.None;
                _startDragPositionMS = Vector3.Zero;
            }
        }
        private void StartDrag() {
            switch (_pressedAxis) {
                case Axis.X:
                case Axis.Y:
                case Axis.Z:
                    var tangentMS = Vector3.Cross(_startDragPositionMS, GetAxis(_pressedAxis));

                    var startDragPositionWS = Transform.TransformVector(_startDragPositionMS);
                    var tangentWS = Vector3.NormalTransform(tangentMS, Transform.ModelMatrix);

                    var hitScreen = new Vector2(Scene.MainCamera.WorldToScreen(startDragPositionWS));
                    _screenTangent = new Vector2(Scene.MainCamera.WorldToScreen(startDragPositionWS + tangentWS)) - hitScreen;
                    _screenTangent.Normalize();

                    return;
                case Axis.All:
                    _startRotation = Transform.WorldRotation;
                    _invertedMatrix.Load(Transform.InvertedModelMatrix);
                    return;
                default:
                    throw new NotImplementedException();
            }
        }
        private void Drag() {
            switch (_pressedAxis) {
                case Axis.X:
                case Axis.Y:
                case Axis.Z: var drag = Scene.Control.ScreenMousePositionDelta;
                    var coeff = Vector2.Dot(drag.Normalized, _screenTangent);
                    var angle = drag.Length * coeff * 0.003f;

                    var axis = GetAxis(_pressedAxis);
                    Transform.WorldRotation *= new Quaternion(axis, angle);
                    return;

                case Axis.All:
                    var ray = Scene.MainCamera.Unproject(Scene.Control.ScreenMousePosition);
                    ray = Ray.Transform(ray, _invertedMatrix);

                    var hit = _sphere.Raycast(ray);
                    if (hit == null)
                        return;

                    var rotation = Quaternion.FromToRotation(_startDragPositionMS, hit.Point.Normalized);
                    Transform.WorldRotation = _startRotation * rotation;
                    return;

                default:
                    throw new NotImplementedException();
            }
        }

        private void Highlight() {
            switch (_selectedAxis) {
                case Axis.X:
                    for (int i = 80; i < 120; i++)
                        _renderer.Mesh.Colors[i] = new Vector3(1, 1, 0);
                    break;
                case Axis.Y:
                    for (int i = 40; i < 80; i++)
                        _renderer.Mesh.Colors[i] = new Vector3(1, 1, 0);
                    break;
                case Axis.Z:
                    for (int i = 0; i < 40; i++)
                        _renderer.Mesh.Colors[i] = new Vector3(1, 1, 0);
                    break;
                case Axis.All:
                    for (int i = 0; i < 120; i++)
                        _renderer.Mesh.Colors[i] = new Vector3(1, 1, 0);
                    break;

                case Axis.None:
                    return;
                default:
                    throw new NotImplementedException();
            }

            _renderer.UpdateMesh();
        }
        private void Unhighlight() {
            switch (_selectedAxis) {
                case Axis.X:
                case Axis.Y:
                case Axis.Z:
                case Axis.All:
                    RestoreColors();
                    break;

                case Axis.None:
                    return;
                default:
                    throw new NotImplementedException();
            }

            _renderer.UpdateMesh();
        }
        private Vector3 GetAxis(Axis axis) {
            switch (axis) {
                case Axis.X:
                    return new Vector3(1, 0, 0);
                case Axis.Y:
                    return new Vector3(0, 1, 0);
                case Axis.Z:
                    return new Vector3(0, 0, 1);
                default:
                    throw new NotImplementedException();
            }
        }

        internal enum Axis {
            None,
            X, Y, Z,
            All
        }
    }
}
