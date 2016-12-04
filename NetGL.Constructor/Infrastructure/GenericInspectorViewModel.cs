using NetGL.SceneGraph.Scene;
using NetGL.SceneGraph.Serialization;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace NetGL.Constructor.Infrastructure {
    public class GenericInspectorViewModel : IRefresh {
        private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public;

        public Component Component { get; private set; }
        public ObservableCollection<PropertyViewModels> Properties { get; private set; }

        public GenericInspectorViewModel(Component component) {
            Assert.NotNull(component);

            Component = component;
            Properties = new ObservableCollection<PropertyViewModels>();

            component
                .GetType()
                .GetProperties(Flags)
                .Where(_ =>
                    Attribute.IsDefined(_, typeof(NotSerializedAttribute)) == false
                    && _.CanWrite == true
                    && _.CanRead == true
                    )
                .Select(_ => PropertyEditorViewModelFactory.CreatePropertyEditor(component, _))
                .Where(_ => _ != null)
                .OrderBy(_ => _.DisplayName)
                .ForEach(_ => Properties.Add(_));
        }

        public void Refresh() {
            Properties
                .OfType<IRefresh>()
                .ForEach(_ => _.Refresh());
        }
    }
}
