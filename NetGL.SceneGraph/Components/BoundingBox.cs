using System;
using NetGL.Core;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Shaders;
using NetGL.Core.Types;
using NetGL.SceneGraph.Scene;
using NetGL.SceneGraph.Serialization;

namespace NetGL.SceneGraph.Components {
    [NotSerialized]
    public class BoundingBox : Component, IUpdatable {
        private MeshRenderable _renderer, _target;
        private Mesh _mesh;

        public BoundingBox(Node owner) : base(owner) { }

        protected override void OnInit() {
            base.OnInit();

            _mesh = new Mesh();
            _mesh.Type = PrimitiveType.Lines;
            _mesh.Indices = new UInt32[]{
                0, 1,
                1, 2,
                2, 3,
                3, 0,
                4, 5,
                5, 6,
                6, 7,
                7, 4,
                0, 4,
                1, 5,
                2, 6,
                3, 7
            };

            _renderer = SceneObject.AddComponent<MeshRenderable>();
            _renderer.Material = new Material(MaterialType.FlatColor);
            _renderer.Material.Color = Vector4.One;
            _renderer.IsRaytracable = false;
            _renderer.IsEnabled = false;
        }
        protected override void OnDispose() {
            base.OnDispose();

            Disposer.Dispose(_renderer.Material);
            Disposer.Dispose(ref _renderer);
        }

        public void Update() {
            if (_target == null)
                return;
            if (_target.IsDisposed) {
                SetTarget(null);
                return;
            }

            Transform.LocalPosition = _target.Transform.WorldPosition + _target.Mesh.Bounds.Center;
            Transform.LocalRotation = _target.Transform.WorldRotation;
            Transform.LocalScale = _target.Transform.LocalScale;
        }

        public void SetTarget(MeshRenderable target) {
            if (IsDisposed) {
                Log.Error("object disposed");
                return;
            }

            if (target == null) {
                _renderer.IsEnabled = false;
                return;
            }
            if (target.Mesh == null) {
                _renderer.IsEnabled = false;
                return;
            }
            if (target.Mesh.Bounds == null) {
                _renderer.IsEnabled = false;
                return;
            }

            _target = target;

            var size = target.Mesh.Bounds.Size / 2;
            var vertices = new Vector3[] {
                new Vector3(-1,-1,-1) * size,
                new Vector3(-1, 1,-1) * size,
                new Vector3( 1, 1,-1) * size,
                new Vector3( 1,-1,-1) * size,
                                        
                new Vector3(-1,-1, 1) * size,
                new Vector3(-1, 1, 1) * size,
                new Vector3( 1, 1, 1) * size,
                new Vector3( 1,-1, 1) * size
            };
            _mesh.Vertices = vertices;

            _renderer.Mesh = _mesh;
            _renderer.UpdateMesh();
            _renderer.IsEnabled = true;

            Update();
        }
    }
}