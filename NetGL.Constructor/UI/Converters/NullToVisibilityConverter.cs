using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NetGL.Constructor.UI.Converters {
    public class NullToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if( "inverse".Equals(parameter as string, StringComparison.InvariantCultureIgnoreCase))
                return value == null ? Visibility.Visible : Visibility.Collapsed;
            else
                return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
