using System;
using System.Windows.Forms;
using NetGL.Core;
using NetGL.Core.Mathematics;
using NetGL.Core.Meshes;
using NetGL.Core.Shaders;
using NetGL.Core.Types;
using NetGL.SceneGraph.Scene;
using Box = NetGL.Core.Mathematics.Box;
using Plane = NetGL.Core.Mathematics.Plane;

namespace NetGL.SceneGraph.Components {
    public class ScaleGizmo : Component {
        private const float CrossOuter = 0.55f;
        private const float CrossInner = 0.35f;
        private const float ColliderWidth = 0.03f;

        private MeshRenderable _arrowRenderer, _lineRenderer;
        private Mesh _arrowMesh, _lineMesh;
        private Material _material;
        private int _coneVertexCount;

        private Plane _xyzPlane, _xyPlane, _xzPlane, _yzPlane, _movePlane;
        private Box _xBox, _yBox, _zBox;

        private Axis _selectedAxis = Axis.None, _hoverAxis = Axis.None, _pressedAxis = Axis.None;

        private Vector3 _startMovePoint;

        public ScaleGizmo(Node owner) : base(owner) { }

        protected override void OnStart() {
            base.OnStart();

            SceneObject.IsSerialized = false;

            _material = new Material(MaterialType.FlatVertexColor);

            Scene.Control.MouseMove += Control_MouseMove;
            Scene.Control.MouseDown += Control_MouseDown;
            Scene.Control.MouseUp += Control_MouseUp;

            SetMesh();
            SetPlanes();
        }
        protected override void OnDispose() {
            base.OnDispose();

            Scene.Control.MouseMove -= Control_MouseMove;
            Scene.Control.MouseDown -= Control_MouseDown;
            Scene.Control.MouseUp -= Control_MouseUp;

            _material.Dispose();
            _arrowRenderer.Dispose();
        }

        private void SetMesh() {
            AddCones();
            AddLines();

            Unhighlight();
        }
        private void AddLines() {
            _lineRenderer = SceneObject.AddComponent<MeshRenderable>();
            _lineRenderer.Material = _material;

            _lineMesh = new Mesh();
            _lineMesh.Type = PrimitiveType.Lines;
            _lineMesh.Vertices = new Vector3[]{
                new Vector3(CrossInner, 0, 0),
                new Vector3(1, 0, 0), //X
                                  
                new Vector3(0, CrossInner, 0),
                new Vector3(0, 1, 0), //Y
                                  
                new Vector3(0, 0, CrossInner),
                new Vector3(0, 0, 1),  //Z

                new Vector3(CrossOuter, 0, 0),
                new Vector3(CrossOuter / 2, CrossOuter / 2, 0),
                new Vector3(CrossOuter / 2, CrossOuter / 2, 0),
                new Vector3(0, CrossOuter, 0),
                new Vector3(CrossInner, 0, 0),
                new Vector3(CrossInner / 2, CrossInner / 2, 0),
                new Vector3(CrossInner / 2, CrossInner / 2, 0),
                new Vector3(0, CrossInner, 0), //XY

                new Vector3(0, CrossOuter, 0),
                new Vector3(0, CrossOuter / 2, CrossOuter / 2),
                new Vector3(0, CrossOuter / 2, CrossOuter / 2),
                new Vector3(0, 0, CrossOuter),
                new Vector3(0,  CrossInner, 0),
                new Vector3(0,  CrossInner / 2, CrossInner / 2),
                new Vector3(0,  CrossInner / 2, CrossInner / 2),
                new Vector3(0, 0, CrossInner), //YZ

                new Vector3(CrossOuter, 0, 0),
                new Vector3(CrossOuter / 2, 0, CrossOuter / 2),
                new Vector3(CrossOuter / 2, 0, CrossOuter / 2),
                new Vector3(0, 0, CrossOuter),
                new Vector3(CrossInner, 0, 0),
                new Vector3(CrossInner / 2, 0, CrossInner / 2),
                new Vector3(CrossInner / 2, 0, CrossInner / 2),
                new Vector3(0, 0, CrossInner), //XZ
            };

            _lineMesh.CalculateBounds();
            _lineRenderer.Mesh = _lineMesh;
        }
        private void AddCones() {
            const float ConeWidth = 0.05f;
            const float ConeLength = 0.20f;

            _arrowRenderer = SceneObject.AddComponent<MeshRenderable>();
            _arrowRenderer.Material = _material;

            var xCone = Cone.Create(ConeWidth, ConeLength, 6);
            xCone.Transform(new Matrix(new Vector3(1 - ConeLength, 0, 0), new Quaternion(Vector3.Forward, MathF.PI / 2), Vector3.One));
            _coneVertexCount = xCone.Vertices.Length;

            var yCone = Cone.Create(ConeWidth, ConeLength, 6);
            yCone.Transform(new Matrix(new Vector3(0, 1 - ConeLength, 0), Quaternion.Identity, Vector3.One));

            var zCone = Cone.Create(ConeWidth, ConeLength, 6);
            zCone.Transform(new Matrix(new Vector3(0, 0, 1 - ConeLength), new Quaternion(Vector3.Right, MathF.PI / 2), Vector3.One));

            _arrowMesh = CompoundMesh.Create(xCone, yCone);
            _arrowMesh = CompoundMesh.Create(_arrowMesh, zCone);

            _arrowMesh.CalculateBounds();
            _arrowRenderer.Mesh = _arrowMesh;
        }

