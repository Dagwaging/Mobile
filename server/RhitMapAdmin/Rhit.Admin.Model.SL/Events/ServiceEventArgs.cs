using System;
using System.Collections.Generic;

namespace Rhit.Admin.Model.Events {
    public delegate void ServiceEventHandler(Object sender, ServiceEventArgs e);

    public class ServiceEventArgs : EventArgs {
        public List<RhitLocation> Locations { get; set; }
    }
}
