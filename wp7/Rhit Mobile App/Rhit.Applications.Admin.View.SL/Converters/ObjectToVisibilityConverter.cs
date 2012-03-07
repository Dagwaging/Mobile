using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Collections;

namespace Rhit.Applications.Views.Converters {
    public class ObjectToVisibilityConverter : IValueConverter {
        public ObjectToVisibilityConverter() {
            NotNullValue = Visibility.Visible;
            NullValue = Visibility.Collapsed;
        }

        public Visibility NotNullValue { get; set; }
        public Visibility NullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value != null ? NotNullValue : NullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
