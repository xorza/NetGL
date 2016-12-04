
using FbxConverterTypes;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.SceneGraph.Import {

    public sealed class FbxImporter {
        public FbxImporter() {
        }

        public void Load(Scene.Scene scene, String filename) {
            var assemblyFilename = Directory.GetCurrentDirectory() + "/FbxConverter.dll";
            Assembly fbxConverterLib = Assembly.LoadFile(assemblyFilename);
            Module module = fbxConverterLib.GetModules().Single();
            Type sceneLoaderType = module.GetTypes().Single(_ => _.Name == "SceneLoader");
            ISceneLoader loader = Activator.CreateInstance(sceneLoaderType) as ISceneLoader;

            if (loader == null) {
                throw new InvalidOperationException();
            }

            FbxConverterTypes.Node fbxscene = loader.Load(filename);

            ImportNode(scene, fbxscene);
        }

        private Scene.Node ImportNode(Scene.Scene scene, FbxConverterTypes.Node node) {
            var so = new Scene.Node(scene, node.name);
            so.Transform.LocalPosition = ToVector3(node.position);
            so.Transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(ToVector3(node.rotation));
            so.Transform.LocalScale = ToVector3(node.scale);

            foreach (var child in node.nodes) {
                var subnode = ImportNode(scene, child);
                subnode.Transform.Parent = so.Transform;
            }

            if (node.mesh != null) {
                var renderable = so.AddComponent<MeshRenderable>();
                var mesh = new Core.Types.Mesh();
                mesh.Vertices = node.mesh.vertices.Select(_ => new Vector3(_.x, _.y, _.z)).ToArray();
                mesh.Normals = node.mesh.normals.Select(_ => new Vector3(_.x, _.y, _.z)).ToArray();
                mesh.Tangents = node.mesh.tangents.Select(_ => new Vector3(_.x, _.y, _.z)).ToArray();
                mesh.TexCoords = node.mesh.uvs.Select(_ => new Vector2(_.x, _.y)).ToArray();
                renderable.Mesh = mesh;
            }

            return so;
        }

        private Vector3 ToVector3(Vec3 v) {
            return new Vector3(v.x, v.y, v.z);
        }
        private Vector2 ToVector2(Vec2 v) {
            return new Vector2(v.x, v.y);
        }
    }
}