        private void SetPlanes() {
            _xyzPlane = new Plane(Vector3.One.Normalized, -CrossInner / MathF.Sqrt(3));
            _xyPlane = new Plane(Vector3.UnitZ, 0);
            _xzPlane = new Plane(Vector3.UnitY, 0);
            _yzPlane = new Plane(Vector3.UnitX, 0);

            _xBox = new Box(new Vector3(0.5f, 0, 0), new Vector3(1, ColliderWidth, ColliderWidth));
            _yBox = new Box(new Vector3(0, 0.5f, 0), new Vector3(ColliderWidth, 1, ColliderWidth));
            _zBox = new Box(new Vector3(0, 0, 0.5f), new Vector3(ColliderWidth, ColliderWidth, 1));
        }

        private void HighlightHoverAxis() {
            if (_hoverAxis == _selectedAxis)
                return;

            Unhighlight();
            _selectedAxis = _hoverAxis;
            switch (_selectedAxis) {
                case Axis.None:
                    return;
                case Axis.X:
                    _lineMesh.Colors[0] = new Vector3(1, 1, 0);
                    _lineMesh.Colors[1] = new Vector3(1, 1, 0);
                    break;
                case Axis.Y:
                    _lineMesh.Colors[2] = new Vector3(1, 1, 0);
                    _lineMesh.Colors[3] = new Vector3(1, 1, 0);
                    break;
                case Axis.Z:
                    _lineMesh.Colors[4] = new Vector3(1, 1, 0);
                    _lineMesh.Colors[5] = new Vector3(1, 1, 0);
                    break;
                case Axis.XY:
                    for (int i = 6; i < 14; i++)
                        _lineMesh.Colors[i] = new Vector3(1, 1, 0);
                    break;
                case Axis.YZ:
                    for (int i = 14; i < 22; i++)
                        _lineMesh.Colors[i] = new Vector3(1, 1, 0);
                    break;
                case Axis.XZ:
                    for (int i = 22; i < 30; i++)
                        _lineMesh.Colors[i] = new Vector3(1, 1, 0);
                    break;
                case Axis.XYZ:
                    for (int i = 0; i < _lineMesh.Colors.Length; i++)
                        _lineMesh.Colors[i] = new Vector3(1, 1, 0);
                    break;
                default:
                    throw new NotImplementedException();
            }

            _lineRenderer.UpdateMesh();
            //_arrowRenderer.UpdateMesh();
        }
        private void Unhighlight() {
            _lineMesh.Colors = new Vector3[] {
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 0), //X
                                  
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 0), //Y
                                  
