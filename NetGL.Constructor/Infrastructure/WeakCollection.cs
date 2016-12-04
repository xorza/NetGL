using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NetGL.Constructor.Infrastructure {
    public class WeakCollection<T> : IEnumerable<T> where T : class {
        private readonly List<WeakReference> _collection = new List<WeakReference>();

        public void Add(T item) {
            if (_collection.Any(_ => ReferenceEquals(_.Target, item)))
                throw new ArgumentException("WeakCollection already contains " + item.ToString());

            var reference = new WeakReference(item);
            _collection.Add(reference);
        }
        public void Remove(T item) {
            _collection.RemoveAll(_ => _.IsAlive == false || ReferenceEquals(_.Target, item));
        }

        public IEnumerator<T> GetEnumerator() {
            _collection.RemoveAll(_ => _.IsAlive == false);
            return _collection
                .Select(_ => (T)_.Target)
                .ToList()
                .Where(_ => ReferenceEquals(_, null) == false)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
