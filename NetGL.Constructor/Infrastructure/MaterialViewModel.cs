using System;
using System.Linq;
using NetGL.Core;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;

namespace NetGL.Constructor.Infrastructure {
    public class MaterialViewModel : NotifyPropertyChange, IRefresh {
        private readonly Material _material;
        public Material Material { get { return _material; } }

        public TextureViewModel[] Textures { get; private set; }
        public UniformViewModel[] Uniforms { get; private set; }

        public MaterialViewModel(Material material) {
            Assert.NotNull(material);

            _material = material;
            Textures = material.Textures
                .Select(_ => new TextureViewModel(_))
                .ToArray();
            Uniforms = material.Values
                .Select(CreateUniformViewModel)
                .ToArray();

            Refresh();
        }
        private UniformViewModel CreateUniformViewModel(Uniform uniform) {
            switch (uniform.Type) {
                case ActiveUniformType.Float:
                    return new FloatUniformViewModel((FloatUniform)uniform);
                case ActiveUniformType.FloatVec3:
                    return new Vector3UniformViewModel((Vector3Uniform)uniform);
                case ActiveUniformType.FloatVec4:
                    return new Vector4UniformViewModel((Vector4Uniform)uniform);
                case ActiveUniformType.Int:
                    return new IntUniformViewModel((IntUniform)uniform);
                default:
                    throw new NotSupportedException(uniform.Type.ToString());
            }
        }

        public void Refresh() {
            Textures.ForEach(_ => _.Refresh());
            Uniforms.ForEach(_ => _.Refresh());
        }
    }
}
