using System;

namespace Rhit.Applications.Models.Events {
    public delegate void DirectionEventHandler(Object sender, DirectionEventArgs e);

    public class DirectionEventArgs : ServiceEventArgs {
        public DirectionEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public Direction_DC Direction { get; set; }
    }
}
