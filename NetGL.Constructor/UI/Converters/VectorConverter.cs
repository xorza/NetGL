using System;
using System.Globalization;
using System.Windows.Data;
using NetGL.Core.Mathematics;

namespace NetGL.Constructor.UI.Converters {
    public class VectorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Vector2) {
                var vec2 = (Vector2)value;

                if (typeof(Vector4) == targetType)
                    return new Vector4(vec2);
                if (typeof(Vector3) == targetType)
                    return new Vector3(vec2);
                if (typeof(Vector2) == targetType)
                    return vec2;
            }
            if (value is Vector3) {
                var vec3 = (Vector3)value;

                if (typeof(Vector4) == targetType)
                    return new Vector4(vec3);
                if (typeof(Vector3) == targetType)
                    return vec3;
                if (typeof(Vector2) == targetType)
                    return new Vector2(vec3);
            }
            if (value is Vector4) {
                var vec4 = (Vector4)value;

                if (typeof(Vector4) == targetType)
                    return vec4;
                if (typeof(Vector3) == targetType)
                    return new Vector3(vec4);
                if (typeof(Vector2) == targetType)
                    return new Vector2(vec4);
            }

            throw new NotSupportedException("VectorConverter does not support converting {0} to {1}".UseFormat(value.GetType(), targetType));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