                new Vector3(0, 0, 1),
                new Vector3(0, 0, 1), //Z
                                  
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 0), 
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 0), //XY
                                  
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, 1), //YZ
                                  
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, 1), //XZ
            };
            _lineRenderer.UpdateMesh();

            if (_arrowMesh.Colors == null)
                _arrowMesh.Colors = new Vector3Buffer(_coneVertexCount * 3);
            for (int i = 0; i < _coneVertexCount * 3; i++) {
                if (i < _coneVertexCount)
                    _arrowMesh.Colors[i] = new Vector3(1, 0, 0);
                else if (i < _coneVertexCount * 2)
                    _arrowMesh.Colors[i] = new Vector3(0, 1, 0);
                else if (i < _coneVertexCount * 3)
                    _arrowMesh.Colors[i] = new Vector3(0, 0, 1);
            }
            _arrowRenderer.UpdateMesh();
        }

        private void Control_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Unhighlight();
                _selectedAxis = Axis.None;
                _pressedAxis = Axis.None;
                _movePlane = null;
                this.Transform.LocalScale = Vector3.One;
            }
        }
        private void Control_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left)
                return;
            if (_selectedAxis == Axis.None)
                return;

            StartMove();
        }
        private void Control_MouseMove(object sender, MouseEventArgs e) {
            if (_pressedAxis == Axis.None) {
                FindHoverAxis();
                HighlightHoverAxis();
            }
            else {
                Move();
            }
        }

        private void StartMove() {
            _pressedAxis = _selectedAxis;

            var camDirection = Transform.WorldPosition - Scene.MainCamera.Transform.WorldPosition;

            switch (_pressedAxis) {
                case Axis.X:
                    _movePlane = new Plane(new Vector3(0, camDirection.Y, camDirection.Z), 0);
                    break;
                case Axis.Y:
                    _movePlane = new Plane(new Vector3(camDirection.X, 0, camDirection.Z), 0);
                    break;
                case Axis.Z:
                    _movePlane = new Plane(new Vector3(camDirection.X, camDirection.Y, 0), 0);
                    break;

                case Axis.XY:
                    _movePlane = _xyPlane;
                    break;
                case Axis.YZ:
                    _movePlane = _yzPlane;
                    break;
                case Axis.XZ:
                    _movePlane = _xzPlane;
                    break;

                case Axis.XYZ:
                    _movePlane = _xyzPlane;
                    break;

                default:
                    throw new NotImplementedException();
            }

            var hit = GetMovePlaneIntersection();
            _startMovePoint = hit.Point;
        }
        private void Move() {
            var hit = GetMovePlaneIntersection();
            if (hit == null)
                return;

            var offset = hit.Point - _startMovePoint;
            var normal = GetAxisNormal();
            var l = Vector3.Dot(normal, offset);
            if (_pressedAxis == Axis.XYZ)
                Transform.LocalScale = Vector3.One + Vector3.One * l;
            else
                Transform.LocalScale = Vector3.One + normal * l;
        }
        private void FindHoverAxis() {
            var ray = Scene.MainCamera.Unproject(Scene.Control.ScreenMousePosition);
            ray = Ray.Transform(ray, Transform.InvertedModelMatrix);

            var hit = _xyzPlane.Raycast(ray);
            if (hit != null) {
                var point = hit.Point;
                if (point.X > 0 && point.Y > 0 && point.Z > 0) {
                    _hoverAxis = Axis.XYZ;
                    return;
                }
            }

            if (CheckAxis(Axis.X, ray))
                return;
            if (CheckAxis(Axis.Y, ray))
                return;
            if (CheckAxis(Axis.Z, ray))
                return;

            if (CheckTwoAxisPlane(Axis.XY, ray))
                return;
            if (CheckTwoAxisPlane(Axis.XZ, ray))
                return;
            if (CheckTwoAxisPlane(Axis.YZ, ray))
                return;

            _hoverAxis = Axis.None;
        }
        private RaycastHit GetMovePlaneIntersection() {
            var screenMousePosition = Scene.Control.ScreenMousePosition;
            var ray = Scene.MainCamera.Unproject(screenMousePosition);
            ray = Ray.Transform(ray, Transform.InvertedModelMatrix);
            var hit = _movePlane.Raycast(ray);
            if (hit == null)
                return null;

            hit.Point = Vector3.Transform(hit.Point, Transform.ModelMatrix);
            return hit;
        }
        private Vector3 GetAxisNormal() {
            switch (_pressedAxis) {
                case Axis.X:
                    return Vector3.UnitX;
                case Axis.Y:
                    return Vector3.UnitY;
                case Axis.Z:
                    return Vector3.UnitZ;
                case Axis.XY:
                    return new Vector3(1, 1, 0).Normalized;
                case Axis.XZ:
                    return new Vector3(1, 0, 1).Normalized;
                case Axis.YZ:
                    return new Vector3(0, 1, 1).Normalized;
                case Axis.XYZ:
                    return new Vector3(-1, 1, -1).Normalized;
                default:
                    throw new NotImplementedException();
            }
        }

        private bool CheckAxis(Axis axis, Ray ray) {
            Box box;

            switch (axis) {
                case Axis.X:
                    box = _xBox;
                    break;
                case Axis.Y:
                    box = _yBox;
                    break;
                case Axis.Z:
                    box = _zBox;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var hit = box.Raycast(ray);
            if (hit == null)
                return false;

            _hoverAxis = axis;
            return true;
        }
        private bool CheckTwoAxisPlane(Axis axis, Ray ray) {
            Plane plane;

            switch (axis) {
                case Axis.XY:
                    plane = _xyPlane;
                    break;
                case Axis.XZ:
                    plane = _xzPlane;
                    break;
                case Axis.YZ:
                    plane = _yzPlane;
                    break;
                default:
                    throw new NotImplementedException(axis.ToString());
            }

            var hit = plane.Raycast(ray);
            if (hit == null)
                return false;

            var point = hit.Point;
            switch (axis) {
                case Axis.XY:
                    point.Z = 0;
                    break;
                case Axis.XZ:
                    point.Y = 0;
                    break;
                case Axis.YZ:
                    point.X = 0;
                    break;
                default:
                    throw new NotImplementedException(axis.ToString());
            }

            if (point.X >= 0 && point.Y >= 0 && point.Z >= 0) {
                var s = point.X + point.Y + point.Z;
                if (s >= CrossInner && s <= CrossOuter) {
                    _hoverAxis = axis;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        internal enum Axis {
            None,
            X, Y, Z,
            XY, XZ, YZ,
            XYZ
        }
    }
}
