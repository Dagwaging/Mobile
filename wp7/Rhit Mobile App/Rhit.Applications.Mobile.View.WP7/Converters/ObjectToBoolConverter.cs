using System;
using System.Globalization;
using System.Windows.Data;
using System.Collections;

namespace Rhit.Applications.Views.Converters {
    public class ObjectToBoolConverter : IValueConverter {
        public ObjectToBoolConverter() {
            Reverse = false;
        }

        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == null ? Reverse : !Reverse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
