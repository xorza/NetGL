using System;

namespace NetGL.Constructor.Infrastructure {
    public class EventArgs<T> : EventArgs {
        public T Value { get; private set; }

        public EventArgs(T value) {
            this.Value = value;
        }
    }
}
