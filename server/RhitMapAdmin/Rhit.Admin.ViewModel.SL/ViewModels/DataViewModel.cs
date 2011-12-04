using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Rhit.Admin.Model;
using Rhit.Admin.Model.Events;
using Rhit.Admin.Model.Services;

namespace Rhit.Admin.ViewModel.ViewModels {
    public class DataViewModel : DependencyObject {
        public DataViewModel(Dispatcher dispatcher) {
            InitializeProperties();

            List<RhitLocation> locations = DataCollector.GetAllLocations();
            if(locations == null)
                DataCollector.Instance.UpdateAvailable += new ServiceEventHandler(OnLocationsRetrieved);
            else OnLocationsRetrieved(this, new ServiceEventArgs() { Locations = locations, });
        }

        private void InitializeProperties() {
            ZoomRange = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
            LocationTree = new ObservableCollection<LocationNode>();
            Locations = new Dictionary<int, RhitLocation>();

            LocationSelected = false;

            AltNames = new List<string>();
            Links = new ObservableCollection<Link>();
        }

        #region Properties
        public RhitLocation CurrentLocation { get; set; }

        public Dictionary<int, RhitLocation> Locations { get; set; }

        public ObservableCollection<Link> Links { get; set; }

        public ObservableCollection<LocationNode> LocationTree { get; set; }

        private Dictionary<int, LocationNode> TempDict { get; set; }
        #endregion

        #region Properties for Dependency Properties
        public string Name {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public List<string> AltNames {
            get { return (List<string>) GetValue(AltNamesProperty); }
            set { SetValue(AltNamesProperty, value); }
        }

        public int Id {
            get { return (int) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public int ParentId {
            get { return (int) GetValue(ParentIdProperty); }
            set { SetValue(ParentIdProperty, value); }
        }

        public string Description {
            get { return (string) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public List<int> ZoomRange {
            get { return (List<int>) GetValue(ZoomRangeProperty); }
            set { SetValue(ZoomRangeProperty, value); }
        }

        public bool LocationSelected {
            get { return (bool) GetValue(LocationSelectedProperty); }
            set { SetValue(LocationSelectedProperty, value); }
        }

        public bool IsDepartable {
            get { return (bool) GetValue(IsDepartableProperty); }
            set { SetValue(IsDepartableProperty, value); }
        }

        public bool IsPOI {
            get { return (bool) GetValue(IsPOIProperty); }
            set { SetValue(IsPOIProperty, value); }
        }

        public bool OnQuickList {
            get { return (bool) GetValue(OnQuickListProperty); }
            set { SetValue(OnQuickListProperty, value); }
        }

        public bool LabelOnHybrid {
            get { return (bool) GetValue(LabelOnHybridProperty); }
            set { SetValue(LabelOnHybridProperty, value); }
        }

        public int MinZoom {
            get { return (int) GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(DataViewModel), new PropertyMetadata(""));

        public static readonly DependencyProperty AltNamesProperty =
            DependencyProperty.Register("AltNames", typeof(List<string>), typeof(DataViewModel), new PropertyMetadata(new List<string>()));

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(DataViewModel), new PropertyMetadata(0));

        public static readonly DependencyProperty ParentIdProperty =
            DependencyProperty.Register("ParentId", typeof(int), typeof(DataViewModel), new PropertyMetadata(0));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(DataViewModel), new PropertyMetadata(""));

        public static readonly DependencyProperty ZoomRangeProperty =
            DependencyProperty.Register("ZoomRange", typeof(List<int>), typeof(DataViewModel), new PropertyMetadata(new List<int>()));

        public static readonly DependencyProperty LocationSelectedProperty =
            DependencyProperty.Register("LocationSelected", typeof(bool), typeof(DataViewModel), new PropertyMetadata(false));

        public static readonly DependencyProperty IsDepartableProperty =
            DependencyProperty.Register("IsDepartable", typeof(bool), typeof(DataViewModel), new PropertyMetadata(false));

        public static readonly DependencyProperty IsPOIProperty =
            DependencyProperty.Register("IsPOI", typeof(bool), typeof(DataViewModel), new PropertyMetadata(false));

        public static readonly DependencyProperty OnQuickListProperty =
            DependencyProperty.Register("OnQuickList", typeof(bool), typeof(DataViewModel), new PropertyMetadata(false));

        public static readonly DependencyProperty LabelOnHybridProperty =
            DependencyProperty.Register("LabelOnHybrid", typeof(bool), typeof(DataViewModel), new PropertyMetadata(false));

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(int), typeof(DataViewModel), new PropertyMetadata(0));
        #endregion

        private void OnLocationsRetrieved(object sender, ServiceEventArgs e) {
            foreach(RhitLocation location in e.Locations) {
                if(location.Id < 0) continue;
                if(Locations.ContainsKey(location.Id)) continue;
                Locations[location.Id] = location;
            }

            TempDict = new Dictionary<int, LocationNode>();

            foreach(RhitLocation location in e.Locations)
                AddNode(location);

            ChangeLocation(LocationTree[0] as object);
        }

        private void AddNode(RhitLocation location) {
            if(location.Id < 0) return;
            if(TempDict.ContainsKey(location.Id)) return;
            LocationNode node = new LocationNode() {
                Name = location.Label,
                Id = location.Id,
            };

            if(location.ParentId <= 0) {
                LocationTree.Add(node);
                TempDict[location.Id] = node;
                return;
            }

            if(TempDict.ContainsKey(location.ParentId)) {
                TempDict[location.ParentId].ChildLocations.Add(node);
                TempDict[location.Id] = node;
                return;
            }

            if(Locations.ContainsKey(location.ParentId)) {
                AddNode(Locations[location.ParentId]);
                TempDict[location.ParentId].ChildLocations.Add(node);
                TempDict[location.Id] = node;
                return;
            }

            LocationTree.Add(node);
            TempDict[location.Id] = node;
        }

        public void ChangeLocation(object selectedObject) {
            LocationNode node = selectedObject as LocationNode;
            if(!Locations.ContainsKey(node.Id)) return;
            CurrentLocation = Locations[node.Id];
            UpdateLocationData();
            LocationSelected = true;
        }

        private void UpdateLocationData() {
            Name = CurrentLocation.Label;
            AltNames = CurrentLocation.AltNames;
            Id = CurrentLocation.Id;
            Description = CurrentLocation.Description;
            ParentId = CurrentLocation.ParentId;

            IsPOI = CurrentLocation.IsPOI;
            IsDepartable = CurrentLocation.IsDepartable;
            OnQuickList = CurrentLocation.OnQuikList;
            LabelOnHybrid = CurrentLocation.LabelOnHybrid;
            MinZoom = CurrentLocation.MinZoomLevel;

            AltNames = new List<string>();
            if(CurrentLocation.AltNames.Count > 0)
                AltNames = CurrentLocation.AltNames;

            Links.Clear();
            if(CurrentLocation.Links.Count > 0)
                foreach(KeyValuePair<string, string> entry in CurrentLocation.Links)
                    Links.Add(new Link() { Name = entry.Key, Address = entry.Value, });
        }
    }

    #region Helper Classes
    public class LocationNode {
        public LocationNode() {
            ChildLocations = new ObservableCollection<LocationNode>();
        }

        public ObservableCollection<LocationNode> ChildLocations { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }
    }

    public class Link {
        public Link() { }

        public string Name { get; set; }

        public string Address { get; set; }
    }
    #endregion
}