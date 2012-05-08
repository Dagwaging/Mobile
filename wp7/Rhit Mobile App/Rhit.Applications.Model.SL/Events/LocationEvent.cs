using System;

namespace Rhit.Applications.Models.Events {
    public delegate void LocationEventHandler(Object sender, LocationEventArgs e);

    public class LocationEventArgs : ServiceEventArgs {
        public LocationEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public LocationData Location { get; set; }
    }
}
