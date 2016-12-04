using System;
using System.Collections;
using System.Collections.Generic;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Components;

namespace NetGL.SceneGraph.Scene {
    internal sealed class RendererCollection : IEnumerable<Renderable> {
        private readonly List<Renderable> _opaque = new List<Renderable>(500);
        private readonly List<Renderable> _transparent = new List<Renderable>(100);

        private readonly Comparison<Renderable> _comparisonOpaque;
        private readonly Comparison<Renderable> _comparisonTransparent;

        private bool _shouldFullSort = true;
        private Vector3 _cameraPosition;

        public IReadOnlyList<Renderable> Opaque { get; private set; }
        public IReadOnlyList<Renderable> Transparent { get; private set; }

        public RendererCollection() {
            this._comparisonOpaque = CompareOpaque;
            this._comparisonTransparent = CompareTransparent;

            Opaque = _opaque.AsReadOnly();
            Transparent = _transparent.AsReadOnly();
        }

        public void Add(Renderable meshRenderer) {
            meshRenderer.MaterialUpdated += MeshRenderer_MaterialUpdated;

            if (meshRenderer.Material == null) {
                _opaque.Add(meshRenderer);
                _shouldFullSort = true;
            }
            else {
                switch (meshRenderer.Material.RenderQueue) {
                    case RenderQueue.Opaque:
                    case RenderQueue.CustomShaderOpaque:
                        _opaque.Add(meshRenderer);
                        _shouldFullSort = true;
                        break;
                    case RenderQueue.Transparent:
                        _transparent.Add(meshRenderer);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
        public void Remove(Renderable meshRenderer) {
            meshRenderer.MaterialUpdated -= MeshRenderer_MaterialUpdated;
            _transparent.Remove(meshRenderer);
            _opaque.Remove(meshRenderer);
        }
        public void Clear() {
            _opaque.Clear();
            _transparent.Clear();
        }

        public void OpenGLSort(Camera camera) {
            if (_shouldFullSort)
                _opaque.Sort(_comparisonOpaque);

            _cameraPosition = camera.Transform.WorldPosition;
            _transparent.Sort(_comparisonTransparent);

            _shouldFullSort = false;
        }

        private void MeshRenderer_MaterialUpdated(Renderable meshRenderer) {
            Remove(meshRenderer);
            Add(meshRenderer);
        }

        private int CompareOpaque(Renderable x, Renderable y) {
            if (ReferenceEquals(x.Material, y.Material))
                return 0;

            if (x.Material == null)
                return -1;
            if (y.Material == null)
                return 1;

            return x.Material.Shader.CompareTo(y.Material.Shader);
        }
        private int CompareTransparent(Renderable x, Renderable y) {
            var xdist = Vector3.DistanceSquared(_cameraPosition, x.Transform.WorldPosition);
            var ydist = Vector3.DistanceSquared(_cameraPosition, y.Transform.WorldPosition);

            return -xdist.CompareTo(ydist);
        }

        public IEnumerator<Renderable> GetEnumerator() {
            for (int i = 0; i < _opaque.Count; i++)
                yield return _opaque[i];
            for (int i = 0; i < _transparent.Count; i++)
                yield return _transparent[i];
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
