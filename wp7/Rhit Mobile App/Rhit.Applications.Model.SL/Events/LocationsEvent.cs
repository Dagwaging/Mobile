using System;
using System.Collections.Generic;

namespace Rhit.Applications.Models.Events {
    public delegate void LocationsEventHandler(Object sender, LocationsEventArgs e);

    public class LocationsEventArgs : ServiceEventArgs {
        public LocationsEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }
        public IList<LocationData> Locations { get; set; }
    }
}
