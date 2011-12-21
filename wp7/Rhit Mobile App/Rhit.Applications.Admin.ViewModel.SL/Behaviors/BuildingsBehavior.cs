using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Services;
using Rhit.Applications.ViewModel.Controllers;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Providers;

namespace Rhit.Applications.ViewModel.Behaviors {
    public class BuildingsBehavior : MapBehavior {

        private enum BehaviorState { Default, MovingCorners, CreatingCorners, Floor };

        public BuildingsBehavior(IBuildingCornersProvider cornerProvider) : base() {
            CornersProvider = cornerProvider;
        }

        private IBuildingCornersProvider CornersProvider { get; set; }

        protected override void Initialize() {
            Label = "Buildings";
            ShowBuildings = true;
            AddCornersCommand = new RelayCommand(p => CreateCorners());
            ChangeCornersCommand = new RelayCommand(p => ShowCorners());
            LoadFloorCommand = new RelayCommand(p => LoadFloor());
            State = BehaviorState.Default;
            Update();
        }

        public ICommand AddCornersCommand { get; private set; }

        public ICommand ChangeCornersCommand { get; private set; }

        public ICommand LoadFloorCommand { get; private set; }

        private BehaviorState State { get; set; }

        private void CreateCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            State = BehaviorState.CreatingCorners;
            ShowBuildings = false;
            CornersProvider.CreateNewCorners();
            ShowSaveCancel = true;
        }

        private void ShowCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            State = BehaviorState.MovingCorners;
            ShowBuildings = false;
            CornersProvider.DisplayCorners(LocationsController.Instance.CurrentLocation.Corners as ICollection<Location>);
            ShowSaveCancel = true;
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

        protected override void Save() {
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

        protected override void Cancel() {
            if(State == BehaviorState.Floor) ImageController.Instance.CloseImage();
            CornersProvider.ClearCorners();
            ShowBuildings = true;
            ShowSaveCancel = false;
            State = BehaviorState.Default;
        }

        public override void Update() {
            LocationsChanged(this, new LocationEventArgs() { NewLocations = LocationsController.Instance.All, });
            ShowBuildings = true;
        }

        private bool ShouldShowLabel(RhitLocation location) {
            //TODO: Add logic to handle maps with text on them already
            if(!ShowLabels) return false;
            //if(Controller.MapControl.ZoomLevel < location.MinZoomLevel) return false;
            return true;
        }

        public override void SaveSettings() {
            DataStorage.SaveState(StorageKey.VisibleOutlines, ShowBuildings);
            DataStorage.SaveState(StorageKey.VisibleLabels, ShowLabels);
        }

        public override void LoadSettings() {
            ShowBuildings = (bool) DataStorage.LoadState<object>(StorageKey.VisibleOutlines, ShowBuildings);
            ShowLabels = (bool) DataStorage.LoadState<object>(StorageKey.VisibleLabels, ShowLabels);
        }

        protected override void LocationsChanged(object sender, LocationEventArgs e) {
            foreach(RhitLocation location in e.NewLocations) {
                if(location.Corners == null || location.Corners.Count <= 0) continue;
                LocationsController.Instance.AddBuilding(location);
            }
        }

        protected override void CurrentLocationChanged(object sender, LocationEventArgs e) {
            if(!ShowBuildings) {
                if(e.OldLocation != null)
                    LocationsController.Instance.RemoveBuilding(e.OldLocation);
                if(e.NewLocation != null)
                    LocationsController.Instance.AddBuilding(e.NewLocation);
            }
            if(e.NewLocation != null) MapController.Instance.MapControl.Center = e.NewLocation.Center;
        }
    }
}
