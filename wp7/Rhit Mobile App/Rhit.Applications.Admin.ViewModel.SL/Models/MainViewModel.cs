using System.Windows;
using Rhit.Applications.Model;
using Rhit.Applications.ViewModel.Controllers;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Model.Events;
using System.Collections.Generic;
using System.Windows.Input;
using Rhit.Applications.Mvvm.Commands;
using System.Collections.ObjectModel;
using Rhit.Applications.ViewModel.Providers;

namespace Rhit.Applications.ViewModel.Models {
    public class MainViewModel : DependencyObject {
        private enum BehaviorState { Default, MovingCorners, CreatingCorners, Floor };

        //NOTE: Requires a call to Initialize before class is usable
        //Note: NoArg Constructor so ViewModel can be created in xaml
        public MainViewModel() { }

        public void Initialize(Map map, IBuildingMappingProvider buildingMappingProvider,
            IBuildingCornersProvider cornerProvider, IBitmapProvider imageProvider) {

            Locations = LocationsController.Instance;

            CornersProvider = cornerProvider;
            LocationsController.Instance.CurrentLocationChanged += new LocationEventHandler(CurrentLocationChanged);
            LocationsController.Instance.LocationsChanged += new LocationEventHandler(LocationsChanged);
            
            SaveCommand = new RelayCommand(p => Save());
            CancelCommand = new RelayCommand(p => Cancel());

            AddCornersCommand = new RelayCommand(p => CreateCorners());
            ChangeCornersCommand = new RelayCommand(p => ShowCorners());
            LoadFloorCommand = new RelayCommand(p => LoadFloor());
            State = BehaviorState.Default;

            ShowBuildings = true;

            
            MapController.CreateMapController(map);
            ImageController.CreateImageController(imageProvider, buildingMappingProvider);
            Image = ImageController.Instance;
            Map = MapController.Instance;
            GotoRhitCommand = new RelayCommand(p => GotoRhit());

            List<RhitLocation> locations = DataCollector.Instance.GetAllLocations(null);
            if(locations == null || locations.Count <= 0)
                DataCollector.Instance.UpdateAvailable += new ServiceEventHandler(OnLocationsRetrieved);
            else OnLocationsRetrieved(this, new ServiceEventArgs());
            Mapper = LocationPositionMapper.Instance;
        }

        public MainViewModel(Map map, IBuildingMappingProvider buildingMappingProvider,
            IBuildingCornersProvider cornerProvider, IBitmapProvider imageProvider) {
                Initialize(map, buildingMappingProvider, cornerProvider, imageProvider);
        }

        private IBuildingCornersProvider CornersProvider { get; set; }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public ICommand AddCornersCommand { get; private set; }

        public ICommand ChangeCornersCommand { get; private set; }

        public ICommand LoadFloorCommand { get; private set; }

        private BehaviorState State { get; set; }


        private void CreateCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            State = BehaviorState.CreatingCorners;
            ShowBuildings = false;
            CornersProvider.CreateNewCorners();
            ShowSave = true;
        }

        private void ShowCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            State = BehaviorState.MovingCorners;
            ShowBuildings = false;
            CornersProvider.DisplayCorners(LocationsController.Instance.CurrentLocation.Corners as ICollection<Location>);
            ShowSave = true;
        }

        private void SaveCorners() {
            var corners = CornersProvider.GetCorners();

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
            }
        }

        private void Cancel() {
            if(State == BehaviorState.Floor) ImageController.Instance.CloseImage();
            CornersProvider.ClearCorners();
            ShowBuildings = true;
            ShowSave = false;
            State = BehaviorState.Default;
            Mapper.Locations.Clear();
        }

        private void LocationsChanged(object sender, LocationEventArgs e) {
            foreach(RhitLocation location in e.NewLocations) {
                if(location.Corners == null || location.Corners.Count <= 0) continue;
                LocationsController.Instance.AddBuilding(location);
            }
        }

        private void CurrentLocationChanged(object sender, LocationEventArgs e) {
            //if(!ShowBuildings) {
            //    if(e.OldLocation != null)
            //        LocationsController.Instance.RemoveBuilding(e.OldLocation);
            //    if(e.NewLocation != null)
            //        LocationsController.Instance.AddBuilding(e.NewLocation);
            //}
            if(e.NewLocation != null) MapController.Instance.MapControl.Center = e.NewLocation.Center;
        }

        private void OnLocationsRetrieved(object sender, ServiceEventArgs e) {
            List<RhitLocation> locations = DataCollector.Instance.GetAllLocations(null);
            if(locations == null || locations.Count <= 0) return;
            Locations.SetLocations(locations);
        }


        #region Dependency Properties
        #region ShowBuildings
        public bool ShowBuildings {
            get { return (bool) GetValue(ShowBuildingsProperty); }
            set { SetValue(ShowBuildingsProperty, value); }
        }

        public static readonly DependencyProperty ShowBuildingsProperty =
           DependencyProperty.Register("ShowBuildings", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowAllLocations
        public bool ShowAllLocations {
            get { return (bool) GetValue(ShowAllLocationsProperty); }
            set { SetValue(ShowAllLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowAllLocationsProperty =
           DependencyProperty.Register("ShowAllLocations", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowInnerLocations
        public bool ShowInnerLocations {
            get { return (bool) GetValue(ShowInnerLocationsProperty); }
            set { SetValue(ShowInnerLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowInnerLocationsProperty =
           DependencyProperty.Register("ShowInnerLocations", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowTopLocations
        public bool ShowTopLocations {
            get { return (bool) GetValue(ShowTopLocationsProperty); }
            set { SetValue(ShowTopLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowTopLocationsProperty =
           DependencyProperty.Register("ShowTopLocations", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowFloorLocations
        public bool ShowFloorLocations {
            get { return (bool) GetValue(ShowFloorLocationsProperty); }
            set { SetValue(ShowFloorLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowFloorLocationsProperty =
           DependencyProperty.Register("ShowFloorLocations", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowSave
        public bool ShowSave {
            get { return (bool) GetValue(ShowSaveProperty); }
            set { SetValue(ShowSaveProperty, value); }
        }

        public static readonly DependencyProperty ShowSaveProperty =
           DependencyProperty.Register("ShowSave", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));
        #endregion
        #endregion


        public ICommand GotoRhitCommand { get; private set; }

        public ImageController Image { get; private set; }

        public LocationsController Locations { get; private set; }

        public LocationPositionMapper Mapper { get; set; }

        public MapController Map { get; private set; }

        public void GotoRhit() {
            //TODO: Don't hard code
            Map.MapControl.Center = new GeoCoordinate(39.4820263, -87.3248677);
            Map.MapControl.ZoomLevel = 16;
        }

        public void SelectLocation(int id, bool show) {
            LocationsController.Instance.SelectLocation(id);
            if(!show) LocationsController.Instance.ShowCurrent = false;
        }

        public void SelectLocation(Location coordinate) {
            Locations.SelectLocation(new GeoCoordinate(coordinate));
        }
    }
}
