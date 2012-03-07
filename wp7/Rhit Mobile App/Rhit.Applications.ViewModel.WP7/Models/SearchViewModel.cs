using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Models;
using Rhit.Applications.Models.Events;
using Rhit.Applications.Models.Services;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.ViewModels {
    public class SearchViewModel : DependencyObject {
        public SearchViewModel() {
            DataCollector.Instance.SearchResultsReturned += new LocationsEventHandler(LocationsReturned);
            Locations = LocationsController.Instance;
            People = new ObservableCollection<object>();
            Places = new ObservableCollection<RhitLocation>();
        }

        void LocationsReturned(object sender, LocationsEventArgs e) {
            Places.Clear();
            foreach(RhitLocation location in e.Locations)
                Places.Add(location);
        }

        public LocationsController Locations { get; set; }

        public ObservableCollection<object> People { get; set; }

        public ObservableCollection<RhitLocation> Places { get; set; }

        public void Search(string searchString) {
            DataCollector.Instance.SearchLocations(Dispatcher, searchString);
        }

        public void SelectLocation(object obj) {
            if(obj is RhitLocation) Locations.SelectLocation(obj as RhitLocation);
        }

    }
}
