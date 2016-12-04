using System;

namespace NetGL.Core {
    public class ContextMissingException : Exception {
        public ContextMissingException()
            : base("No current context for this thread") {
        }
    }
}