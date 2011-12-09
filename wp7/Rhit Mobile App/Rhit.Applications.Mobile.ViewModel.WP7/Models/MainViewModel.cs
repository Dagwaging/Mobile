using System;
using System.Windows;
using Microsoft.Phone.Controls.Maps;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;
using Rhit.Applications.ViewModel.Controllers;
using System.Collections.Generic;
using Rhit.Applications.Model;

namespace Rhit.Applications.ViewModel.Models {
    public class MainViewModel : DependencyObject {
        public MainViewModel(Map map) {
            DataCollector.Instance.UpdateAvailable += new ServiceEventHandler(UpdateAvailable);
            MapController.CreateMapController(map);
            Map = MapController.Instance;
            Settings = SettingsController.Instance;
        }

        public MainViewModel() {
            DataCollector.Instance.UpdateAvailable += new ServiceEventHandler(UpdateAvailable);
            Map = MapController.Instance;
            Settings = SettingsController.Instance;
        }

        public MapController Map { get; set; }

        public SettingsController Settings { get; set; }

        private void UpdateAvailable(object sender, ServiceEventArgs e) {
            List<RhitLocation> locations = DataCollector.Instance.GetAllLocations(null);
            if(locations == null || locations.Count <= 0) return;
            Map.SetLocations(locations);
        }


    }
}
