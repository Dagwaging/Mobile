using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Services;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.ViewModel.Models {
    public class SearchViewModel : DependencyObject {
        public SearchViewModel() {
            //DataCollector.Instance.SearchResultsAvailable += new SearchEventHandler(SearchResultsAvailable);
            Locations = LocationsController.Instance;
            People = new ObservableCollection<object>();
            Places = new ObservableCollection<RhitLocation>();
        }

        //private void SearchResultsAvailable(object sender, SearchEventArgs e) {
        //    People.Clear();
        //    Places.Clear();
        //    foreach(RhitLocation location in e.Places)
        //        Places.Add(location);
        //}

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
