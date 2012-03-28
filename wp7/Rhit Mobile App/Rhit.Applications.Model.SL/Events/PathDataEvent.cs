using System;
using System.Collections.Generic;

namespace Rhit.Applications.Models.Events {
    public delegate void PathDataEventHandler(Object sender, PathDataEventArgs e);

    public class PathDataEventArgs : ServiceEventArgs {
        public PathDataEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }
        
        public IEnumerable<Direction_DC> Directions { get; set; }
        public IEnumerable<DirectionMessage_DC> Messages { get; set; }
        public IEnumerable<Node_DC> Nodes { get; set; }
        public IEnumerable<Partition_DC> Partitions { get; set; }
        public IEnumerable<Path_DC> Paths { get; set; }
    }
}
