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

        public BuildingsBehavior(IBuildingCornersProvider cornerProvider) : base() {
            CornersProvider = cornerProvider;
        }

        private IBuildingCornersProvider CornersProvider { get; set; }

        protected override void Initialize() {
            Label = "Buildings";
            AreBuildingsVisible = true;
            ChangeCornersCommand = new RelayCommand(p => ShowCorners());
            Update();
        }

        public ICommand ChangeCornersCommand { get; private set; }

        private void ShowCorners() {
            if(LocationsController.Instance.CurrentLocation == null) return;
            AreBuildingsVisible = false;
            CornersProvider.DisplayCorners(LocationsController.Instance.CurrentLocation.Locations as ICollection<Location>);
        }
        
        protected override void Save() {
            ICollection<Location> corners = CornersProvider.GetCorners();
            //LocationsController.Instance.CurrentLocation
            //TODO: Scott - Save above

            CornersProvider.RemoveCorners();
            AreBuildingsVisible = true;
        }

        protected override void Cancel() {
            CornersProvider.RemoveCorners();
            AreBuildingsVisible = true;
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
