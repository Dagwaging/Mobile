using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rhit.Applications.View.Converters {
    public class BoolToGridSizeConverter : IValueConverter {
        public BoolToGridSizeConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool val = System.Convert.ToBoolean(value);
            if(val) return new GridLength(1, GridUnitType.Star);
            return new GridLength(0, GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if((GridLength) value == new GridLength(0, GridUnitType.Star)) return false;
            return false;
        }
    }
}