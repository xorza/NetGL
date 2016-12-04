using System;

namespace NetGL.Core.Helpers {
    [Serializable]
    public sealed class AssertException : Exception {
        public AssertException(string text)
            : base(text) {
        }

        public AssertException()
            : base() {
        }
    }
}