using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Controllers;
using Rhit.Applications.ViewModel.Providers;

namespace Rhit.Applications.ViewModel.Models {
    public class MapViewModel : DependencyObject {
        private enum BehaviorState { Default, MovingCorners, CreatingCorners, Floor, AddingLocation, FloorAddingLocation };

        //NOTE: Requires a call to Initialize before class is usable
        //Note: NoArg Constructor so ViewModel can be created in xaml
        public MapViewModel() {}

        public void Initialize(IBuildingMappingProvider buildingMappingProvider,
            ILocationsProvider locationsProvider, IBitmapProvider imageProvider) {

            Locations = LocationsController.Instance;

            LocationsProvider = locationsProvider;
            LocationsController.Instance.CurrentLocationChanged += new LocationEventHandler(CurrentLocationChanged);

            SaveCommand = new RelayCommand(p => Save());
            CancelCommand = new RelayCommand(p => Cancel());

            AddLocationCommand = new RelayCommand(p => AddLocation());
            AddCornersCommand = new RelayCommand(p => CreateCorners());
            ChangeCornersCommand = new RelayCommand(p => ShowCorners());
            LoadFloorCommand = new RelayCommand(p => LoadFloor());
            State = BehaviorState.Default;

            ShowBuildings = true;

            ImageController.CreateImageController(imageProvider, buildingMappingProvider);
            Image = ImageController.Instance;
            Map = MapController.Instance;

            Mapper = LocationPositionMapper.Instance;
        }

        public void SetMode(Map map) {
            map.Mode = Map.CurrentMode;

            List<UIElement> es = new List<UIElement>();
            foreach(UIElement e in map.Children) es.Add(e);
            map.Children.Clear();

            map.Children.Add(Map.TileLayer);

            //Re-add elements put onto the map in the view
            foreach(UIElement e in es) map.Children.Add(e);
            Map.ZoomLevel = 16;
            if(Locations.CurrentLocation != null)
                Map.Center = Locations.CurrentLocation.Center;
        }

        private void CurrentLocationChanged(object sender, LocationEventArgs e) {
            if(e.NewLocation != null) Map.Center = e.NewLocation.Center;
        }


        #region Commands
        public ICommand SaveCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        public ICommand AddLocationCommand { get; private set; }

        public ICommand AddCornersCommand { get; private set; }

        public ICommand ChangeCornersCommand { get; private set; }

        public ICommand LoadFloorCommand { get; private set; }
        #endregion


        private BehaviorState State { get; set; }

        private ILocationsProvider LocationsProvider { get; set; }

        public MapController Map { get; private set; }

        public ImageController Image { get; private set; }

        public LocationsController Locations { get; private set; }

        public LocationPositionMapper Mapper { get; set; }


        #region Dependency Properties
        #region ShowBuildings
        public bool ShowBuildings {
            get { return (bool) GetValue(ShowBuildingsProperty); }
            set { SetValue(ShowBuildingsProperty, value); }
        }

