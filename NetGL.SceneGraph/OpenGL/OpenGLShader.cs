using NetGL.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.SceneGraph.OpenGL {
    [Flags]
    public enum ShaderDefines {
        USE_LIGHTING = 1 << 0,
        CUSTOM_VERTEX = 1 << 1,
    }

    internal class OpenGLShader : ShaderProgram {
        private ShaderDefines _flags;

        public virtual ShaderDefines Flags {
            get {
                return _flags;
            }
            set {
                if (IsCompiled)
                    throw new InvalidOperationException("Shader is already compiled!");

                _flags = value;
            }
        }

        public override void Compile() {
            var enumValues = (ShaderDefines[])Enum.GetValues(typeof(ShaderDefines));
            enumValues.ForEach(_ => {
                if ((_ & Flags) == _)
                    Define(_.ToString());
            });            

            base.Compile();
        }
    }
}
