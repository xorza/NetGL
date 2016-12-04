using System;
using System.Reflection;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using System.Linq.Expressions;

namespace NetGL.Constructor.Infrastructure {
    public class BooleanPropertyEditorViewModel : GenericPropertyViewModel<bool> {
        public BooleanPropertyEditorViewModel(Component component, PropertyInfo prop)
            : base(component, prop) { }
    }
    public class MaterialPropertyEditorViewModel : PropertyViewModels, IRefresh {
        private MaterialViewModel _value;
        public MaterialViewModel Value {
            get { return _value; }
            set {
                if (_value == value)
                    return;

                _value = value;
                if (_value == null)
                    Property.SetValue(Component, null);
                else
                    Property.SetValue(Component, _value.Material);

                OnPropertyChanged();
            }
        }

        public MaterialPropertyEditorViewModel(Component component, PropertyInfo prop)
            : base(component, prop) {
            Refresh();
        }

        public Material GetMaterial() {
            return (Material)Property.GetValue(Component);
        }
        public void Refresh() {
            var material = GetMaterial();
            if (material == null)
                Value = null;
            else {
                if (Value == null)
                    Value = new MaterialViewModel(material);
                else {
                    if (ReferenceEquals(Value.Material, material))
                        Value.Refresh();
                    else
                        Value = new MaterialViewModel(material);
                }
            }
        }
    }
    public class FloatPropertyEditorViewModel : GenericPropertyViewModel<float> {
        public FloatPropertyEditorViewModel(Component component, PropertyInfo prop)
            : base(component, prop) { }
    }
    public class Int32PropertyEditorViewModel : GenericPropertyViewModel<Int32> {
        public Int32PropertyEditorViewModel(Component component, PropertyInfo prop)
            : base(component, prop) { }
    }
    public class SBytePropertyEditorViewModel : GenericPropertyViewModel<SByte> {
        public SBytePropertyEditorViewModel(Component component, PropertyInfo prop)
            : base(component, prop) { }
    }
    public class Color4PropertyEditorViewModel : GenericPropertyViewModel<Vector4> {
        public Color4PropertyEditorViewModel(Component component, PropertyInfo prop)
            : base(component, prop) { }
    }
    public class Color3PropertyEditorViewModel : GenericPropertyViewModel<Vector3> {
        public Color3PropertyEditorViewModel(Component component, PropertyInfo prop)
            : base(component, prop) { }
    }
    public class Vector3PropertyEditorViewModel : GenericPropertyViewModel<Vector3> {
        public Vector3PropertyEditorViewModel(Component component, PropertyInfo prop)
            : base(component, prop) { }
    }
    public class FloatRangePropertyEditorViewModel : GenericPropertyViewModel<float> {
        public FloatRangePropertyEditorViewModel(Component component, PropertyInfo prop)
            : base(component, prop) {
            var attribute = (NumberRangeAttribute)Attribute.GetCustomAttribute(prop, typeof(NumberRangeAttribute));
            Minimum = attribute.Min;
            Maximum = attribute.Max;
        }

        public double Minimum { get; private set; }
        public double Maximum { get; private set; }
    }
    public class EnumPropertyEditorViewModel : GenericPropertyViewModel<object> {
        public Array PossibleValues { get; private set; }

        public EnumPropertyEditorViewModel(Component component, PropertyInfo prop)
            : base(component, prop) {
            PossibleValues = Enum
                              .GetValues(prop.PropertyType);
        }
    }

    public abstract class GenericPropertyViewModel<T> : PropertyViewModels, IRefresh {
        private T _value;

        public T Value {
            get { return _value; }
            set {
                if (ReferenceEquals(_value, value))
                    return;

                _value = value;
                Property.SetValue(Component, _value);
                OnPropertyChanged();
            }
        }

        protected GenericPropertyViewModel(Component component, PropertyInfo prop) :
            base(component, prop) {
            Refresh();
        }

        public void Refresh() {
            var v = Property.GetValue(Component);
            Value = (T)v;
        }
    }

    public abstract class PropertyViewModels : NotifyPropertyChange {
        public Component Component { get; private set; }
        protected PropertyInfo Property { get; private set; }
        public string DisplayName { get; private set; }
        public PropertyTooltipViewModel Tooltip { get; private set; }

        protected PropertyViewModels(Component component, PropertyInfo prop) {
            Assert.NotNull(prop);
            Assert.NotNull(component);

            Property = prop;
            Component = component;
            DisplayName = DisplayAttribute.GetPropertyDisplayName(prop);
            Tooltip = new PropertyTooltipViewModel(prop);
        }

        public override string ToString() {
            return "Property of type: {0}".UseFormat(Property.Name);
        }
    }

    public static class PropertyEditorViewModelFactory {
        public static PropertyViewModels CreatePropertyEditor(Component comp, PropertyInfo prop) {
            if (Attribute.IsDefined(prop, typeof(NumberRangeAttribute)) && prop.PropertyType.IsNumeric())
                return new FloatRangePropertyEditorViewModel(comp, prop);
            else if (Attribute.IsDefined(prop, typeof(ColorAttribute))) {
                if (typeof(Vector3) == prop.PropertyType)
                    return new Color3PropertyEditorViewModel(comp, prop);
                else if (typeof(Vector4) == prop.PropertyType)
                    return new Color4PropertyEditorViewModel(comp, prop);
                else
                    throw new NotSupportedException();
            }
            else if (prop.PropertyType.IsEnum)
                return new EnumPropertyEditorViewModel(comp, prop);
            else if (typeof(Int32) == prop.PropertyType)
                return new Int32PropertyEditorViewModel(comp, prop);
            else if (typeof(SByte) == prop.PropertyType)
                return new SBytePropertyEditorViewModel(comp, prop);
            if (prop.PropertyType.IsNumeric())
                return new FloatPropertyEditorViewModel(comp, prop);
            else if (typeof(Vector3) == prop.PropertyType)
                return new Vector3PropertyEditorViewModel(comp, prop);
            else if (typeof(bool) == prop.PropertyType)
                return new BooleanPropertyEditorViewModel(comp, prop);
            else if (typeof(Material) == prop.PropertyType)
                return new MaterialPropertyEditorViewModel(comp, prop);

            return null;
        }
    }
}
