using System;
using System.Reflection;

namespace NetGL.SceneGraph.Components {
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DisplayAttribute : Attribute {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public DisplayAttribute(string name) {
            this.Name = name;
            this.Description = null;
        }
        public DisplayAttribute(string name, string description) {
            this.Name = name;
            this.Description = description;
        }

        public static string GetPropertyDisplayName(PropertyInfo property) {
            var attr = (DisplayAttribute)GetCustomAttribute(property, typeof(DisplayAttribute));
            if (attr == null)
                return property.Name;
            else
                return attr.Name;
        }
        public static string GetPropertyDescription(PropertyInfo property) {
            var attr = (DisplayAttribute)GetCustomAttribute(property, typeof(DisplayAttribute));
            if (attr == null)
                return null;
            else
                return attr.Description;
        }
    }
}
