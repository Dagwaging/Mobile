using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Models;
using Rhit.Applications.Models.Events;
using Rhit.Applications.Models.Services;
using Rhit.Applications.ViewModels.Utilities;

namespace Rhit.Applications.ViewModels.Controllers {
    public class LocationsController : DependencyObject {
        protected LocationsController() {
            InitCollections();
            InitLocationTypes();
            BuildingsHidden = false;

            DataCollector.Instance.LocationUpdate += new LocationEventHandler(LocationUpdate);
            DataCollector.Instance.LocationDeleted += new LocationEventHandler(LocationDeleted);
            DataCollector.Instance.LocationsReturned += new LocationsEventHandler(LocationsReturned);
            DataCollector.Instance.GetAllLocations();
        }

        private void InitCollections() {
            LocationDictionary = new Dictionary<int, LocationData>();
            All = new ObservableCollection<LocationData>();
            LocationTree = new ObservableCollection<LocationNode>();
            Top = new ObservableCollection<LocationData>();
            InnerLocations = new ObservableCollection<LocationData>();
            Buildings = new ObservableCollection<LocationData>();
            HiddenBuildings = new ObservableCollection<LocationData>();
            QuickList = new ObservableCollection<LocationData>();
            PointsOfInterest = new ObservableCollection<LocationData>();
        }

        private void InitLocationTypes() {
            LocationTypes = new ObservableCollection<LocationType>() {
                LocationType.NormalLocation,
                LocationType.PointOfInterest,
                LocationType.OnQuickList,
                LocationType.Printer,
                LocationType.MenRestroom,
                LocationType.WomenRestroom,
                LocationType.UnisexRestroom,
            };
        }

        private void LocationsReturned(object sender, LocationsEventArgs e) {
            IList<LocationData> locations = e.Locations;
            if(locations == null || locations.Count <= 0) return;
            SetLocations(locations);
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
        protected static LocationsController _instance;
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

        protected void UpdateCollections() {
            PointsOfInterest.Clear();
            QuickList.Clear();
            Buildings.Clear();
            LocationDictionary.Clear();
            foreach(LocationData location in All) {
                if(location.Id < 0) continue;
                if(LocationDictionary.ContainsKey(location.Id)) continue;
                LocationDictionary[location.Id] = location;
                if(location.Type == LocationType.OnQuickList) {
                    QuickList.Add(location);
                    PointsOfInterest.Add(location);
                } else if(location.Type == LocationType.PointOfInterest)
                    PointsOfInterest.Add(location);
                if(location.ParentId == 0) Top.Add(location);
                if(location.Corners != null && location.Corners.Count > 0) {
                    if(BuildingsHidden) HiddenBuildings.Add(location);
                    else Buildings.Add(location);
                }
                    
            }
            UpdateTree();
        }

        internal void HideBuildings() {
            Buildings.Clear();
            HiddenBuildings.Clear();
            foreach(LocationData location in All)
                if(location.Corners != null && location.Corners.Count > 0)
                    HiddenBuildings.Add(location);
            if(CurrentLocation != null)
                Buildings.Add(CurrentLocation.OriginalLocation);
            BuildingsHidden = true;
        }

        internal void ShowBuildings() {
            Buildings.Clear();
            HiddenBuildings.Clear();
            foreach(LocationData location in All)
                if(location.Corners != null && location.Corners.Count > 0)
                    Buildings.Add(location);
            BuildingsHidden = false;
        }

        private bool BuildingsHidden { get; set; }

        private void UpdateTree() {
            LocationTree.Clear();
            LocationNodeDict = new Dictionary<int, LocationNode>();
            foreach(LocationData location in All) AddNode(location); 
        }

        private void AddNode(LocationData location) {
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

        protected Dictionary<int, LocationData> LocationDictionary { get; set; }

        #region Collections
        public ObservableCollection<LocationData> All { get; set; }

        public ObservableCollection<LocationData> Buildings { get; set; }

        public ObservableCollection<LocationData> HiddenBuildings { get; set; }

        public ObservableCollection<LocationData> InnerLocations { get; set; }

        public ObservableCollection<LocationNode> LocationTree { get; set; }

        public ObservableCollection<LocationData> PointsOfInterest { get; set; }

        public ObservableCollection<LocationData> QuickList { get; set; }

        public ObservableCollection<LocationData> Top { get; set; }
        #endregion

        public void SetLocations(ICollection<LocationData> locations) {
            UnSelect();
            All.Clear();
            foreach(LocationData location in locations) All.Add(location);
            UpdateCollections();
            OnLocationsChanged(new EventArgs());
        }

        #region SelectLocation Methods
        protected void SetSelectedLocation(LocationData location) {
            //CurrentLocation = new SimpleRhitLocation(location);
            CurrentLocation = new RhitLocation(location);
        }

        public void SelectLocation(int id) {
            if(LocationDictionary.ContainsKey(id))
                SelectLocation(LocationDictionary[id]);
        }

        public void SelectLocation(LocationData location) {
            if(!LocationDictionary.ContainsKey(location.Id)) return;
            SetSelectedLocation(location);

            InnerLocations.Clear();
            foreach(LocationData child in All)
                if(child.ParentId == CurrentLocation.Id)
                    InnerLocations.Add(child);

            if(BuildingsHidden) {
                Buildings.Clear();
                Buildings.Add(CurrentLocation.OriginalLocation);
            }

            OnCurrentLocationChanged(new EventArgs());
        }

        public void SelectLocation(LocationNode node) {
            SelectLocation(node.Location);
        }
        #endregion

        public void UnSelect() {
            CurrentLocation = null;
            InnerLocations.Clear();
            if(BuildingsHidden) Buildings.Clear();
            OnCurrentLocationChanged(new EventArgs());
        }

        public ObservableCollection<LocationType> LocationTypes { get; private set; }

        #region CurrentLocation
        public SimpleRhitLocation CurrentLocation {
            get { return (SimpleRhitLocation) GetValue(CurrentLocationProperty); }
            set { SetValue(CurrentLocationProperty, value); }
        }

        public static readonly DependencyProperty CurrentLocationProperty =
           DependencyProperty.Register("CurrentLocation", typeof(SimpleRhitLocation), typeof(LocationsController), new PropertyMetadata(null));
        #endregion
    }
}
