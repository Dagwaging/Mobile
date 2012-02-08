using System;
using System.Globalization;
using System.Windows.Data;
using Rhit.Applications.Model;
using System.Collections.Generic;

namespace Rhit.Applications.ViewModel.Converters {
    public class LocationToLinkConverter : IValueConverter {
        public LocationToLinkConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null) return null;
            Dictionary<string, string> linkDict = (value as RhitLocation).Links;
            if(linkDict == null || linkDict.Count <= 0) return null;
            List<Link> links = new List<Link>();
            foreach(string name in linkDict.Keys)
                links.Add(new Link() { Name=name, Address=linkDict[name], });
            return links;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class Link {
        public Link() { }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}