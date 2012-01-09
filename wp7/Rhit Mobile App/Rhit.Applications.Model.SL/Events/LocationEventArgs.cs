using System;

namespace Rhit.Applications.Model.Events {
    public delegate void LocationEventHandler(Object sender, LocationEventArgs e);

    public class LocationEventArgs : ServiceEventArgs {
        public LocationEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }
        public RhitLocation Location { get; set; }
    }
}