        public static readonly DependencyProperty ShowBuildingsProperty =
           DependencyProperty.Register("ShowBuildings", typeof(bool), typeof(MapViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowAllLocations
        public bool ShowAllLocations {
            get { return (bool) GetValue(ShowAllLocationsProperty); }
            set { SetValue(ShowAllLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowAllLocationsProperty =
           DependencyProperty.Register("ShowAllLocations", typeof(bool), typeof(MapViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowInnerLocations
        public bool ShowInnerLocations {
            get { return (bool) GetValue(ShowInnerLocationsProperty); }
            set { SetValue(ShowInnerLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowInnerLocationsProperty =
           DependencyProperty.Register("ShowInnerLocations", typeof(bool), typeof(MapViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowTopLocations
        public bool ShowTopLocations {
            get { return (bool) GetValue(ShowTopLocationsProperty); }
            set { SetValue(ShowTopLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowTopLocationsProperty =
           DependencyProperty.Register("ShowTopLocations", typeof(bool), typeof(MapViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowFloorLocations
        public bool ShowFloorLocations {
            get { return (bool) GetValue(ShowFloorLocationsProperty); }
            set { SetValue(ShowFloorLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowFloorLocationsProperty =
           DependencyProperty.Register("ShowFloorLocations", typeof(bool), typeof(MapViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowSave
        public bool ShowSave {
            get { return (bool) GetValue(ShowSaveProperty); }
            set { SetValue(ShowSaveProperty, value); }
        }

        public static readonly DependencyProperty ShowSaveProperty =
           DependencyProperty.Register("ShowSave", typeof(bool), typeof(MapViewModel), new PropertyMetadata(false));
        #endregion
        #endregion


        private void AddLocation() {
            if(State == BehaviorState.Floor) {
                State = BehaviorState.FloorAddingLocation;
                LocationsProvider.QueryLocation();
            } else {
                ShowBuildings = false;
                State = BehaviorState.AddingLocation;
                LocationsProvider.QueryLocation();
            }
        }

        private void CreateCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            State = BehaviorState.CreatingCorners;
            ShowBuildings = false;
            LocationsProvider.CreateNewCorners();
            ShowSave = true;
        }

        private void ShowCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            State = BehaviorState.MovingCorners;
            ShowBuildings = false;
            LocationsProvider.DisplayCorners(LocationsController.Instance.CurrentLocation.Corners as ICollection<Location>);
            ShowSave = true;
        }

        private void SaveCorners() {
            var corners = LocationsProvider.GetLocations();

            var executions = new List<KeyValuePair<string, Dictionary<string, object>>>() {
                new KeyValuePair<string, Dictionary<string, object>>("spDeleteMapAreaCorners", new Dictionary<string, object>() {
                    { "location", LocationsController.Instance.CurrentLocation.Id }
                })
            };

            foreach(var corner in corners) {
                executions.Add(new KeyValuePair<string, Dictionary<string, object>>("spAddMapAreaCorner", new Dictionary<string, object>() {
                    { "location", LocationsController.Instance.CurrentLocation.Id },
                    { "lat", corner.Latitude },
                    { "lon", corner.Longitude }
                }));
            }

            DataCollector.Instance.ExecuteBatchStoredProcedure(Dispatcher, executions);
            Cancel();
        }

        private void LoadFloor() {
            ShowSave = false;
            if(LocationsController.Instance.CurrentLocation == null) return;
            State = BehaviorState.Floor;
            ShowBuildings = false;
            ShowFloorLocations = true;
            ImageController.Instance.LoadImage();
        }

        private void SaveFloor() {
            ImageController.Instance.CloseImage();
            LocationPositionMapper.Instance.Save();
            LocationPositionMapper.Instance.Clear();
            Cancel();
        }

        private void Save() {
            switch(State) {
                case BehaviorState.Default:
                    Cancel();
                    break;
                case BehaviorState.MovingCorners:
                    SaveCorners();
                    break;
                case BehaviorState.CreatingCorners:
                    SaveCorners();
                    break;
                case BehaviorState.Floor:
                    SaveFloor();
                    break;
                case BehaviorState.AddingLocation:
                    SaveLocation();
                    break;
                case BehaviorState.FloorAddingLocation:
                    SaveLocation();
                    break;
            }
        }

        private void SaveLocation() {
            if(State == BehaviorState.FloorAddingLocation) {
                IList<Location> locations = LocationsProvider.GetLocations();
                //TODO: Scott - Use LocationsProvider.ParentId and LocationsProvider.Name
                IList<Point> points = LocationsProvider.GetPoints();
                Location newLocation = null;
                if(locations.Count > 0) newLocation = locations[0];
                else if(points.Count > 0) newLocation = LocationPositionMapper.Instance.ConvertPositionToLocation(points[0]);
                if(newLocation != null) {
                    DataCollector.Instance.ExecuteStoredProcedure(Dispatcher, "spCreateLocation", new Dictionary<string, object>() {
                        { "id", LocationsProvider.Id }, 
                        { "name", LocationsProvider.Name },
                        { "lat", newLocation.Latitude },
                        { "lon", newLocation.Longitude },
                        { "floor", Mapper.Floor },
                        { "parent", LocationsProvider.ParentId },
                    });
                }
            }

            if(State == BehaviorState.AddingLocation) {
                IList<Location> locations = LocationsProvider.GetLocations();
                //TODO: Scott - Use LocationsProvider.ParentId and LocationsProvider.Name
                if(locations.Count <= 0) return;
                Location newLocation = locations[0];
                DataCollector.Instance.ExecuteStoredProcedure(Dispatcher, "spCreateLocation", new Dictionary<string, object>() {
                    { "id", LocationsProvider.Id }, 
                    { "name", LocationsProvider.Name },
                    { "lat", newLocation.Latitude },
                    { "lon", newLocation.Longitude },
                    { "floor", 0 },
                    { "parent", LocationsProvider.ParentId },
                });
            }

            Cancel();
        }

        private void Cancel() {
            if(State == BehaviorState.FloorAddingLocation) {
                LocationsProvider.Clear();
                State = BehaviorState.Floor;
                return;
            }
            if(State == BehaviorState.Floor) ImageController.Instance.CloseImage();
            LocationsProvider.Clear();
            ShowBuildings = true;
            State = BehaviorState.Default;
            Mapper.Locations.Clear();
        }


        public void SelectLocation(int id) {
            LocationsController.Instance.SelectLocation(id);
        }
    }
}
