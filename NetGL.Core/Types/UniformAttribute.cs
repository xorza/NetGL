using System;

namespace NetGL.Core.Types {

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UniformAttribute : Attribute {
        public string UniformName { get; private set; }

        public UniformAttribute() { UniformName = null; }
        public UniformAttribute(string uniformName) {
            if (string.IsNullOrWhiteSpace(uniformName))
                throw new ArgumentException("uniformName");

            this.UniformName = uniformName;
        }
    }
}
