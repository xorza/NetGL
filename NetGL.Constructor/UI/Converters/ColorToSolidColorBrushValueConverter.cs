using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NetGL.Constructor.UI.Converters
{
    public class ColorToSolidColorBrushValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
                return null;

            // For a more sophisticated converter, check also the targetType and react accordingly..
            if (value is Color)
                return new SolidColorBrush((Color)value);

            // You can support here more source types if you wish
            // For the example I throw an exception
            Type type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If necessary, here you can convert back. Check if which brush it is (if its one),
            // get its Color-value and return it.

            throw new NotImplementedException();
        }
    }
}
