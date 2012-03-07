﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Rhit.Applications.Views.Converters {
    public class RadioButtonConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (value.ToString() == parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return (bool) value ? Enum.Parse(targetType, parameter.ToString(), true) : null;
        }
    }
}
