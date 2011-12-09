using System;
using System.Device.Location;
using System.Globalization;
using System.Windows.Data;
using Rhit.Applications.Model;

namespace Rhit.Applications.ViewModel.Converters {
    public class LocationToCoordinateConverter : IValueConverter {
        public LocationToCoordinateConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null) return new GeoCoordinate(0, 0);
            return (value as RhitLocation).Center;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}