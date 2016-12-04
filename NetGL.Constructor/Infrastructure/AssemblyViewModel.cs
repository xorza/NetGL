using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetGL.Constructor.Infrastructure {
    public class AssemblyViewModel {
        public string Name { get; private set; }
        public Assembly Assembly { get; private set; }
        public List<TypeViewModel> Types { get; private set; }

        public AssemblyViewModel(Assembly a, Type parentTypeFilter) {
            Assert.NotNull(a);

            this.Assembly = a;
            this.Types = new List<TypeViewModel>();
            this.Name = Assembly.GetName().Name;

            var types = a.GetTypes();
            foreach (var type in types) {
                if (!type.IsSubclassOf(parentTypeFilter))
                    continue;

                this.Types.Add(new TypeViewModel(type));
            }
        }

        public override string ToString() {
            return Name;
        }
    }
}
