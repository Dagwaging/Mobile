using System;

namespace Rhit.Applications.Models.Events {
    public delegate void VersionEventHandler(Object sender, VersionEventArgs e);

    public class VersionEventArgs : ServiceEventArgs {
        public VersionEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public double ServerVersion { get; set; }

        public double ServicesVersion { get; set; }
    }
}
