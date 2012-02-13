using System;
using System.ComponentModel;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.ViewModel.Models {
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
    }
}
