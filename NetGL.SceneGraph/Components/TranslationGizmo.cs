using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetGL.Core;
using NetGL.Core.Mathematics;
using NetGL.Core.Meshes;
using NetGL.Core.Shaders;
using NetGL.Core.Types;
using NetGL.SceneGraph.Scene;
using NetGL.SceneGraph.Serialization;
using Box = NetGL.Core.Mathematics.Box;
using Plane = NetGL.Core.Mathematics.Plane;

namespace NetGL.SceneGraph.Components {
    [NotSerialized]
    public class TranslationGizmo : Component {
        private const float CrossMid = 0.4f;
        private const float CrossSize = 0.15f;
        private const float ColliderWidth = 0.03f;

        private MeshRenderable _coneRenderer, _lineRenderer;
        private Material _material;
        private Box _xBox, _yBox, _zBox, _xyBox, _xzBox, _yzBox;
        private Axis? _selectedAxis = null, _pressedAxis = null;
        private Plane _dragPlane;
        private Vector3 _moveOffset;
        
        public TranslationGizmo(Node owner) : base(owner) { }

        protected override void OnInit() {
            base.OnInit();

            _material = new Material(MaterialType.FlatVertexColor);

            AddCones();
            AddLines();
            AddColliders();
        }
        protected override void OnStart() {
            base.OnStart();

            Scene.Control.MouseMove += Control_MouseMove;
            Scene.Control.MouseDown += Control_MouseDown;
            Scene.Control.MouseUp += Control_MouseUp;
        }
        protected override void OnDispose() {
            base.OnDispose();

            Scene.Control.MouseMove -= Control_MouseMove;
            Scene.Control.MouseDown -= Control_MouseDown;
            Scene.Control.MouseUp -= Control_MouseUp;
            _coneRenderer.Dispose();
            _lineRenderer.Dispose();
            _material.Dispose();
        }

