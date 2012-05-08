using System;
using System.Collections.Generic;

namespace Rhit.Applications.Models.Events {
    public delegate void DirectionsEventHandler(Object sender, DirectionsEventArgs e);

    public class DirectionsEventArgs : ServiceEventArgs {
        public DirectionsEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public IEnumerable<DirectionPath_DC> Paths { get; set; }
    }
}
