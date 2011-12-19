using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rhit.Applications.ViewModel.Controllers;
using Microsoft.Maps.MapControl;

namespace Rhit.Applications.ViewModel.Behaviors {
    public class LocationsBehavior : MapBehavior {

        public LocationsBehavior() : base() { }

        protected override void Initialize() {
            //throw new NotImplementedException();
            Label = "Locations";
            ShowTopLocations = true;
        }

        public override void SaveSettings() {
            //throw new NotImplementedException();
        }

        public override void LoadSettings() {
            //throw new NotImplementedException();
        }

        public override void Update() {
            ShowTopLocations = true;
        }

        protected override void LocationsChanged(object sender, Model.Events.LocationEventArgs e) {
            //throw new NotImplementedException();
        }

        protected override void CurrentLocationChanged(object sender, Model.Events.LocationEventArgs e) {
            //throw new NotImplementedException();
        }

        protected override void Save() {
            //throw new NotImplementedException();
        }

        protected override void Cancel() {
            //throw new NotImplementedException();
        }
    }
}
