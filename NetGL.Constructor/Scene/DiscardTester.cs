using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Shaders;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.OpenGL;
using NetGL.SceneGraph.Scene;
using Plane = NetGL.Core.Meshes.Plane;

namespace NetGL.Constructor.Scene {
    public class DiscardTester : Component, IUpdatable {
        private Material _material;
        private MeshRenderable _renderer;

        public DiscardTester(Node owner) : base(owner) { }

        protected override void OnStart() {
            base.OnStart();

            SceneObject.Transform.WorldRotation = new Quaternion(Vector3.UnitX, MathF.PIOver2);

            _renderer = SceneObject.AddComponent<MeshRenderable>();
            _renderer.IsEnabled = true;

            var mesh = Plane.Create(2, 2);
            _renderer.Mesh = mesh;
            _material = _renderer.Material = new Material(null, DiscardFrag, RenderQueue.Opaque);
            _material.Color = Vector4.One;
            _material.SetTexture("uniform_DiscardTexture", new Texture2("Resources/Textures/1.png", false));
            _material.SetTexture("uniform_NormalTexture", new Texture2("Resources/Textures/1.png", true));
            _material.MainTexture = new Texture2("Resources/Textures/2.png", false);
            _material.SetValue("uniform_DiscardFactor", 0f);
        }
        protected override void OnDispose() {
            base.OnDispose();

            _material.Textures.ForEach(_ => Disposer.Dispose(_.Texture));
            Disposer.Dispose(_material.MainTexture);
            Disposer.Dispose(ref _material);
            Disposer.Dispose(ref _renderer);
        }

        public void Update() {
            var factor = MathF.Sin(Time.CurrentFloat * 0.5f);
            _material.SetValue("uniform_DiscardFactor", factor * factor);
        }

        private const string DiscardFrag = @"
uniform vec4 uniform_Color = vec4(1.0);
uniform sampler2D uniform_MainTexture;
uniform sampler2D uniform_NormalTexture;
uniform sampler2D uniform_DiscardTexture;
uniform float uniform_DiscardFactor;
uniform float uniform_SpecPower;
uniform float uniform_SpecIntensity;

void getFragment(inout fragment_info fragment) {
	vec3 rgbFactor = texture(uniform_DiscardTexture, frag_in.texcoord0).rgb * 0.333;
	float factor = rgbFactor.r + rgbFactor.g + rgbFactor.b;
	if(factor < uniform_DiscardFactor)
		discard;

	fragment.normal = UnpackNormal(uniform_NormalTexture, frag_in.texcoord0);
	fragment.albedo = uniform_Color * texture(uniform_MainTexture, frag_in.texcoord0);
	fragment.specPower = uniform_SpecPower;
	fragment.specIntensity = uniform_SpecIntensity;
}
";
    }
}
