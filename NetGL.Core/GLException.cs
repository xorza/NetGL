using System;

namespace NetGL.Core {
    public class GLException : Exception {
        public GLException() { }
        public GLException(string message) : base(message) { }
        public GLException(string message, Exception innerException) : base(message, innerException) { }
    }
}