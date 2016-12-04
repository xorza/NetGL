using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.OpenGL;

namespace NetGL.SceneGraph.Shaders {
    internal class QuadShader : OpenGLShader {
        private Vector2 _size;
        [Uniform("size")]
        public Vector2 Size { get { return _size; } set { if (_size == value) return; _size = value; OnUniformValueChanged(value); } }

        private Vector2 _position;
        [Uniform("position")]
        public Vector2 Position { get { return _position; } set { if (_position == value) return; _position = value; OnUniformValueChanged(value); } }

        private int _texture;
        [Uniform("texture1")]
        public int Texture { get { return _texture; } set { if (_texture == value) return; _texture = value; OnUniformValueChanged(value); } }
    }
}
