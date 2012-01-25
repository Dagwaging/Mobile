using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Model;
using System;
using System.Collections.Generic;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;
using Microsoft.Maps.MapControl;
using System.Collections.Specialized;

namespace Rhit.Applications.ViewModel.Controllers {
    public class LocationsController : DependencyObject {
        private LocationsController() {
            LocationDictionary = new Dictionary<int, RhitLocation>();
            LocationTypes = new ObservableCollection<LocationType>() {
                LocationType.NormalLocation,
                LocationType.PointOfInterest,
                LocationType.OnQuickList,
                LocationType.Printer,
                LocationType.MenRestroom,
                LocationType.WomenRestroom,
                LocationType.UnisexRestroom,
            };
            All = new ObservableCollection<RhitLocation>();
            LocationTree = new ObservableCollection<LocationNode>();
            Top = new ObservableCollection<RhitLocation>();
            InnerLocations = new ObservableCollection<RhitLocation>();
            Buildings = new ObservableCollection<RhitLocation>();
            QuickList = new ObservableCollection<RhitLocation>();
            PointsOfInterest = new ObservableCollection<RhitLocation>();
            ShowAllBuildings = true;
            ShowSelectedBuilding = true;

            DataCollector.Instance.GetAllLocations();
            DataCollector.Instance.LocationsReturned += new LocationsEventHandler(LocationsReturned);
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

        private void LocationsReturned(object sender, LocationsEventArgs e) {
            IList<RhitLocation> locations = e.Locations;
            if(locations == null || locations.Count <= 0) return;
            SetLocations(locations);
        }

        #region Singleton Instance
        private static LocationsController _instance;
        public static LocationsController Instance {
            get {
                if(_instance == null)
                    _instance = new LocationsController();
                return _instance;
            }
        }
        #endregion

        #region Events
        #region CurrentLocationChanged
        public event EventHandler CurrentLocationChanged;
        protected virtual void OnCurrentLocationChanged(EventArgs e) {
            if(CurrentLocationChanged != null) CurrentLocationChanged(this, e);
        }
        #endregion

        #region LocationsChanged
        public event EventHandler LocationsChanged;
        protected virtual void OnLocationsChanged(EventArgs e) {
            if(LocationsChanged != null) LocationsChanged(this, e);
        }
        #endregion
        #endregion

        private void UpdateCollections() {
            PointsOfInterest.Clear();
            QuickList.Clear();
            Buildings.Clear();
            LocationDictionary.Clear();
            foreach(RhitLocation location in All) {
                if(location.Id < 0) continue;
                if(LocationDictionary.ContainsKey(location.Id)) continue;
                LocationDictionary[location.Id] = location;
                if(location.Type == LocationType.OnQuickList) {
                    QuickList.Add(location);
                    PointsOfInterest.Add(location);
                } else if(location.Type == LocationType.PointOfInterest)
                    PointsOfInterest.Add(location);
                if(location.ParentId == 0) Top.Add(location);
                if(location.Corners != null && location.Corners.Count > 0)
                    Buildings.Add(location);
            }
            UpdateTree();
        }

        private void UpdateTree() {
            LocationTree.Clear();
            LocationNodeDict = new Dictionary<int, LocationNode>();
            foreach(RhitLocation location in All) AddNode(location); 
        }

        private void AddNode(RhitLocation location) {
            if(LocationNodeDict.ContainsKey(location.Id)) return;

            LocationNode node = new LocationNode(location);
            if(location.ParentId <= 0) {
                LocationTree.Add(node);
                LocationNodeDict[location.Id] = node;
            } else if(LocationNodeDict.ContainsKey(location.ParentId)) {
                LocationNodeDict[location.ParentId].ChildLocations.Add(node);
                LocationNodeDict[location.Id] = node;
            } else if(LocationDictionary.ContainsKey(location.ParentId)) {
                AddNode(LocationDictionary[location.ParentId]);
                LocationNodeDict[location.ParentId].ChildLocations.Add(node);
                LocationNodeDict[location.Id] = node;
                return;
            } else {
                LocationTree.Add(node);
                LocationNodeDict[location.Id] = node;
            }
        }

        //Used for initializing the LocationTree
        private Dictionary<int, LocationNode> LocationNodeDict { get; set; }

        private Dictionary<int, RhitLocation> LocationDictionary { get; set; }

        #region Collections
        public ObservableCollection<RhitLocation> All { get; set; }

        public ObservableCollection<RhitLocation> Buildings { get; set; }

        public ObservableCollection<RhitLocation> InnerLocations { get; set; }

        public ObservableCollection<LocationNode> LocationTree { get; set; }

        public ObservableCollection<RhitLocation> PointsOfInterest { get; set; }

        public ObservableCollection<RhitLocation> QuickList { get; set; }

        public ObservableCollection<RhitLocation> Top { get; set; }
        #endregion

        public void SetLocations(ICollection<RhitLocation> locations) {
            UnSelect();
            All.Clear();
            foreach(RhitLocation location in locations) All.Add(location);
            UpdateCollections();
            OnLocationsChanged(new EventArgs());
        }

        #region SelectLocation Methods
        public void SelectLocation(int id) {
            if(LocationDictionary.ContainsKey(id))
                SelectLocation(LocationDictionary[id]);
        }

        public void SelectLocation(RhitLocation location) {
            if(!LocationDictionary.ContainsKey(location.Id)) return;
            CurrentLocation = new ObservableRhitLocation(location);

            InnerLocations.Clear();
            foreach(RhitLocation child in All)
                if(child.ParentId == CurrentLocation.Id)
                    InnerLocations.Add(child);

            OnCurrentLocationChanged(new EventArgs());
        }

        public void SelectLocation(LocationNode node) {
            SelectLocation(node.Location);
        }
        #endregion

        public void UnSelect() {
            CurrentLocation = null;
            InnerLocations.Clear();
            OnCurrentLocationChanged(new EventArgs());
        }

        public ObservableCollection<LocationType> LocationTypes { get; private set; }

        #region CurrentLocation
        public ObservableRhitLocation CurrentLocation {
            get { return (ObservableRhitLocation) GetValue(CurrentLocationProperty); }
            set { SetValue(CurrentLocationProperty, value); }
        }

        public static readonly DependencyProperty CurrentLocationProperty =
           DependencyProperty.Register("CurrentLocation", typeof(ObservableRhitLocation), typeof(LocationsController), new PropertyMetadata(null));
        #endregion

        #region ShowAllBuildings
        public bool ShowAllBuildings {
            get { return (bool) GetValue(ShowAllBuildingsProperty); }
            set { SetValue(ShowAllBuildingsProperty, value); }
        }

        public static readonly DependencyProperty ShowAllBuildingsProperty =
           DependencyProperty.Register("ShowAllBuildings", typeof(bool), typeof(LocationsController), new PropertyMetadata(false));
        #endregion

        #region ShowSelectedBuilding
        public bool ShowSelectedBuilding {
            get { return (bool) GetValue(ShowSelectedBuildingProperty); }
            set { SetValue(ShowSelectedBuildingProperty, value); }
        }

        public static readonly DependencyProperty ShowSelectedBuildingProperty =
           DependencyProperty.Register("ShowSelectedBuilding", typeof(bool), typeof(LocationsController), new PropertyMetadata(false));
        #endregion
    }
}
