using System;
using System.Collections.ObjectModel;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;
using Rhit.Applications.ViewModel.Utilities;

namespace Rhit.Applications.ViewModel.Controllers {
    public class DynLocationsController : LocationsController {
        protected DynLocationsController() : base() {
            LocationTypes = new ObservableCollection<LocationType>() {
                LocationType.NormalLocation,
                LocationType.PointOfInterest,
                LocationType.OnQuickList,
                LocationType.Printer,
                LocationType.MenRestroom,
                LocationType.WomenRestroom,
                LocationType.UnisexRestroom,
            };
            
            DataCollector.Instance.LocationUpdate += new LocationEventHandler(LocationUpdate);
            DataCollector.Instance.LocationDeleted += new LocationEventHandler(LocationDeleted);
        }

        private void LocationDeleted(object sender, LocationEventArgs e) {
            int currentId = -1;
            if(CurrentLocation != null) currentId = CurrentLocation.Id;
            UnSelect();

            if(LocationDictionary.ContainsKey(e.Location.Id))
                All.Remove(LocationDictionary[e.Location.Id]);

            UpdateCollections();
            if(currentId != -1 && LocationDictionary.ContainsKey(currentId))
                SelectLocation(LocationDictionary[currentId]);
            OnLocationsChanged(new EventArgs());
        }

        private void LocationUpdate(object sender, LocationEventArgs e) {
            int currentId = -1;
            if(CurrentLocation != null) currentId = CurrentLocation.Id;
            UnSelect();

            if(LocationDictionary.ContainsKey(e.Location.Id))
                All.Remove(LocationDictionary[e.Location.Id]);
            All.Add(e.Location);

            UpdateCollections();
            if(currentId != -1 && LocationDictionary.ContainsKey(currentId))
                SelectLocation(LocationDictionary[currentId]);
            OnLocationsChanged(new EventArgs());
        }

        #region Singleton Instance
        protected static new DynLocationsController _instance;
        public static new DynLocationsController Instance {
            get {
                if(_instance == null)
                    _instance = new DynLocationsController();
                return _instance;
            }
        }
        #endregion

        public ObservableCollection<LocationType> LocationTypes { get; private set; }

        protected override void SetSelectedLocation(RhitLocation location) {
            CurrentLocation = new DynRhitLocation(location);
        }
    }
}
