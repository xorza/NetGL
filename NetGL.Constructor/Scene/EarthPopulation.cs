using NetGL.Core.Mathematics;
using NetGL.Core.Meshes;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetGL.Constructor.Scene {
    public class EarthPopulation : Component {
        public EarthPopulation(Node so) : base(so) { }

        protected override void OnStart() {
            base.OnStart();

            var mesh = NetGL.Core.Meshes.Sphere.Create(0.5f, 50, 50);

            var material = new Material(null, EarthFragShader, RenderQueue.Opaque);
            material.Color = Vector4.One;
            material.MainTexture = new Texture2("Resources\\Textures\\Earth\\2_no_clouds_8k.jpg", false);
            material.SetTexture("uniform_NightTexture", new Texture2("Resources\\Textures\\Earth\\5_night_8k.jpg", false));
            material.SetTexture("uniform_SpecularTexture", new Texture2("Resources\\Textures\\Earth\\specular.jpg", false));
            material.SetTexture("uniform_NormalTexture", new Texture2("Resources\\Textures\\Earth\\earth_normalmap_flat_8192x4096.jpg", true));
            material.SetValue("uniform_SpecPower", 32);

            var renderer = SceneObject.AddComponent<MeshRenderable>();
            renderer.Material = material;
            renderer.Mesh = mesh;

            SceneObject.AddComponent<Rotator>().Rotation = new Vector3(0, 0.1f, 0);

            var data = ReadPopulationData()[0];
            var datamesh = BuildPopulationMesh(data);

            renderer = SceneObject.AddComponent<MeshRenderable>();
            renderer.Mesh = datamesh;
            renderer.Material = new Material(MaterialType.FlatVertexColor, RenderQueue.Transparent);

            var sunSO = new Node(SceneObject.Scene, "sun");
            sunSO.AddComponent<LightSource>().Type = LightType.Directional;
        }

        private static Mesh BuildPopulationMesh(PopulationData data) {
            var count = Math.Min(8192, data.Data.Count);
            var colors = new Vector3Buffer(8 * count);
            var meshBuilder = new MeshBuilder();

            var matrix = Matrix.Identity;

            for (int i = 0; i < count; i++) {
                var location = data.Data[i];
                var height = location.Size * 0.8f;
                var width = 0.0025f;

                matrix.LoadIdentity();
                matrix.Rotate(Quaternion.CreateFromYawPitchRoll(MathF.Deg2Rad * (location.Lng), 0, MathF.Deg2Rad * (90 - location.Lat)));
                matrix.Translate(new Vector3(-0.5f * width, 0.5f + 0.5f * height, -0.5f * width));
                matrix.Scale(new Vector3(width, height, width));

                meshBuilder.AddBox(matrix);

                var color = Vector3.Lerp(new Vector3(0, 1, 0), new Vector3(1, 0.7f, 0), 3 * location.Size / data.Max);
                for (int k = 0; k < 8; k++)
                    colors[k + i * 8] = color;
            }

            var datamesh = meshBuilder.GetMesh();
            datamesh.Colors = colors;
            return datamesh;
        }

        private static List<PopulationData> ReadPopulationData() {
            var populationData = new List<PopulationData>();

            var json = File.ReadAllText("Resources\\Data\\population909500.json");
            var arr = JArray.Parse(json);
            foreach (JArray item in arr) {
                var year = Int32.Parse((string)((JValue)item[0]).Value);
                var locations = (JArray)item[1];

                var entry = new PopulationData() {
                    Year = year
                };
                populationData.Add(entry);

                for (int i = 0; i < locations.Count; i += 3) {
                    var lat = Convert.ToInt32(((JValue)locations[i]).Value);
                    var lng = Convert.ToInt32(((JValue)locations[i + 1]).Value);
                    var size = Convert.ToSingle(((JValue)locations[i + 2]).Value);

                    var location = new LocationPopulation() {
                        Lat = lat,
                        Lng = lng,
                        Size = size
                    };
                    entry.Max = Math.Max(size, entry.Max);
                    entry.Data.Add(location);
                }
            }

            return populationData;
        }


        internal class PopulationData {
            public int Year;
            public readonly List<LocationPopulation> Data = new List<LocationPopulation>();
            public float Max;
        }
        internal struct LocationPopulation {
            public float Lat;
            public float Lng;
            public float Size;
        }

        private const string EarthFragShader = @"
uniform sampler2D uniform_MainTexture;
uniform sampler2D uniform_NightTexture;
uniform sampler2D uniform_NormalTexture;
uniform sampler2D uniform_SpecularTexture;
uniform vec3 uniform_Rim  = vec3(0.6, 0.6, 0.9);
uniform float uniform_RimPower = 5;
uniform float uniform_BumpStrength = 0.3;
uniform float uniform_SpecPower = 0.0;
uniform float uniform_SpecIntensity = 0.0;

void getFragment(inout fragment_info fragment) {
	vec3 normal = normalize(frag_in.normal);	
	float rim = 1.0 - clamp(dot(-normalize(frag_in.view_pos), normal), 0, 1);

	fragment.emission = uniform_Rim * pow(rim, uniform_RimPower) + texture(uniform_NightTexture, frag_in.texcoord0).xyz;
	fragment.normal = mix(normal, UnpackNormal(uniform_NormalTexture, frag_in.texcoord0), uniform_BumpStrength);
	fragment.albedo = texture(uniform_MainTexture, frag_in.texcoord0);
	fragment.specPower = uniform_SpecPower;
	fragment.specIntensity = texture(uniform_SpecularTexture, frag_in.texcoord0).x;
}
";
    }
}
