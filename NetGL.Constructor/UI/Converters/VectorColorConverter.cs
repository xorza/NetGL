using System;
using System.Globalization;
using System.Windows.Data;
using NetGL.Core.Mathematics;

namespace NetGL.Constructor.UI.Converters {
    public class VectorColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Vector3) {
                var vec3 = (Vector3)value;

                if (typeof(Vector4) == targetType)
                    return new Vector4(vec3, 1);
                if (typeof(Vector3) == targetType)
                    return vec3;
            }
            if (value is Vector4) {
                var vec4 = (Vector4)value;

                if (typeof(Vector4) == targetType)
                    return vec4;
                if (typeof(Vector3) == targetType)
                    return new Vector3(vec4);
            }

            throw new NotSupportedException("VectorConverter does not support converting {0} to {1}".UseFormat(value.GetType(), targetType));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Convert(value, targetType, parameter, culture);
        }
    }

    public class Vector3ToVector4ColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Vector3 == false)
                throw new ArgumentException();

            return new Vector4((Vector3)value, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Vector4 == false)
                throw new ArgumentException();

            return new Vector3((Vector4)value);
        }
    }
    public class Vector4ToVector3ColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Vector4 == false)
                throw new ArgumentException();

            return new Vector3((Vector4)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Vector3 == false)
                throw new ArgumentException();

            return new Vector4((Vector3)value, 1);
        }
    }
}
