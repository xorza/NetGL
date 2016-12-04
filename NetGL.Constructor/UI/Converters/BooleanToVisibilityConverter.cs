using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NetGL.Constructor.UI.Converters {
    public class BooleanToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if ("inverse".Equals(parameter as string, StringComparison.InvariantCultureIgnoreCase))
                return true.Equals(value) ? Visibility.Collapsed : Visibility.Visible;
            else
                return true.Equals(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
