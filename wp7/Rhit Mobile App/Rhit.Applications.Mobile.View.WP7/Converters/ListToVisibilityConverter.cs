using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rhit.Applications.View.Converters {
    public class ListToVisibilityConverter : IValueConverter {
        public ListToVisibilityConverter() {
            NotNullValue = Visibility.Visible;
            NullValue = Visibility.Collapsed;
        }

        public Visibility NotNullValue { get; set; }
        public Visibility NullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null) return NullValue;
            if(value is IList) {
                IList list = value as IList;
                return list.Count <= 0 ? NullValue : NotNullValue;
            } else return NullValue;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
