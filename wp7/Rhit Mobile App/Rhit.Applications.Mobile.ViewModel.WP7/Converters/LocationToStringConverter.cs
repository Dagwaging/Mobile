using System;
using System.Device.Location;
using System.Globalization;
using System.Windows.Data;
using Rhit.Applications.Model;

namespace Rhit.Applications.ViewModel.Converters {
    public class LocationToStringConverter : IValueConverter {
        public LocationToStringConverter() {
            IsDescription = false;
        }

        public bool IsDescription { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null) return new GeoCoordinate(0, 0);
            if(IsDescription) return (value as RhitLocation).Description;
            return (value as RhitLocation).Label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}