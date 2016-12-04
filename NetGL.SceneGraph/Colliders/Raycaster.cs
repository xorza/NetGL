using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NetGL.Core;
using NetGL.Core.Helpers;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using NetGL.Core.Infrastructure;

namespace NetGL.SceneGraph.Colliders {
    internal class Raycaster {
        private Ray _worldSpaceRay;
        private List<RaycastMeshRendererHit> _results = null;
        private float _nearestHit;
        private Layer _currentLayer;

        public Raycaster() { }

        public RaycastMeshRendererHit Raycast(Ray ray, IEnumerable<Renderable> renderables) {
            return Raycast(ray, renderables, Layer.Default);
        }
        public RaycastMeshRendererHit Raycast(Ray ray, IEnumerable<Renderable> renderables, Layer layer) {
            _worldSpaceRay = ray;
            _currentLayer = layer;
            _nearestHit = float.MaxValue;
            _results = new List<RaycastMeshRendererHit>();

            var raycasts = new List<MeshRenderRaycastInfo>();
            var meshRenderers = renderables.OfType<MeshRenderable>();
            foreach (var renderer in meshRenderers) {
                var raycastInfo = RaycastMeshBounds(renderer);
                if (raycastInfo != null)
                    raycasts.Add(raycastInfo);
            }

            raycasts.Sort();
            for (int i = 0; i < raycasts.Count; i++) {
                var raycastInfo = raycasts[i];
                if (raycastInfo.DistanceToBounds > _nearestHit)
                    break;

                RaycastMeshTriangles(raycastInfo);
            }

            _results.Sort();
            return _results.FirstOrDefault();
        }

        private MeshRenderRaycastInfo RaycastMeshBounds(MeshRenderable renderer) {
            if (renderer.IsRaytracable == false)
                return null;
            if (renderer.IsEnabled == false)
                return null;
            if (renderer.Mesh == null)
                return null;
            if (renderer.Mesh.Bounds == null)
                return null;
            if (renderer.Mesh.Vertices == null)
                return null;
            if (renderer.SceneObject.Layer.Intersects(_currentLayer) == false)
                return null;

            if (renderer.Mesh.Type != PrimitiveType.Triangles) {
                Log.Warning(renderer.Mesh.Type + " is not supported for raytracing");
                return null;
            }

            var result = new MeshRenderRaycastInfo(_worldSpaceRay, renderer);
            if (renderer.Mesh.Bounds.Sphere.FastRaycast(result.ModelSpaceRay))
                return result;
            else
                return null;
        }
        private void RaycastMeshTriangles(MeshRenderRaycastInfo raycast) {
            var mesh = raycast.MeshRenderable.Mesh;
            var triangle = new Triangle();

            if (mesh.Indices != null) {
                for (int i = 0; i < mesh.Indices.Length / 3; i++) {
                    triangle.A = mesh.Vertices[(Int32)mesh.Indices[3 * i]];
                    triangle.B = mesh.Vertices[(Int32)mesh.Indices[3 * i + 1]];
                    triangle.C = mesh.Vertices[(Int32)mesh.Indices[3 * i + 2]];

                    var hit = triangle.Raycast(raycast.ModelSpaceRay);
                    if (hit != null)
                        AddHit(raycast, hit);
                }
            }
            else {
                for (int i = 0; i < mesh.Vertices.Length / 3; i++) {
                    triangle.A = mesh.Vertices[3 * i];
                    triangle.B = mesh.Vertices[3 * i + 1];
                    triangle.C = mesh.Vertices[3 * i + 2];

                    var hit = triangle.Raycast(raycast.ModelSpaceRay);
                    if (hit != null)
                        AddHit(raycast, hit);
                }
            }
        }
        private void AddHit(MeshRenderRaycastInfo raycast, Reference<Vector3> hit) {
            var distance = hit.Value.Z;
            var hitPoint = raycast.ModelSpaceRay.Origin + raycast.ModelSpaceRay.Direction * distance;
            hitPoint = Vector3.Transform(hitPoint, raycast.MeshRenderable.Transform.ModelMatrix);

            var raycasthit = new RaycastMeshRendererHit() {
                Distance = Vector3.Distance(_worldSpaceRay.Origin, hitPoint),
                Renderer = raycast.MeshRenderable,
                Point = hitPoint
            };

            lock (_results) {
                if (_nearestHit > raycasthit.Distance)
                    _nearestHit = raycasthit.Distance;
                _results.Add(raycasthit);
            }
        }

        internal class MeshRenderRaycastInfo : IComparable<MeshRenderRaycastInfo> {
            public float DistanceToBounds { get; private set; }
            public Ray ModelSpaceRay { get; private set; }
            public MeshRenderable MeshRenderable { get; private set; }

            public MeshRenderRaycastInfo(Ray worldSpaceRay, MeshRenderable renderer) {
                this.MeshRenderable = renderer;

                var invertedModelMatrix = new Matrix(renderer.Transform.ModelMatrix);
                invertedModelMatrix.Invert();

                this.ModelSpaceRay = new Ray();

                ModelSpaceRay.Origin = Vector3
                    .Transform(worldSpaceRay.Origin, invertedModelMatrix);
                ModelSpaceRay.Direction = Vector3
                    .NormalTransform(worldSpaceRay.Direction, invertedModelMatrix)
                    .Normalized;

                var modelOriginToCenter = ModelSpaceRay.Origin - MeshRenderable.Mesh.Bounds.Center;
                var modelDistanceOriginToCenter = modelOriginToCenter.Length;
                var modelDistanceOriginToBounds = modelDistanceOriginToCenter - MeshRenderable.Mesh.Bounds.Radius;
                if (modelDistanceOriginToBounds < 0)
                    DistanceToBounds = 0;
                else {
                    var modelCenterToBounds = modelOriginToCenter * (modelDistanceOriginToBounds / modelDistanceOriginToCenter);
                    var worldCenterToBounds = Vector3.NormalTransform(modelCenterToBounds, MeshRenderable.Transform.ModelMatrix);

                    DistanceToBounds = worldCenterToBounds.Length;
                }
            }

            public int CompareTo(MeshRenderRaycastInfo other) {
                return DistanceToBounds.CompareTo(other.DistanceToBounds);
            }
        }
    }
}