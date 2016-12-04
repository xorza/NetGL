using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetGL.SceneGraph.Serialization {
    internal class ComponentDTO {
        private static readonly Type TypeOfSingle = typeof(Single);
        private static readonly Type TypeOfNotSerialized = typeof(NotSerializedAttribute);

        private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        public string TypeName { get; set; }
        public Guid? TypeGUID { get; set; }
        public Guid Id { get; set; }
        public bool IsEnabled { get; set; }

        public Dictionary<string, object> FieldValues { get; set; }

        public ComponentDTO() {
            FieldValues = new Dictionary<string, object>();
        }
        public ComponentDTO(SerializationContext context, Component component)
            : this() {
            var type = component.GetType();
            TypeName = type.AssemblyQualifiedName;
            Id = component.Id;
            IsEnabled = component.IsEnabled;
            TypeGUID = type.GUID;

            component
                .GetType()
                .GetProperties(Flags)
                .Where(_ => Attribute.IsDefined(_, TypeOfNotSerialized) == false)
                .ForEach(_ => SerializeProperty(context, component, _));
        }

        private void SerializeProperty(SerializationContext context, Component component, PropertyInfo prop) {
            Assert.False(FieldValues.ContainsKey(prop.Name));
            Assert.False(Attribute.IsDefined(prop, TypeOfNotSerialized));

            var value = prop.GetValue(component);
            var converted = context.ConvertValue(value);
            if (converted != null)
                FieldValues.Add(prop.Name, converted);
        }


        public Component Create(DeserializationContext context, Node owner) {
            var type = Type.GetType(TypeName);
            var result = (Component)Activator.CreateInstance(type, owner);

            foreach (var keyValue in FieldValues) {
                var propName = keyValue.Key;
                var prop = type.GetProperty(propName, Flags);
                if (prop == null) {
                    Log.Warning("Unknown field: " + propName);
                    continue;
                }
                if (prop.CanWrite == false) {
                    Log.Warning("Property is not writable: " + propName);
                    continue;
                }

                var value = context.ConvertValue(keyValue.Value, prop.PropertyType);
                prop.SetValue(result, value);
            }

            result.IsEnabled = IsEnabled;
            result.Id = Id;

            return result;
        }
    }
}