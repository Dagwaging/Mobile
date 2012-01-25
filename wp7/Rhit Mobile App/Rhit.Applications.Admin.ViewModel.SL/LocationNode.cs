using System.Collections.ObjectModel;
using Rhit.Applications.Model;

namespace Rhit.Applications.ViewModel {
    public class LocationNode {
        public LocationNode(RhitLocation location) {
            ChildLocations = new ObservableCollection<LocationNode>();
            Location = location;
            Name = Location.Label;
            Id = Location.Id;
        }

        public RhitLocation Location { get; private set; }

        public ObservableCollection<LocationNode> ChildLocations { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }
    }
}
