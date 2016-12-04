
using NetGL.Core.Mathematics;
using NetGL.Core.Shaders;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using System.Collections.Generic;

namespace NetGL.Constructor.Scene {
    public class TestObjectCreation : Component, IUpdatable {
        private readonly List<MeshRenderable> _renderers = new List<MeshRenderable>();
        private readonly Mesh _mesh = NetGL.Core.Meshes.Sphere.Create(0.1f, 10, 10);

        public TestObjectCreation(Node owner) : base(owner) { }

        public void Update() {
            const int objectCount = 20;

            _renderers.ForEach(_ => _.Dispose());

            for (int i = 0; i < objectCount; i++) {
                var sceneObject = new Node(Scene);
                sceneObject.Transform.Parent = Transform;
                sceneObject.Transform.LocalPosition = 0.3f * new Vector3(-objectCount / 2 + i, 0, 0);

                var renderer = sceneObject.AddComponent<MeshRenderable>();
                renderer.Mesh = _mesh;
                var material = renderer.Material = new Material(MaterialType.DiffuseColor);
                material.Color = new Vector4(RandomF.InsideUnitSphere().Normalized, 1);

                _renderers.Add(renderer);
            }
        }
    }
}
