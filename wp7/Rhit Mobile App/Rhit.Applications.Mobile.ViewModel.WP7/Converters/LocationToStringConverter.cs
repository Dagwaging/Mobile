using System;
using System.Device.Location;
using System.Globalization;
using System.Windows.Data;
using Rhit.Applications.Model;
using System.Collections.Generic;

namespace Rhit.Applications.ViewModel.Converters {
    public class LocationToStringConverter : IValueConverter {
        public LocationToStringConverter() {
            IsDescription = false;
            IsAltNames = false;
        }

        //TODO: think of a better way to handle this
        public bool IsDescription { get; set; }

        //TODO: think of a better way to handle this
        public bool IsAltNames { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null) return "No Location Found";
            if(IsDescription) return (value as RhitLocation).Description;
            if(IsAltNames) {
                List<string> altNames = (value as RhitLocation).AltNames;
                if(altNames == null || altNames.Count <= 0) return "";
                string names = "";
                foreach(string name in altNames) names += name + ',';
                names = names.Remove(names.Length - 1, 1);
            }
            return (value as RhitLocation).Label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}