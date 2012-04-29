using System;

namespace Rhit.Applications.Models.Events {
    public delegate void DirectionMessageEventHandler(Object sender, DirectionMessageEventArgs e);

    public class DirectionMessageEventArgs : ServiceEventArgs {
        public DirectionMessageEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public DirectionMessage_DC DirectionMessage { get; set; }
    }
}
