using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls.Maps;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.ViewModel.Models {
    public class MainViewModel : DependencyObject {
        public MainViewModel(Map map) {
            Locations = LocationsController.Instance;
            DataCollector.Instance.UpdateAvailable += new ServiceEventHandler(UpdateAvailable);
            Settings = SettingsController.Instance;
            MapController.CreateMapController(map);
            Map = MapController.Instance;
            User = UserController.Instance;
        }

        public MainViewModel() {
            Locations = LocationsController.Instance;
            Map = MapController.Instance;
            Settings = SettingsController.Instance;
        }

        public MapController Map { get; private set; }

        public SettingsController Settings { get; private set; }

        public LocationsController Locations { get; private set; }

        public UserController User { get; private set; }

        private void UpdateAvailable(object sender, ServiceEventArgs e) {
            List<RhitLocation> locations = DataCollector.Instance.GetAllLocations(null);
            if(locations == null || locations.Count <= 0) return;
            Locations.SetLocations(locations);
        }

        public void IgnoreEvent(Point point) {
            Map.EventCoordinate = Map.MapControl.ViewportPointToLocation(point);
        }

        public void GotoRhit() {
            //TODO: Don't hard code
            Map.MapControl.Center = new GeoCoordinate(39.4820263, -87.3248677);
            Map.MapControl.ZoomLevel = 16;
        }

        public void GotoUser() {
            Map.MapControl.Center = User.Location;
            if(Map.MapControl.ZoomLevel < 16)
                Map.MapControl.ZoomLevel = 16;
        }

        public void GotoCurrentLocation() {
            if(Locations.CurrentLocation == null) {
                GotoRhit();
                return;
            }
            Map.MapControl.Center = Locations.CurrentLocation.Center;
            if(Map.MapControl.ZoomLevel < 16)
                Map.MapControl.ZoomLevel = 16;
        }
    }
}
