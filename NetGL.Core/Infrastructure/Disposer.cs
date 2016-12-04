using System;

namespace NetGL.Core.Infrastructure {
    public static class Disposer {
        public static void Dispose<T>(ref T obj) where T : class {
            var disposable = obj as IDisposable;
            if (disposable != null)
                disposable.Dispose();
            obj = null;
        }
        public static void Dispose<T>(T obj) where T : class {
            var disposable = obj as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
