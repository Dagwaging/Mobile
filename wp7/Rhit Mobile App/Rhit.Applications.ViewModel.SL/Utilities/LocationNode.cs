using System.Collections.ObjectModel;
using Rhit.Applications.Models;

namespace Rhit.Applications.ViewModels.Utilities {
    public class LocationNode {
        public LocationNode(LocationData location) {
            ChildLocations = new ObservableCollection<LocationNode>();
            Location = location;
            Name = Location.Label;
            Id = Location.Id;
        }

        public LocationData Location { get; private set; }

        public ObservableCollection<LocationNode> ChildLocations { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }
    }
}
