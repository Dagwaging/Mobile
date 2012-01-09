using System;
using System.Collections.Generic;

namespace Rhit.Applications.Model.Events {
    public delegate void LocationsEventHandler(Object sender, LocationsEventArgs e);

    public class LocationsEventArgs : ServiceEventArgs {
        public LocationsEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }
        public ICollection<RhitLocation> Locations { get; set; }
    }
}
