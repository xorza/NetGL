using System;
using System.Collections.Generic;

namespace NetGL.Core.Types {
    public class ResourceTracker : IDisposable {
        private bool _isDisposed = false;

        private const int GCGenerationTrigger = 2;

        private int _gcCount = 0;
        private readonly List<ResourceReference> _trackedResources = new List<ResourceReference>();

        public bool IsActive {
            get {
                return true;
            }
        }
        public bool SaveCreationStackTrace {
            get {
                return true;
            }
        }


        internal ResourceReference AddTrackedResource(UIntObject resource, string creationStackTrace) {
            Assert.True(IsActive);

            if (_isDisposed)
                throw new ObjectDisposedException("ResourceTracker");

            var reference = new ResourceReference(resource, creationStackTrace);
            _trackedResources.Add(reference);

            return reference;
        }

        public void RunGC() {
            if (_isDisposed)
                throw new ObjectDisposedException("ResourceTracker");

            var count = GC.CollectionCount(GCGenerationTrigger);
            if (count == _gcCount)
                return;
            _gcCount = count;

            for (int i = _trackedResources.Count - 1; i >= 0; i--) {
                var track = _trackedResources[i];
                track.UpdateAndDestroy();
                if (track.IsAlive == false)
                    _trackedResources.RemoveAt(i);
            }
        }

        public void Dispose() {
            _isDisposed = true;
            _trackedResources.Clear();
        }
    }
}