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
        private static LocationsController _instance;

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
            UnSelect();

            if(LocationDictionary.ContainsKey(e.Location.Id))
                All.Remove(LocationDictionary[e.Location.Id]);

            UpdateCollections();
            OnLocationsChanged(new EventArgs());
        }

        private void LocationUpdate(object sender, LocationEventArgs e) {
            UnSelect();

            if(LocationDictionary.ContainsKey(e.Location.Id))
                All.Remove(LocationDictionary[e.Location.Id]);
            All.Add(e.Location);

            UpdateCollections();
            OnLocationsChanged(new EventArgs());
        }

        private void LocationsReturned(object sender, LocationsEventArgs e) {
            IList<RhitLocation> locations = e.Locations;
            if(locations == null || locations.Count <= 0) return;
            SetLocations(locations);
        }

        #region Singleton Instance
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

        #region Dependency Properties
        #region CurrentLocation
        public ObservableRhitLocation CurrentLocation {
            get { return (ObservableRhitLocation) GetValue(CurrentLocationProperty); }
            set { SetValue(CurrentLocationProperty, value); }
        }

        public static readonly DependencyProperty CurrentLocationProperty =
           DependencyProperty.Register("CurrentLocation", typeof(ObservableRhitLocation), typeof(LocationsController), new PropertyMetadata(null, new PropertyChangedCallback(OnCurrentLocationChanged)));

        private static void OnCurrentLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(e.NewValue == null) (d as LocationsController).IsCurrentLocation = false;
            else (d as LocationsController).IsCurrentLocation = true;
        }
        #endregion

        //TODO: Remove - Use CurrentLocation instead
        #region IsCurrentLocation
        public bool IsCurrentLocation {
            get { return (bool) GetValue(IsCurrentLocationProperty); }
            set { SetValue(IsCurrentLocationProperty, value); }
        }

        public static readonly DependencyProperty IsCurrentLocationProperty =
           DependencyProperty.Register("IsCurrentLocation", typeof(bool), typeof(LocationsController), new PropertyMetadata(false));
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

        public static readonly DependencyProperty TypesProperty =
            DependencyProperty.Register("Types", typeof(List<LocationType>), typeof(LocationsController), new PropertyMetadata(new List<LocationType>()));


        #endregion
    }

    public class ObservableRhitLocation : DependencyObject {
        public ObservableRhitLocation(RhitLocation location) {
            OriginalLocation = location;
            InitilizeData();
        }

        private void InitilizeData() {
            AltNames = new ObservableCollection<AlternateName>();
            foreach(string name in OriginalLocation.AltNames) AltNames.Add(new AlternateName(name));
            Corners = new ObservableCollection<Location>();
            foreach(Location location in OriginalLocation.Corners) Corners.Add(location);
            Links = new ObservableCollection<Link>();
            foreach(ILink link in OriginalLocation.Links)
                Links.Add(new Link() { Name = link.Name, Address = link.Address, });

            Center = OriginalLocation.Center;
            Description = OriginalLocation.Description;
            Floor = OriginalLocation.Floor;
            Id = OriginalLocation.Id;
            LabelOnHybrid = OriginalLocation.LabelOnHybrid;
            MinZoom = OriginalLocation.MinZoomLevel;
            Label = OriginalLocation.Label;
            ParentId = OriginalLocation.ParentId;
            Type = OriginalLocation.Type;

            AltNames.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            Corners.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            Links.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
        }

        public List<string> CheckChanges() {
            RhitLocation location = OriginalLocation;
            List<string> changes = new List<string>();
            if(Center != location.Center) changes.Add("Center");
            if(Description != location.Description) changes.Add("Description");
            if(Floor != location.Floor) changes.Add("Floor");
            if(Id != location.Id) changes.Add("Id");
            if(Label != location.Label) changes.Add("Label");
            if(LabelOnHybrid != location.LabelOnHybrid) changes.Add("LabelOnHybrid");
            if(MinZoom != location.MinZoomLevel) changes.Add("MinZoomLevel");
            if(ParentId != location.ParentId) changes.Add("ParentId");
            if(Type != location.Type) changes.Add("Type");

            if(Links.Count != location.Links.Count) changes.Add("Links");
            else {
                foreach(Link link in Links) {
                    if(location.Links.Contains(link)) continue;
                    changes.Add("Links");
                    break;
                }
            }

            if(AltNames.Count != location.AltNames.Count) changes.Add("AltNames");
            else {
                foreach(AlternateName altName in AltNames) {
                    if(location.AltNames.Contains(altName.Name)) continue;
                    changes.Add("AltNames");
                    break;
                }
            }

            if(changes.Count > 0) HasChanged = true;
            else HasChanged = false;
            return changes;
        }

        public RhitLocation OriginalLocation { get; private set; }

        public ObservableCollection<AlternateName> AltNames { get; set; }

        public ObservableCollection<Location> Corners { get; private set; }

        public ObservableCollection<Link> Links { get; private set; }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            CheckChanges();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            (d as ObservableRhitLocation).CheckChanges();
        }

        #region Description
        public string Description {
            get { return (string) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ObservableRhitLocation), new PropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Floor
        public int Floor {
            get { return (int) GetValue(FloorProperty); }
            set { SetValue(FloorProperty, value); }
        }

        public static readonly DependencyProperty FloorProperty =
            DependencyProperty.Register("Floor", typeof(int), typeof(ObservableRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Center
        public Location Center {
            get { return (Location) GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Location), typeof(ObservableRhitLocation), new PropertyMetadata(new Location(), new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region HasChanged
        public bool HasChanged {
            get { return (bool) GetValue(HasChangedProperty); }
            set { SetValue(HasChangedProperty, value); }
        }

        public static readonly DependencyProperty HasChangedProperty =
            DependencyProperty.Register("HasChanged", typeof(bool), typeof(ObservableRhitLocation), new PropertyMetadata(false));
        #endregion

        #region Id
        public int Id {
            get { return (int) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(ObservableRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Label
        public string Label {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
         DependencyProperty.Register("Label", typeof(string), typeof(ObservableRhitLocation), new PropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region LabelOnHybrid
        public bool LabelOnHybrid {
            get { return (bool) GetValue(LabelOnHybridProperty); }
            set { SetValue(LabelOnHybridProperty, value); }
        }

        public static readonly DependencyProperty LabelOnHybridProperty =
            DependencyProperty.Register("LabelOnHybrid", typeof(bool), typeof(ObservableRhitLocation), new PropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region MinZoom
        public int MinZoom {
            get { return (int) GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(int), typeof(ObservableRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region ParentId
        public int ParentId {
            get { return (int) GetValue(ParentIdProperty); }
            set { SetValue(ParentIdProperty, value); }
        }

        public static readonly DependencyProperty ParentIdProperty =
            DependencyProperty.Register("ParentId", typeof(int), typeof(ObservableRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Type
        public LocationType Type {
            get { return (LocationType) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(LocationType), typeof(ObservableRhitLocation), new PropertyMetadata(LocationType.NormalLocation, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion
    }

    public class AlternateName {
        public AlternateName() { Name = ""; }
        public AlternateName(string name) { Name = name; }
        public string Name { get; set; }
    }

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

    public class Link : DependencyObject, ILink {
        public Link() { }

        #region Name
        public string Name {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Link), new PropertyMetadata(""));
        #endregion

        #region Address
        public string Address {
            get { return (string) GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(Link), new PropertyMetadata(""));
        #endregion
    }
}