        private void Control_MouseUp(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left) {
                Unhighlight();
                _selectedAxis = null;
                _pressedAxis = null;
                _dragPlane = null;
            }
        }
        private void Control_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left)
                return;
            if (_selectedAxis == null)
                return;

            _pressedAxis = _selectedAxis;
            StartMove();
        }
        private void Control_MouseMove(object sender, MouseEventArgs e) {
            if (_pressedAxis != null)
                Move();
            else
                Select();
        }

        private void AddLines() {
            _lineRenderer = SceneObject.AddComponent<MeshRenderable>();
            _lineRenderer.Material = _material;

            var mesh = new Mesh();
            mesh.Type = PrimitiveType.Lines;
            mesh.Vertices = new Vector3[]{
                new Vector3(0,0,0),
                new Vector3(1,0,0), //X

                new Vector3(0,0,0),
                new Vector3(0,1,0), //Y

                new Vector3(0,0,0),
                new Vector3(0,0,1),  //Z

                new Vector3(CrossMid, CrossMid - CrossSize, 0),
                new Vector3(CrossMid, CrossMid, 0),
                new Vector3(CrossMid - CrossSize, CrossMid, 0),
                new Vector3(CrossMid, CrossMid, 0), //XY

                new Vector3(CrossMid, 0, CrossMid - CrossSize),
                new Vector3(CrossMid, 0, CrossMid),
                new Vector3(CrossMid - CrossSize, 0, CrossMid),
                new Vector3(CrossMid, 0, CrossMid), //XZ

                new Vector3(0, CrossMid, CrossMid - CrossSize),
                new Vector3(0, CrossMid, CrossMid),
                new Vector3(0, CrossMid - CrossSize, CrossMid),
                new Vector3(0, CrossMid, CrossMid), //YZ
            };
            mesh.Colors = new Vector3[]{
                new Vector3(1,0,0),
                new Vector3(1,0,0), //X

                new Vector3(0,1,0),
                new Vector3(0,1,0), //Y

                new Vector3(0,0,1),
                new Vector3(0,0,1), //Z

                new Vector3(1,0,0),
                new Vector3(1,0,0),
                new Vector3(0,1,0),
                new Vector3(0,1,0), //XY

                new Vector3(1,0,0),
                new Vector3(1,0,0),
                new Vector3(0,0,1),
                new Vector3(0,0,1), //XZ

                new Vector3(0,1,0),
                new Vector3(0,1,0),
                new Vector3(0,0,1),
                new Vector3(0,0,1), //YZ
            };

            mesh.CalculateBounds();
            _lineRenderer.Mesh = mesh;
        }
        private void AddCones() {
            const float ConeWidth = 0.05f;
            const float ConeLength = 0.20f;

            _coneRenderer = SceneObject.AddComponent<MeshRenderable>();
            _coneRenderer.Material = _material;

            var xCone = Cone.Create(ConeWidth, ConeLength, 6);
            xCone.Transform(new Matrix(new Vector3(1 - ConeLength, 0, 0), new Quaternion(Vector3.Forward, MathF.PI / 2), Vector3.One));
            xCone.Colors = new Vector3Buffer(xCone.Vertices.Length, new Vector3(1, 0, 0));

            var yCone = Cone.Create(ConeWidth, ConeLength, 6);
            yCone.Transform(new Matrix(new Vector3(0, 1 - ConeLength, 0), Quaternion.Identity, Vector3.One));
            yCone.Colors = new Vector3Buffer(yCone.Vertices.Length, new Vector3(0, 1, 0));

            var zCone = Cone.Create(ConeWidth, ConeLength, 6);
            zCone.Transform(new Matrix(new Vector3(0, 0, 1 - ConeLength), new Quaternion(Vector3.Right, MathF.PI / 2), Vector3.One));
            zCone.Colors = new Vector3Buffer(zCone.Vertices.Length, new Vector3(0, 0, 1));

            var mesh = CompoundMesh.Create(xCone, yCone);
            mesh = CompoundMesh.Create(mesh, zCone);

            mesh.CalculateBounds();
            _coneRenderer.Mesh = mesh;
        }
        private void AddColliders() {
            _xBox = new Box(new Vector3(0.5f, 0, 0), new Vector3(1, ColliderWidth, ColliderWidth));
            _yBox = new Box(new Vector3(0, 0.5f, 0), new Vector3(ColliderWidth, 1, ColliderWidth));
            _zBox = new Box(new Vector3(0, 0, 0.5f), new Vector3(ColliderWidth, ColliderWidth, 1));

            var offset = CrossMid - CrossSize / 2;
            _xyBox = new Box(new Vector3(offset, offset, 0), new Vector3(CrossSize, CrossSize, 0));
            _xzBox = new Box(new Vector3(offset, 0, offset), new Vector3(CrossSize, 0, CrossSize));
            _yzBox = new Box(new Vector3(0, offset, offset), new Vector3(0, CrossSize, CrossSize));
        }

        private void StartMove() {
            _dragPlane = new Plane();
            var camDirection = Transform.WorldPosition - Scene.MainCamera.Transform.WorldPosition;

            switch (_pressedAxis.Value) {
                case Axis.X:
                    _dragPlane.Normal = new Vector3(0, camDirection.Y, camDirection.Z);
                    break;
                case Axis.Y:
                    _dragPlane.Normal = new Vector3(camDirection.X, 0, camDirection.Z);
                    break;
                case Axis.Z:
                    _dragPlane.Normal = new Vector3(camDirection.X, camDirection.Y, 0);
                    break;

                case Axis.XY:
                    _dragPlane.Normal = new Vector3(0, 0, 1);
                    break;
                case Axis.YZ:
                    _dragPlane.Normal = new Vector3(1, 0, 0);
                    break;
                case Axis.XZ:
                    _dragPlane.Normal = new Vector3(0, 1, 0);
                    break;

                default:
                    throw new NotImplementedException();
            }

            var hit = GetMovePlaneIntersection();
            _moveOffset = Transform.LocalPosition - hit.Point;
        }
        private void Move() {
            var hit = GetMovePlaneIntersection();
            if (hit == null)
                return;

            var position = hit.Point + _moveOffset;
            var axisMask = GetAxisMask(_pressedAxis.Value);
            //axisMask = Vector3.Transform(axisMask, SceneObject.Transform.WorldRotation).Normalized;
            //axisMask = Vector3.NormalTransform(axisMask, SceneObject.Transform.ModelMatrix);

            Transform.LocalPosition = axisMask * position + (Vector3.One - axisMask) * Transform.LocalPosition;
        }
        private void Select() {
            var ray = Scene.MainCamera.Unproject(Scene.Control.ScreenMousePosition);
            var modelSpaceRay = Ray.Transform(ray, Transform.InvertedModelMatrix);

            var axis = GetHitAxis(modelSpaceRay);

            if (axis == null) {
                Unhighlight();
                _selectedAxis = null;
                return;
            }

            if (_selectedAxis == axis)
                return;

            Unhighlight();
            _selectedAxis = axis;
            Highlight();
        }

        private void Highlight() {
            switch (_selectedAxis.Value) {
                case Axis.X:
                    _lineRenderer.Mesh.Colors[0] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[1] = new Vector3(1, 1, 0);
                    //for (int i = 0; i < 14; i++)
                    //    _coneRenderer.Mesh.Colors[i] = new Vector3(1, 1, 0);
                    break;
                case Axis.Y:
                    _lineRenderer.Mesh.Colors[2] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[3] = new Vector3(1, 1, 0);
                    //for (int i = 14; i < 28; i++)
                    //    _coneRenderer.Mesh.Colors[i] = new Vector3(1, 1, 0);
                    break;
                case Axis.Z:
                    _lineRenderer.Mesh.Colors[4] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[5] = new Vector3(1, 1, 0);
                    //for (int i = 28; i < 14 + 28; i++)
                    //    _coneRenderer.Mesh.Colors[i] = new Vector3(1, 1, 0);
                    break;
                case Axis.XY:
                    _lineRenderer.Mesh.Colors[6] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[7] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[8] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[9] = new Vector3(1, 1, 0);
                    break;
                case Axis.XZ:
                    _lineRenderer.Mesh.Colors[10] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[11] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[12] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[13] = new Vector3(1, 1, 0);
                    break;
                case Axis.YZ:
                    _lineRenderer.Mesh.Colors[14] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[15] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[16] = new Vector3(1, 1, 0);
                    _lineRenderer.Mesh.Colors[17] = new Vector3(1, 1, 0);
                    break;
                default:
                    throw new NotImplementedException();
            }

            _lineRenderer.UpdateMesh();
            //_coneRenderer.UpdateMesh();
        }
        private void Unhighlight() {
            if (_selectedAxis == null)
                return;

            switch (_selectedAxis.Value) {
                case Axis.X:
                    _lineRenderer.Mesh.Colors[0] = new Vector3(1, 0, 0);
                    _lineRenderer.Mesh.Colors[1] = new Vector3(1, 0, 0);
                    //for (int i = 0; i < 14; i++)
                    //    _coneRenderer.Mesh.Colors[i] = new Vector3(1, 0, 0);
                    break;
                case Axis.Y:
                    _lineRenderer.Mesh.Colors[2] = new Vector3(0, 1, 0);
                    _lineRenderer.Mesh.Colors[3] = new Vector3(0, 1, 0);
                    //for (int i = 14; i < 28; i++)
                    //    _coneRenderer.Mesh.Colors[i] = new Vector3(0, 1, 0);
                    break;
                case Axis.Z:
                    _lineRenderer.Mesh.Colors[4] = new Vector3(0, 0, 1);
                    _lineRenderer.Mesh.Colors[5] = new Vector3(0, 0, 1);
                    //for (int i = 28; i < 14 + 28; i++)
                    //    _coneRenderer.Mesh.Colors[i] = new Vector3(0, 0, 1);
                    break;
                case Axis.XY:
                    _lineRenderer.Mesh.Colors[6] = new Vector3(1, 0, 0);
                    _lineRenderer.Mesh.Colors[7] = new Vector3(1, 0, 0);
                    _lineRenderer.Mesh.Colors[8] = new Vector3(0, 1, 0);
                    _lineRenderer.Mesh.Colors[9] = new Vector3(0, 1, 0);
                    break;
                case Axis.XZ:
                    _lineRenderer.Mesh.Colors[10] = new Vector3(1, 0, 0);
                    _lineRenderer.Mesh.Colors[11] = new Vector3(1, 0, 0);
                    _lineRenderer.Mesh.Colors[12] = new Vector3(0, 0, 1);
                    _lineRenderer.Mesh.Colors[13] = new Vector3(0, 0, 1);
                    break;
                case Axis.YZ:
                    _lineRenderer.Mesh.Colors[14] = new Vector3(0, 1, 0);
                    _lineRenderer.Mesh.Colors[15] = new Vector3(0, 1, 0);
                    _lineRenderer.Mesh.Colors[16] = new Vector3(0, 0, 1);
                    _lineRenderer.Mesh.Colors[17] = new Vector3(0, 0, 1);
                    break;
                default:
                    throw new NotImplementedException();
            }

            _lineRenderer.UpdateMesh();
            //_coneRenderer.UpdateMesh();
        }

        private Vector3 GetAxisMask(Axis axis) {
            switch (axis) {
                case Axis.X:
                    return new Vector3(1, 0, 0);
                case Axis.Y:
                    return new Vector3(0, 1, 0);
                case Axis.Z:
                    return new Vector3(0, 0, 1);
                case Axis.XY:
                    return new Vector3(1, 1, 0);
                case Axis.XZ:
                    return new Vector3(1, 0, 1);
                case Axis.YZ:
                    return new Vector3(0, 1, 1);
                default:
                    throw new NotImplementedException();
            }
        }
        private Axis? GetHitAxis(Ray modelSpaceRay) {
            var hits = new List<AxisHit>();

            hits.Add(new AxisHit(Axis.XY, _xyBox.Raycast(modelSpaceRay)));
            hits.Add(new AxisHit(Axis.XZ, _xzBox.Raycast(modelSpaceRay)));
            hits.Add(new AxisHit(Axis.YZ, _yzBox.Raycast(modelSpaceRay)));

            hits.Add(new AxisHit(Axis.X, _xBox.Raycast(modelSpaceRay)));
            hits.Add(new AxisHit(Axis.Y, _yBox.Raycast(modelSpaceRay)));
            hits.Add(new AxisHit(Axis.Z, _zBox.Raycast(modelSpaceRay)));

            hits.RemoveAll(_ => _.Hit == null);
            if (hits.Count == 0)
                return null;

            hits.Sort();
            return hits[0].Axis;
        }
        private RaycastHit GetMovePlaneIntersection() {
            var screenMousePosition = Scene.Control.ScreenMousePosition;
            var ray = Scene.MainCamera.Unproject(screenMousePosition);
            ray = Ray.Transform(ray, Transform.InvertedModelMatrix);
            var hit = _dragPlane.Raycast(ray);
            if (hit == null)
                return null;

            hit.Point = Vector3.Transform(hit.Point, Transform.ModelMatrix);
            return hit;
        }

        internal enum Axis {
            X, Y, Z,
            XY, XZ, YZ
        }

        internal class AxisHit : IComparable<AxisHit> {
            public Axis Axis { get; private set; }
            public RaycastHit Hit { get; private set; }

            public AxisHit(Axis axis, RaycastHit hit) {
                this.Axis = axis;
                this.Hit = hit;
            }

            public int CompareTo(AxisHit other) {
                return this.Hit.Distance.CompareTo(other.Hit.Distance);
            }
        }
    }
}
