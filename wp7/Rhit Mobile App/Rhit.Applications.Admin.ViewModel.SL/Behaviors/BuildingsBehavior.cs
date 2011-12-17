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

        private enum BehaviorState { Default, MovingCorners, CreatingCorners, };

        public BuildingsBehavior(IBuildingCornersProvider cornerProvider) : base() {
            CornersProvider = cornerProvider;
        }

        private IBuildingCornersProvider CornersProvider { get; set; }

        protected override void Initialize() {
            Label = "Buildings";
            AreBuildingsVisible = true;
            AddCornersCommand = new RelayCommand(p => CreateCorners());
            ChangeCornersCommand = new RelayCommand(p => ShowCorners());
            State = BehaviorState.Default;
            Update();
        }

        public ICommand AddCornersCommand { get; private set; }

        public ICommand ChangeCornersCommand { get; private set; }

        private BehaviorState State { get; set; }

        private void CreateCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            AreBuildingsVisible = false;
            CornersProvider.CreateNewCorners();
            AreSaveCancelVisible = true;
        }

        private void ShowCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            AreBuildingsVisible = false;
            CornersProvider.DisplayCorners(LocationsController.Instance.CurrentLocation.Locations as ICollection<Location>);
            AreSaveCancelVisible = true;
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
                    OverwriteCorners();
                    break;
            }
        }

        private void SaveCorners() {
            var corners = CornersProvider.GetCorners();

            var executions = new List<KeyValuePair<string, Dictionary<string, object>>>() {
                new KeyValuePair<string, Dictionary<string, object>>("spDeleteMapAreaCorners", new Dictionary<string, object>() {
                    { "location", LocationsController.Instance.CurrentLocation.Id }
                })
            };

            foreach (var corner in corners) {
                executions.Add(new KeyValuePair<string, Dictionary<string, object>>("spAddMapAreaCorner", new Dictionary<string, object>() {
                    { "location", LocationsController.Instance.CurrentLocation.Id },
                    { "lat", corner.Latitude },
                    { "lon", corner.Longitude }
                }));
            }

            DataCollector.Instance.ExecuteBatchStoredProcedure(Dispatcher, executions);
            Cancel();
        }

        private void OverwriteCorners() {
            var corners = CornersProvider.GetCorners();

            //TODO: - Scott
            //This is the ViewModel endpoint for adding/removing corners
            //This maybe the same as the above method, in which case just let me know

            Cancel();
        }

        protected override void Cancel() {
            CornersProvider.ClearCorners();
            AreBuildingsVisible = true;
            AreSaveCancelVisible = false;
        }

        public override void Update() {
            LocationsChanged(this, new LocationEventArgs() { NewLocations = LocationsController.Instance.All, });
            AreBuildingsVisible = true;
        }

        private bool ShouldShowLabel(RhitLocation location) {
            //TODO: Add logic to handle maps with text on them already
            if(!AreLabelsVisible) return false;
            //if(Controller.MapControl.ZoomLevel < location.MinZoomLevel) return false;
            return true;
        }

        public override void SaveSettings() {
            DataStorage.SaveState(StorageKey.VisibleOutlines, AreBuildingsVisible);
            DataStorage.SaveState(StorageKey.VisibleLabels, AreLabelsVisible);
        }

        public override void LoadSettings() {
            AreBuildingsVisible = (bool) DataStorage.LoadState<object>(StorageKey.VisibleOutlines, AreBuildingsVisible);
            AreLabelsVisible = (bool) DataStorage.LoadState<object>(StorageKey.VisibleLabels, AreLabelsVisible);
        }

        protected override void LocationsChanged(object sender, LocationEventArgs e) {
            foreach(RhitLocation location in e.NewLocations) {
                if(location.Locations == null || location.Locations.Count <= 0) continue;
                LocationsController.Instance.AddBuilding(location);
            }
        }

        protected override void CurrentLocationChanged(object sender, LocationEventArgs e) {
            if(!AreBuildingsVisible) {
                if(e.OldLocation != null)
                    LocationsController.Instance.RemoveBuilding(e.OldLocation);
                if(e.NewLocation != null)
                    LocationsController.Instance.AddBuilding(e.NewLocation);
            }
            if(e.NewLocation != null) MapController.Instance.MapControl.Center = e.NewLocation.Center;
        }
    }
}
