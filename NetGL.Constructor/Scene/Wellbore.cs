using NetGL.Core;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Meshes;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NetGL.Constructor.Scene {
    public class Wellbore : Component {
        private Material _wellMaterial;

        public Wellbore(Node so) : base(so) { }

        protected override void OnStart() {
            base.OnStart();

            var points = new List<Vector3>();
            points.Add(new Vector3(0, 0, 0));
            points.Add(new Vector3(0, 0, 1));
            points.Add(new Vector3(0, 1, 1));
            points.Add(new Vector3(0, 1, 0));
            points.Add(new Vector3(0, 0, 0));
            points.Add(new Vector3(0, -2, 0));
            points.Add(new Vector3(0, -2, -2));

            points = Spline.CatmullRom(points, 0.02f);

            var trajectory = new List<ExtrusionPoint>();
            points.ForEach(_ => trajectory.Add(new ExtrusionPoint() { Radius = 0.1f, Position = _ }));

            var wellMesh = CircularExtrusion.Create(trajectory);
            wellMesh.DrawStyle = PolygonMode.Fill;

            _wellMaterial = new Material(MaterialType.FlatVertexColor);
            var texture = new Texture2();
            _wellMaterial.MainTexture = new Texture2("Resources\\Textures\\Wellbore\\fluid_colors.png", false);
            _wellMaterial.SetTexture("uniform_DisplaceTexture", texture);
            _wellMaterial.Color = Vector4.One;

            var renderer = SceneObject.AddComponent<MeshRenderable>();
            renderer.Mesh = wellMesh;
            renderer.Material = _wellMaterial;

            var buffer = new ByteBuffer(1024 * 4);

            for (int i = 0; i < buffer.Length; i += 4) {
                var t = 0.1f + (i / (float)buffer.Length);

                var noise1 = GetNoise(t * 400f) * 0.6f;
                var noise2 = Math.Pow(GetNoise(t * 6), 2);
                //noise1 = 0;
                //noise2 = 0;
                buffer[i] = (byte)(255 * (noise1 + noise2) / 1.6f);
            }

            texture.SetImage(buffer, new Size(1, buffer.Length / 4), PixelFormat.Red, PixelInternalFormat.R8);
        }
        private float GetNoise(float t) {
            var result = (SimplexNoise.Generate(t) + 1) / 2;
            Assert.True(result >= 0 && result <= 1);
            return result;
        }

        protected override void OnDispose() {
            base.OnDispose();

            Disposer.Dispose(ref _wellMaterial);
        }

        public static string VShader = @"
uniform float uniform_Displace = 0.1;
uniform sampler2D uniform_DisplaceTexture;
uniform sampler2D uniform_MainTexture;

vertex_info getVertex() {
    float displaceSample = texture(uniform_DisplaceTexture, in_TexCoord0).x;
	float displacement = uniform_Displace * displaceSample;
	vec3 position = in_Position + in_Normal * displacement;

	vertex_info result;

	result.model_pos = position;
    result.normal = in_Normal;
    result.tangent = vec3(0);
    result.albedo = texture(uniform_MainTexture, vec2(0, 1 - displaceSample)).xyz;
	return result;
}
";
    }
}
