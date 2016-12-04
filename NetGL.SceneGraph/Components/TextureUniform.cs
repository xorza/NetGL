using NetGL.Core.Types;

namespace NetGL.SceneGraph.Components {
    public class TextureUniform {
        private readonly IntUniform _uniform;
        private readonly string _name;
        private Texture _texture;

        public string Name {
            get {
                return _name;
            }
        }
        public IntUniform Uniform {
            get {
                return _uniform;
            }
        }
        public Texture Texture {
            get {
                return _texture;
            }
            set {
                _texture = value;
            }
        }

        internal TextureUniform(IntUniform uniform) {
            _uniform = uniform;
            _name = uniform.Name;
        }
    }
}
