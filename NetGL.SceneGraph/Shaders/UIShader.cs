using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.SceneGraph.Shaders {
    internal class UIShader : OpenGLShader {
        //uniform vec2 size;
        //uniform vec2 position;
        //uniform vec2 viewport_size;
        //uniform vec4 color;

        private Vector2 _size, _position, _viewportSize;
        private Vector4 _color;
        private uint _id;

        [Uniform("size")]
        public Vector2 Size { get { return _size; } set { if (_size == value) return; _size = value; OnUniformValueChanged(value); } }
        [Uniform("position")]
        public Vector2 Position { get { return _position; } set { if (_position == value) return; _position = value; OnUniformValueChanged(value); } }
        [Uniform("viewport_size")]
        public Vector2 ViewportSize { get { return _viewportSize; } set { if (_viewportSize == value) return; _viewportSize = value; OnUniformValueChanged(value); } }
        [Uniform("color")]
        public Vector4 Color { get { return _color; } set { if (_color == value) return; _color = value; OnUniformValueChanged(value); } }
        [Uniform("id")]
        public UInt32 ID { get { return _id; } set { if (_id == value) return; _id = value; OnUniformValueChanged(value); } }

        public UIShader() {
            Code = Resources.UI;
            Compile();
        }
    }
}
