using System;
using System.ComponentModel;
using Rhit.Applications.ViewModels.Controllers;

#if WINDOWS_PHONE
using System.Device.Location;
#else
using Rhit.Applications.Models;
#endif


namespace Rhit.Applications.ViewModels {
    public class MapViewModel : BaseMapViewModel {
        protected override void Initialize() {
            base.Initialize();
            if(DesignerProperties.IsInDesignTool) return;

            Locations = LocationsController.Instance;
            LocationsController.Instance.CurrentLocationChanged += new EventHandler(CurrentLocationChanged);
            Settings = SettingsController.Instance;
        }

        protected void CurrentLocationChanged(object sender, EventArgs e) {
            if(LocationsController.Instance.CurrentLocation != null)
                Center = LocationsController.Instance.CurrentLocation.Center;
        }

        public LocationsController Locations { get; protected set; }

        public SettingsController Settings { get; protected set; }

        public virtual void SelectLocation(int id) {
            LocationsController.Instance.SelectLocation(id);
        }

        public void GotoRhit() {
            ZoomLevel = 17;
            Center = new GeoCoordinate(39.483433300823, -87.3257801091232); //TODO: No Hard coding
        }
    }
}
