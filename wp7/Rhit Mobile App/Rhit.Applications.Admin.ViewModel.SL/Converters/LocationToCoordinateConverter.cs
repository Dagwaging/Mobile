using System;
using System.Globalization;
using System.Windows.Data;
using Rhit.Applications.Model;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.ViewModel.Converters {
    public class LocationToCoordinateConverter : IValueConverter {
        public LocationToCoordinateConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null) return new GeoCoordinate(0, 0);
            return (value as ObservableRhitLocation).Center;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}