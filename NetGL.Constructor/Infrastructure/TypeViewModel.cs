using System;

namespace NetGL.Constructor.Infrastructure {
    public class TypeViewModel {
        public Type Type { get; private set; }

        public TypeViewModel(Type t) {
            Assert.NotNull(t);

            this.Type = t;
        }

        public override string ToString() {
            return Type.Name;
        }
    }
}
