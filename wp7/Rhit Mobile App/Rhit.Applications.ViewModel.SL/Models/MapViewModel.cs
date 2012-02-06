using System;
using System.Collections.Generic;
using System.Windows;
using Rhit.Applications.ViewModel.Controllers;
using System.ComponentModel;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
#else
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model;
#endif

namespace Rhit.Applications.ViewModel.Models {
    public class MapViewModel : DependencyObject {
        public MapViewModel() {
            Initialize();
        }

        protected MapViewModel(bool initialize) {
            if(initialize) Initialize();
        }

        private void Initialize() {
            if(DesignerProperties.IsInDesignTool) return;
            Locations = LocationsController.Instance;
            Paths = PathsController.Instance;
            LocationsController.Instance.CurrentLocationChanged += new EventHandler(CurrentLocationChanged);
            Map = MapController.Instance;
            Settings = SettingsController.Instance;

            Settings.ShowBuildings = true;
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
            else Map.Center = new GeoCoordinate(39.483433300823, -87.3257801091232); //TODO: No Hard coding
            return;
        }

        protected void CurrentLocationChanged(object sender, EventArgs e) {
            if(LocationsController.Instance.CurrentLocation != null)
                Map.Center = LocationsController.Instance.CurrentLocation.Center;
        }

        public MapController Map { get; protected set; }

        public LocationsController Locations { get; protected set; }

        public PathsController Paths { get; protected set; }

        public SettingsController Settings { get; protected set; }

        public virtual void SelectLocation(int id) {
            LocationsController.Instance.SelectLocation(id);
        }
    }
}
