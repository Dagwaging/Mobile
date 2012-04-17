using System;

namespace Rhit.Applications.Models.Events {
    public delegate void NodeEventHandler(Object sender, NodeEventArgs e);

    public class NodeEventArgs : ServiceEventArgs {
        public NodeEventArgs(ServiceEventArgs baseArgs)
            : base() {
            Copy(baseArgs);
        }

        public Node_DC Node { get; set; }
    }
}
