using System;

namespace Rhit.Applications.Model.Events {
    public delegate void VersionEventHandler(Object sender, VersionEventArgs e);

    public class VersionEventArgs : ServiceEventArgs {
        public double ServerVersion { get; set; }
        public double ServicesVersion { get; set; }
    }
}
