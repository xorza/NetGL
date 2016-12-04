using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.Core.Meshes {
    public class ExtrusionPoint {
        public Vector3 Position { get; set; }
        public float Radius { get; set; }

        public static ExtrusionPoint Lerp(ExtrusionPoint p1, ExtrusionPoint p2, float v) {
            var result = new ExtrusionPoint();

            result.Radius = p1.Radius * (1 - v) + p2.Radius * v;
            result.Position = Vector3.Lerp(p1.Position, p2.Position, v);

            return result;
        }
    }


    public class CircularExtrusion {
        public static Mesh Create(IList<ExtrusionPoint> trajectory, int sideSegmentCount = 20) {
            var builder = new CircularExtrusion(trajectory, sideSegmentCount);
            return builder.Generate();
        }

        private sealed class Orientation {
            public Vector3
                Direction = new Vector3(1, 0, 0),
                BasisX = new Vector3(0, 1, 0),
                BasisY = new Vector3(0, 0, 1);

            public Orientation() { }
            public Orientation(Vector3 direction) {
                var axis = new Vector3(0, 0, 1);

                Direction = direction.Normalized;
                BasisX = Vector3.Cross(direction, axis);
                if (BasisX.Length < 0.001f) {
                    BasisX = new Vector3(1, 0, 0);
                    BasisY = new Vector3(0, 1, 0);
                    return;
                }
                BasisY = Vector3.Cross(direction, BasisX);

                BasisX.Normalize();
                BasisY.Normalize();
            }

            public static Orientation Lerp(Orientation o1, Orientation o2, float v) {
                var result = new Orientation();

                result.Direction = Vector3.Lerp(o1.Direction, o2.Direction, v);
                result.BasisX = Vector3.Lerp(o1.BasisX, o2.BasisX, v);
                result.BasisY = Vector3.Lerp(o1.BasisY, o2.BasisY, v);

                return result;
            }

            public void TransformDirection(Vector3 newDirection) {
                //var axis = Vector3.Cross(Direction, newDirection);
                //var angle = Vector3.Dot(Direction, newDirection);
                //var rotation = new Quaternion(axis, MathF.Acos(angle));
                var rotation = Quaternion.FromToRotation(Direction, newDirection);

                Direction = newDirection;
                BasisX = Vector3.Transform(BasisX, rotation);
                BasisY = Vector3.Transform(BasisY, rotation);
            }

            public Vector3[] GetRing(int sideSegmentCount, ExtrusionPoint point) {
                var vertices = new List<Vector3>();

                for (int i = 0; i < sideSegmentCount; i++) {
                    var v = point.Radius * BasisX * MathF.Cos(MathF.PI * 2 * i / (sideSegmentCount - 1)) +
                        point.Radius * BasisY * MathF.Sin(MathF.PI * 2 * i / (sideSegmentCount - 1)) +
                        point.Position;
                    vertices.Add(v);
                }

                return vertices.ToArray();
            }
            public Vector3[] GetRingNormals(int sideSegmentCount) {
                var vertices = new List<Vector3>();

                for (int i = 0; i < sideSegmentCount; i++) {
                    var v = BasisX * (float)Math.Cos(Math.PI * 2 * i / (sideSegmentCount - 1)) +
                         BasisY * (float)Math.Sin(Math.PI * 2 * i / (sideSegmentCount - 1));
                    vertices.Add(v);
                }

                return vertices.ToArray();
            }
            public Vector2[] GetRingUVs(int sideSegmentCount, float uOffset) {
                var result = new Vector2[sideSegmentCount];
                for (int i = 0; i < sideSegmentCount; i++)
                    result[i] = new Vector2(i / (float)(sideSegmentCount - 1), uOffset);
                return result;
            }
        }

        private readonly int _sideSegmentCount = 20;
        private readonly IList<ExtrusionPoint> _trajectory = null;
        private readonly bool _isCapped = true;

        private List<Vector3> _vertices, _normals;
        private List<Vector2> _uvs;

        private CircularExtrusion(IList<ExtrusionPoint> trajectory, int sideSegmentCount, bool isCapped = false) {
            if (sideSegmentCount < 3) throw new ArgumentOutOfRangeException("sideSegmentCount should be greater than 3");
            if (trajectory.Count < 2) throw new ArithmeticException("trajectory should contain more than 1 trajectory point");

            this._isCapped = isCapped;
            this._trajectory = trajectory;
            this._sideSegmentCount = sideSegmentCount;

            Generate();
        }

        #region extrusion caps
        private void AddBottomCap(ExtrusionPoint point, Orientation orientation) {
            _vertices.AddRange(orientation.GetRing(_sideSegmentCount, point));
            _normals.AddRange(Enumerable.Repeat(orientation.Direction, _sideSegmentCount + 1));
            _uvs.AddRange(Enumerable.Repeat(new Vector2(0, 0), _sideSegmentCount + 1));

            _vertices.Add(point.Position);
        }
        private void AddTopCap(ExtrusionPoint point, Orientation orientation) {
            _vertices.Add(point.Position);

            _vertices.AddRange(orientation.GetRing(_sideSegmentCount, point));
            _normals.AddRange(Enumerable.Repeat(-orientation.Direction, _sideSegmentCount + 1));
            _uvs.AddRange(Enumerable.Repeat(new Vector2(0, 0), _sideSegmentCount + 1));
        }
        #endregion

        #region geometry generation
        private Mesh Generate() {
            _vertices = new List<Vector3>();
            _normals = new List<Vector3>();
            _uvs = new List<Vector2>();

            var uOffset = 0f;
            var ringCount = 0;
            ExtrusionPoint currentPoint, nextPoint = null;

            var orientation = new Orientation(GetDirection(0));

            for (int i = 0; i < _trajectory.Count - 1; i++) {
                currentPoint = _trajectory[i];
                nextPoint = _trajectory[i + 1];
                uOffset = i / (float)(_trajectory.Count - 1);

                var direction = GetDirection(i);
                orientation.TransformDirection(direction);

                if (_isCapped && i == 0)
                    AddTopCap(currentPoint, orientation);

                AddRing(uOffset, currentPoint, orientation);
                ringCount++;
            }

            orientation.TransformDirection(GetDirection(_trajectory.Count - 1));
            AddRing(uOffset, nextPoint, orientation);
            ringCount++;

            if (_isCapped)
                AddBottomCap(nextPoint, orientation);

            var mesh = new Mesh();

            mesh.Vertices = _vertices;
            mesh.Normals = _normals;
            mesh.TexCoords = _uvs;
            mesh.Indices = GetIndices(ringCount);

            mesh.Type = PrimitiveType.Triangles;
            mesh.DrawStyle = PolygonMode.Fill;
            mesh.FrontFace = FrontFaceDirection.CounterClockwise;

            mesh.CalculateBounds();

            _vertices = null;
            _normals = null;
            _uvs = null;

            return mesh;
        }
        private void AddRing(float uOffset, ExtrusionPoint currentPoint, Orientation orientation) {
            var ringVertices = orientation.GetRing(_sideSegmentCount, currentPoint);
            var ringNormals = orientation.GetRingNormals(_sideSegmentCount);
            var ringUVs = orientation.GetRingUVs(_sideSegmentCount, uOffset);


            _vertices.AddRange(ringVertices);
            _normals.AddRange(ringNormals);
            _uvs.AddRange(ringUVs);
        }
        private int[] GetIndices(int ringCount) {
            var result = new List<int>();

            for (int i = 0; i < _sideSegmentCount && _isCapped; i++) {
                result.Add(0);
                result.Add((i + 1) % _sideSegmentCount + 1);
                result.Add(i + 1);
            }

            for (int i = 0; i < ringCount - 1; i++) {
                var ring = new int[_sideSegmentCount * 6];
                for (int j = 0; j < _sideSegmentCount - 1; j++) {
                    ring[6 * j + 0] = j + 0;
                    ring[6 * j + 1] = (j + 1) % _sideSegmentCount;
                    ring[6 * j + 2] = j + _sideSegmentCount;
                    ring[6 * j + 3] = j + _sideSegmentCount;
                    ring[6 * j + 4] = (j + 1) % _sideSegmentCount;
                    ring[6 * j + 5] = (j + 1) % _sideSegmentCount + _sideSegmentCount;
                }

                if (_isCapped)
                    result.AddRange(ring.Select(_ => _ + _sideSegmentCount + 1 + _sideSegmentCount * i));
                else
                    result.AddRange(ring.Select(_ => _ + _sideSegmentCount * i));
            }

            for (int i = 0; i < _sideSegmentCount && _isCapped; i++) {
                var lastIndex = _vertices.Count - 1;
                result.Add(lastIndex);
                result.Add(lastIndex - (i + 1) % _sideSegmentCount - 1);
                result.Add(lastIndex - (i + 1));
            }

            return result.ToArray();
        }
        #endregion

        #region extrustion direction (orientation) calculation
        private Vector3 GetDirection(int index)
            //returns ring orientation for current point on trajectory
        {
            Vector3 prev = Vector3.Zero, next = Vector3.Zero, current = _trajectory[index].Position;

            if (index > 0)
                prev = _trajectory[index - 1].Position;
            if (index < _trajectory.Count - 1)
                next = _trajectory[index + 1].Position;

            if (index == 0)
                return (next - current).Normalized;

            if (index == _trajectory.Count - 1)
                return (current - prev).Normalized;

            return ((next - current).Normalized + (current - prev).Normalized) / 2;
        }

        //following two methods return next or prev point on trajectory thats distance to current(index) is greater than Config.MinNoticeableLength
        private Vector3 GetPreviousPointOnTrajectory(int index) {
            Vector3 result;
            int i = index;

            i--;
            result = _trajectory[i].Position;

            return result;
        }
        private Vector3 GetNextPointOnTrajectory(int index) {
            Vector3 result;
            int i = index;

            i++;
            result = _trajectory[i].Position;

            return result;
        }
        #endregion
    }
}
