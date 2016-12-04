using NetGL.Constructor.Infrastructure;
using NetGL.Core;
using NetGL.Core.Mathematics;
using NetGL.Core.Meshes;
using NetGL.Core.Text;
using NetGL.Core.Types;
using NetGL.SceneGraph;
using NetGL.SceneGraph.Colliders;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Import;
using NetGL.SceneGraph.OpenGL;
using NetGL.SceneGraph.Scene;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using Box = NetGL.Core.Meshes.Box;
using Plane = NetGL.Core.Meshes.Plane;

namespace NetGL.Constructor.Scene {
    public class SceneViewModel : NotifyPropertyChange {
        private static SceneViewModel _instance = null;
        public static SceneViewModel Instance {
            get {
                return _instance ?? (_instance = new SceneViewModel());
            }
        }

        private ObjectSelector _selector;
        private Node _selectedSceneObject;

        private SceneGraph.Scene.Scene _scene;

        public NetGL.SceneGraph.Scene.Scene Scene {
            get {
                return _scene;
            }
            set {
                if (_scene == value)
                    return;

                _scene = value;
                SetScene();
            }
        }
        public Node SelectedSceneObject {
            get {
                return _selectedSceneObject;
            }
            set {
                if (_selectedSceneObject == value)
                    return;

                _selectedSceneObject = value;

                if (_selector != null)
                    _selector.Select(_selectedSceneObject);

                OnPropertyChanged();
            }
        }
        public ICommand AddNewSceneObjectCommand {
            get {
                return new Command(() => {
                    new Node(_scene);
                });
            }
        }

        public TextureFactory TextureFactory { get; private set; }

        private SceneViewModel() {
            TextureFactory = new TextureFactory();
        }

        private void SetScene() {
            //AddDemo();

            AddCamera();
            AddLights();

            //AddReflectionTest();
            AddPlane();
            //AddTestParticles();
            AddSpheres();
            //AddBroccoli();
            //AddConstructorComponents();
            //AddText();
            //AddGrid();
            //AddCollider();
            //AddGizmo();
            //AddShakeLight();
            //AddDiscardPlane();
            //AddDeferredTest();

            //AddTestObjectCreation();

            //AddParsedObj();

            //AddEarth();
            //AddWellbore();

            //AddTestComponent();

            AddParsedFbx();
        }

