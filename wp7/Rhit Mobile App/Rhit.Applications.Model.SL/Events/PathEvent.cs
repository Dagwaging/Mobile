using System;

namespace Rhit.Applications.Models.Events {
    public delegate void PathEventHandler(Object sender, PathEventArgs e);

    public class PathEventArgs : ServiceEventArgs {
        public PathEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public Path_DC Path { get; set; }
    }
}
