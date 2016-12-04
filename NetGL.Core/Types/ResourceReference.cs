using System;
using NetGL.Core.Infrastructure;

namespace NetGL.Core.Types {
    internal class ResourceReference : IDisposable {
        private readonly WeakReference _reference;
        private readonly DisposeAction _disposeAction;
        private readonly uint _handle;
        private bool _isAlive;
        private readonly string _creationStackTrace;

        private readonly string _resourceName;

        public Boolean IsAlive {
            get {
                return _isAlive;
            }
        }

        public ResourceReference(UIntObject resource, string creationStackTrace) {
            Assert.NotNull(resource);

            _reference = new WeakReference(resource);
            _handle = resource.Handle;
            _disposeAction = resource.GetDisposeAction();
            _isAlive = true;
            _creationStackTrace = creationStackTrace;

            _resourceName = resource.ToString();
        }

        public void UpdateAndDestroy() {
            if (_isAlive == false)
                return;

            _isAlive = _reference.IsAlive;
            if (_isAlive == false) {
                Log.Info(string.Format("Disposing {0} due to GC, creation stack trace: {1}", _resourceName, _creationStackTrace));
                _disposeAction(_handle);
            }
        }

        public override string ToString() {
            return string.Format("ResourceTracker: alive {0}, name {1}", _reference.IsAlive, _resourceName);
        }

        public void Dispose() {
            _isAlive = false;
        }
    }
}
