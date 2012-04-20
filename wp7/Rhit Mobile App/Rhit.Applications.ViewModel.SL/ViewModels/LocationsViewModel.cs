using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Models.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModels.Controllers;
using Rhit.Applications.ViewModels.Providers;
using Rhit.Applications.ViewModels.Utilities;


namespace Rhit.Applications.ViewModels {
    public class LocationsViewModel : TaskedViewModel {
        private enum BehaviorState { Default, MovingCorners, CreatingCorners, Floor, AddingLocation, FloorAddingLocation, };

        protected override void Initialize() {
            base.Initialize();

            State = BehaviorState.Default;
        }

        protected override void InitializeCommands() {
            base.InitializeCommands();
            
            AddLocationCommand = new RelayCommand(p => AddLocation());
            AddCornersCommand = new RelayCommand(p => CreateCorners());
            ChangeCornersCommand = new RelayCommand(p => ShowCorners());
            LoadFloorCommand = new RelayCommand(p => LoadFloor());
        }

        #region Save Command/Methods
        private void SaveFloor() {
            ImageController.Instance.CloseImage();
            LocationPositionMapper.Instance.Save();
            LocationPositionMapper.Instance.Clear();
            Cancel();
        }

        protected override void Save() {


            switch(State) {

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
                case BehaviorState.Default:
                default:
                    Cancel();
                    break;
            }
        }

        private void SaveCorners() {
            DataCollector.Instance.ChangeLocationCorners(LocationsController.Instance.CurrentLocation.Id, LocationsProvider.GetLocations());
            Cancel();
        }

        private void SaveLocation() {
            if(State == BehaviorState.FloorAddingLocation) {
                IList<Location> locations = LocationsProvider.GetLocations();
                IList<Point> points = LocationsProvider.GetPoints();
                Location newLocation = null;
                if(locations.Count > 0) newLocation = locations[0];
                else if(points.Count > 0) newLocation = LocationPositionMapper.Instance.ConvertPositionToLocation(points[0]);
                if(newLocation != null) {
                    DataCollector.Instance.CreateLocation(LocationsProvider.Id, LocationsProvider.ParentId, LocationsProvider.Name, newLocation.Latitude, newLocation.Longitude, Mapper.Floor);
                }
            }

            if(State == BehaviorState.AddingLocation) {
                IList<Location> locations = LocationsProvider.GetLocations();
                if(locations.Count <= 0) return;
                Location newLocation = locations[0];
                DataCollector.Instance.CreateLocation(LocationsProvider.Id, LocationsProvider.ParentId, LocationsProvider.Name, newLocation.Latitude, newLocation.Longitude, 0);
            }

            Cancel();
        }
        #endregion

        #region Cancel Command/Method
        protected override void Cancel() {
            if(State == BehaviorState.FloorAddingLocation) {
                LocationsProvider.Clear();
                State = BehaviorState.Floor;
                return;
            }
            if(State == BehaviorState.Floor) ImageController.Instance.CloseImage();
            LocationsProvider.Clear();
            Settings.ShowBuildings = true;
            State = BehaviorState.Default;
            Mapper.Locations.Clear();
        }
        #endregion

        #region Add Location Command/Method
        public ICommand AddLocationCommand { get; private set; }

        private void AddLocation() {
            if(State == BehaviorState.Floor) {
                State = BehaviorState.FloorAddingLocation;
                LocationsProvider.QueryLocation();
            } else {
                Settings.ShowBuildings = false;
                State = BehaviorState.AddingLocation;
                LocationsProvider.QueryLocation();
            }
        }
        #endregion

        #region Add Corners Command/Method
        public ICommand AddCornersCommand { get; private set; }

        private void CreateCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            State = BehaviorState.CreatingCorners;
            Settings.ShowBuildings = false;
            LocationsProvider.CreateNewCorners();
            ShowSave = true;
        }
        #endregion

        #region Change Corners Command/Method
        public ICommand ChangeCornersCommand { get; private set; }

        private void ShowCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            State = BehaviorState.MovingCorners;
            Settings.ShowBuildings = false;
            LocationsProvider.DisplayCorners(LocationsController.Instance.CurrentLocation.Corners as ICollection<Location>);
            ShowSave = true;
        }
        #endregion

        #region Load Floor Command/Method
        public ICommand LoadFloorCommand { get; private set; }

        private void LoadFloor() {
            ShowSave = false;
            if(LocationsController.Instance.CurrentLocation == null) return;
            State = BehaviorState.Floor;
            Settings.ShowBuildings = false;
            ShowFloorLocations = true;
            ImageController.Instance.LoadImage();
        }
        #endregion

        private BehaviorState State { get; set; }

        public override void SelectLocation(int id) {
            LocationsController.Instance.SelectLocation(id);
        }

        #region ShowInnerLocations
        public bool ShowInnerLocations {
            get { return (bool) GetValue(ShowInnerLocationsProperty); }
            set { SetValue(ShowInnerLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowInnerLocationsProperty =
           DependencyProperty.Register("ShowInnerLocations", typeof(bool), typeof(TaskedViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowFloorLocations
        public bool ShowFloorLocations {
            get { return (bool) GetValue(ShowFloorLocationsProperty); }
            set { SetValue(ShowFloorLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowFloorLocationsProperty =
           DependencyProperty.Register("ShowFloorLocations", typeof(bool), typeof(TaskedViewModel), new PropertyMetadata(false));
        #endregion
    }
}
