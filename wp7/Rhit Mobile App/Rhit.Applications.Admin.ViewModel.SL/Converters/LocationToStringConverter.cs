using System;
using System.Globalization;
using System.Windows.Data;
using Rhit.Applications.Model;
using System.Collections.Generic;
using Rhit.Applications.ViewModel.Controllers;

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
            if(value == null) return "No Location Selected";
            if(IsDescription) return (value as ObservableRhitLocation).Description;
            if(IsAltNames) {
                IList<string> altNames = new List<string>();
                foreach(AlternateName altName in (value as ObservableRhitLocation).AltNames)
                    altNames.Add(altName.Name);
                if(altNames == null || altNames.Count <= 0) return "";
                string names = "";
                foreach(string name in altNames) names += name + ',';
                names = names.Remove(names.Length - 1, 1);
            }
            return (value as ObservableRhitLocation).Label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}