        private void AddTestComponent() {
            var so = new Node(Scene, "test1");
            so.AddComponent<TestComponent>();
        }
        private void AddDemo() {
            var cameraSO = new Node(Scene, "main camera"); //scene object that will host Camera and CameraManipulator components
            cameraSO.AddComponent<Camera>(); //adding Camera to scene
            cameraSO.AddComponent<CameraManipulator>(); //adding manipulator so you can control camera with (W, A, S, D and mouse Mid) around the scene.

            var sphereSO = new Node(Scene, "spheres"); //scene object that will host sphere mesh renderer component
            sphereSO.Transform.WorldPosition = new Vector3(0, 0, -1); //position object in world space
            var renderer = sphereSO.AddComponent<MeshRenderable>(); //adding component that can render 3D meshes
            var mesh = Icosahedron.Create(0.05f, 3); //generating Icosahedron mesh (sphere)
            renderer.Mesh = mesh; //assigning mesh to renderer

            var lightSO = new Node(Scene, "light0"); //scene object for light source component hosting
            lightSO.Transform.WorldPosition = new Vector3(0, 0, -0.5f); //setting its position
            lightSO.AddComponent<LightSource>(); //adding lightsource component
        }
        private void AddWellbore() {
            var so = new Node(_scene, "wellbore");
            so.AddComponent<Wellbore>();
        }
        private void AddEarth() {
            var so = new Node(_scene, "earth");
            so.AddComponent<EarthPopulation>();
        }
        private void AddReflectionTest() {
            var environmentMap = new TextureCubemap();
            var images = new Bitmap[6]{
               (Bitmap) Bitmap.FromFile("Resources\\Textures\\Cubemap\\xneg.png"),
               (Bitmap) Bitmap.FromFile("Resources\\Textures\\Cubemap\\xpos.png"),
               (Bitmap) Bitmap.FromFile("Resources\\Textures\\Cubemap\\yneg.png"),
               (Bitmap) Bitmap.FromFile("Resources\\Textures\\Cubemap\\ypos.png"),
               (Bitmap) Bitmap.FromFile("Resources\\Textures\\Cubemap\\zneg.png"),
               (Bitmap) Bitmap.FromFile("Resources\\Textures\\Cubemap\\zpos.png"),
            };
            environmentMap.SetImages(images);

            var mesh = Icosahedron.Create(0.4f, 3);

            var material = new Material(MaterialType.RimReflectionDiffuseTextureColor, RenderQueue.Opaque);
            material.Color = Vector4.Zero;
            material.SetTexture("uniform_ReflectionTexture", environmentMap);

            var so = new Node(_scene, "reflective");
            var renderer = so.AddComponent<MeshRenderable>();
            renderer.Material = material;
            renderer.Mesh = mesh;
        }
        private void AddDeferredTest() {
            var planeSo = new Node(_scene, "plane");
            planeSo.AddComponent<MeshRenderable>().Mesh = Plane.Create(10, 10);
            planeSo.Transform.LocalRotation = new Quaternion(Vector3.Left, MathF.PI / 2);

            var sphereMesh = Icosahedron.Create(0.01f, 2);
            var sphereMaterial = new Material(MaterialType.FlatColor);
            sphereMaterial.Color = Vector4.One;

            var lightsParent = new Node(_scene, "lights");
            lightsParent.AddComponent<Rotator>().Rotation = new Vector3(0.7, 0, 0);
            var lightCount = 50;
            for (int i = 0; i < lightCount; i++) {
                var so = new Node(_scene);
                so.Transform.Parent = lightsParent.Transform;
                var light = so.AddComponent<LightSource>();
                var color = new Vector3(RandomF.Float(), RandomF.Float(), RandomF.Float());
                color /= MathF.Max(color.X, color.Y, color.Z);
                light.Diffuse = color;
                light.Attenuation = new Vector3(0, 4f, 0);

                var position = Vector3.Zero;
                while (position.LengthSquared < 0.05f)
                    position = RandomF.InsideUnitSphere();
                so.Transform.LocalPosition = position * 5;

                var meshRenderer = so.AddComponent<MeshRenderable>();
                meshRenderer.Mesh = sphereMesh;
                meshRenderer.Material = sphereMaterial;
            }
        }
        private void AddTestParticles() {
            //var so = new SceneObject(_scene, "particles");
            //var particles = so.AddComponent<ParticlesRenderable>();
            //var material = particles.Material = new Material(new ShaderProgram(Resources.Particles_vert, Resources.Particles_frag, null), RenderQueue.Transparent);
            //material.MainTexture = new Texture2("Resources/Textures/star.png", false, false);
            //material.MainTexture.Wrap = TextureWrapMode.ClampToBorder;
            //material.BlendMode = BlendingMode.Additive;
            throw new NotImplementedException();
        }
        private void AddTestObjectCreation() {
            var so = new Node(_scene, "test object creation");
            so.AddComponent<TestObjectCreation>();
        }
        private void AddDiscardPlane() {
            var so = new Node(Scene, "dicard plane");
            so.Transform.LocalPosition = new Vector3(0, -0.5f, 0);
            so.AddComponent<DiscardTester>();
        }
        private void AddShakeLight() {
            var so = new Node(Scene, "shaking light 1");
            var light = so.AddComponent<LightSource>();
            light.Type = LightType.Point;

            var r = so.AddComponent<MeshRenderable>();
            var mesh = Icosahedron.Create(0.05f, 3);
            var material = new Material(MaterialType.FlatColor);
            r.Mesh = mesh;
            r.Material = material;

            so.AddComponent<Tweener>();
            //so.AddComponent<Shaker>();
        }
        private void AddGizmo() {
            var gizmo = new Node(Scene, "rotation gizmo");
            gizmo.Transform.LocalPosition = new Vector3(2, 0, 0);
            gizmo.AddComponent<RotationGizmo>();
            gizmo.Layer = Layer.IgnoreRaycast;

            gizmo = new Node(Scene, "translation gizmo");
            gizmo.Transform.LocalPosition = new Vector3(-2, 0, 0);
            gizmo.AddComponent<TranslationGizmo>();
            gizmo.Layer = Layer.IgnoreRaycast;

            gizmo = new Node(Scene, "scale gizmo");
            gizmo.Transform.LocalPosition = new Vector3(0, 0, 0);
            gizmo.AddComponent<ScaleGizmo>();
            gizmo.Layer = Layer.IgnoreRaycast;
        }
        private void AddCollider() {
            var colliderSO = new Node(Scene, "collider");
            var collider = colliderSO.AddComponent<BoxCollider>();

            var renderer = colliderSO.AddComponent<MeshRenderable>();
            renderer.Mesh = Box.CreateLines();
            renderer.Material = new Material(MaterialType.DiffuseColor);
            renderer.Material.Color = new Vector4(1, 1, 1, 0.5f);
        }
        private void AddLights() {
            var material = new Material(MaterialType.FlatColor);
            material.Color = new Vector4(1, 1, 1, 1);
            var mesh = Icosahedron.Create(0.02f, 1);

            var light = new Node(Scene, "light0");
            light.Transform.LocalPosition = new Vector3(0, 1.5f, 0);
            light.AddComponent<LightSource>();
            var renderer = light.AddComponent<MeshRenderable>();
            renderer.Mesh = mesh;
            renderer.Material = material;

            light = new Node(Scene, "light1");
            light.Transform.LocalPosition = new Vector3(0, -1.5f, 0);
            light.AddComponent<LightSource>();
            renderer = light.AddComponent<MeshRenderable>();
            renderer.Mesh = mesh;
            renderer.Material = material;
        }
        private void AddCamera() {
            var cameraRotator = new Node(Scene, "camera rotator");

            var rotator = cameraRotator.AddComponent<Rotator>();
            rotator.Rotation = new Vector3(0, 0.05f, 0);
            rotator.IsEnabled = false;

            var cameraSO = new Node(Scene, "main camera");
            cameraSO.Transform.Parent = cameraRotator.Transform;
            cameraSO.Transform.LocalPosition = new Vector3(0, 2, 1.2f);
            cameraSO.Transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(new Vector3(-1, 0, 0));

            var camera = cameraSO.AddComponent<Camera>();
            cameraSO.AddComponent<CameraManipulator>();

            //var shake = new ShakeTween(_scene.Time, cameraRotator.Transform, 1, new Vector3(0.1f));
            //_glControl.Scene.Tweens.Add(shake);
        }
        private void AddPlane() {
            var planeSo = new Node(Scene, "plane");

            planeSo.Transform.LocalPosition = new Vector3(0, 0.01f, 0);
            planeSo.Transform.LocalRotation = new Quaternion(Vector3.Left, MathF.PI / 2);
            var renderer = planeSo.AddComponent<MeshRenderable>();
            renderer.IsEnabled = true;

            var mesh = Plane.Create(2, 2);
            renderer.Mesh = mesh;
            renderer.Material = new Material(MaterialType.DiffuseNormalTextureColor);
            renderer.Material.SetTexture("uniform_NormalTexture", new Texture2("Resources/Textures/1.png", true));
            renderer.Material.MainTexture = new Texture2("Resources/Textures/2.png", false);
        }
        private void AddGrid() {
            var p = new Node(Scene);
            p.Layer = Layer.IgnoreRaycast;

            var renderer1 = p.AddComponent<MeshRenderable>();
            renderer1.Material = new Material(MaterialType.FlatColor);
            renderer1.Material.Color = new Vector4(0.7f);
            renderer1.Mesh = Grid.Create(21, 3);
        }
        private void AddConstructorComponents() {
            var selectorSo = new Node(Scene, "selector");
            selectorSo.IsSerialized = false;
            _selector = selectorSo.AddComponent<ObjectSelector>();
            _selector.Selected += renderer => {
                if (renderer == null)
                    SelectedSceneObject = null;
                else
                    SelectedSceneObject = renderer.SceneObject;
            };

            selectorSo.AddComponent<MouseDrag>();
        }
        private void AddText() {
            //var font = new FontRenderer(new Font("Consolas", 150));

            //var textSo = new SceneObject(Scene);
            //var textRenderer = textSo.AddComponent<MeshRenderable>();
            //textRenderer.Mesh = font.CreateMesh("123asd lalala asfas 8234 2");
            //textRenderer.Material = new Material(new ShaderProgram(NetGL.Core.Shaders.Shaders.Text_vert, NetGL.Core.Shaders.Shaders.Text_frag, null), RenderQueue.CustomShaderOpaque);
            //textRenderer.Material.MainTexture = font.CreateTexture();
            //textRenderer.Material.Color = new Vector4(1.0f);


            throw new NotImplementedException();
        }
        private void AddBroccoli() {
            var mesh = Icosahedron.Create(0.7f, 4);
            var sphereSo = new Node(Scene, "big sphere");
            sphereSo.Transform.LocalPosition = new Vector3(0, -3, 0);
            var material = new Material(Shaders.Brokkoli_vert, Shaders.Brokkoli_frag, RenderQueue.Opaque);
            material.MainTexture = new Texture2("Resources\\Textures\\green.png", false);
            material.MainTexture.Wrap = TextureWrapMode.ClampToEdge;
            material.Color = new Vector4(1, 1, 1, 1);
            material.SetValue("uniform_Weight", 0.5f);

            var r = sphereSo.AddComponent<MeshRenderable>();
            r.Mesh = mesh;
            r.Material = material;
        }
        private void AddSpheres() {
            var mesh = Icosahedron.Create(0.05f, 3);
            var spheres = new Node(Scene, "spheres");
            spheres.Transform.WorldPosition = new Vector3(0, 0.3f, 0);

            var material = new Material(MaterialType.DiffuseColor);


            var count = 5;
            for (int i = 0; i < count; i++) {
                for (int j = 0; j < count; j++) {
                    for (int k = 0; k < count; k++) {
                        var so = new Node(Scene);
                        var r = so.AddComponent<MeshRenderable>();
                        r.Mesh = mesh;
                        r.Material = material;

                        so.Transform.LocalPosition = (new Vector3(i, k, j) - new Vector3(1, 0, 1) * (count * 0.5f)) * 0.2f;
                        so.Transform.Parent = spheres.Transform;
                    }
                }
            }
        }

        private void AddParsedObj() {
            var importer = new ObjImporter();
            var mesh = importer.LoadFile("Resources\\Meshes\\interior.objx");
            mesh.CalculateNormals();

            var so = new Node(_scene, "teapot");
            so.Transform.LocalScale = new Vector3(0.2f);

            var renderer = so.AddComponent<MeshRenderable>();

            renderer.Mesh = mesh;
        }

        private void AddParsedFbx() {
            var importer = new FbxImporter();
            importer.Load(_scene, "C:\\test.fbx");

        }
    }
}
