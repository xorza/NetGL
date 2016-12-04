using NetGL.Core.Infrastructure;
using System;

namespace NetGL.Core.Types {
    public class Shader : UIntObject {
        internal Shader(ShaderType type, string source)
            : base() {
            var handle = Context.CreateShader(type);

            try {
                Context.ShaderSource(handle, source);
                Context.CompileShader(handle);

                var compileMessage = Context.GetShaderInfoLog(handle);
                var compileStatus = Context.GetShaderiv(handle, ShaderInfo.CompileStatus);

                if (compileStatus == 0)
                    throw new GLException(string.Format("Error while compiling {0}. Compiler message:\n{1}", type, compileMessage));
                if (string.IsNullOrWhiteSpace(compileMessage) == false)
                    Log.Info(compileMessage);
            }
            catch {
                Context.DeleteShader(handle);
                throw;
            }

            Initialize(handle);
        }

        internal override DisposeAction GetDisposeAction() {
            return Context.DeleteShader;
        }
    }
}