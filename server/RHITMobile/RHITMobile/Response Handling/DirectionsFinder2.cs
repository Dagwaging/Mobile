using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace RHITMobile {
    public class DirectionsFinder : PathHandler {
        private static Hash<DirectionsFinder> _directions = new Hash<DirectionsFinder>(16);

        public static void EnqueueMonitors(ThreadManager TM) {
            TM.Enqueue(_directions.LookForNewIndex(TM), ThreadPriority.Low);
            TM.Enqueue(_directions.CheckForIncreaseSize(TM), ThreadPriority.Low);
        }

        public static void WriteStatus() {
            Console.Write("Directions in progress: ");
            _directions.WriteStatus();
        }

        public static DirectionsFinder GetFinder(int id) {
            return _directions[id];
        }

        private double _pathFindingDone = 0;
        private double _messageFindingDone = 0;
        private int _done { get { return (int)(_pathFindingDone * 80 + _messageFindingDone * 20); } }
        private int _id;
        private DirectionsSettings _settings;
        private double _maxDist;

        public List<DirectionPath> Paths { get; set; }
        public Node Start { get; set; }
        public bool Valid { get; set; }
        public double TotalDist { get; set; }

        private Node _end = null;
        public Node End {
            get {
                return _end ?? Start;
            }
            set {
                _end = value;
            }
        }

        public DirectionsFinder(DataRow row) {
            _id = _directions.Insert(this);
            Start = new Node(row);
            Paths = new List<DirectionPath>();
            _maxDist = Double.MaxValue;
            Valid = false;
        }

        public DirectionsFinder(Node start) {
            _id = _directions.Insert(this);
            Start = start;
            Paths = new List<DirectionPath>();
            _maxDist = Double.MaxValue;
            Valid = false;
        }

        public DirectionsFinder(Node start, DirectionsSettings settings, double maxDist) {
            _id = -1;
            Start = start;
            Paths = new List<DirectionPath>();
            _settings = settings;
            _maxDist = maxDist;
            Valid = false;
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;

            if ((int)state >= 0)
            {
                TM.Enqueue(GetDirections(TM, (int)state, query));
            }

            yield return TM.Sleep(currentThread, 1000);

            while (_done == 0)
                yield return TM.Sleep(currentThread, 1000);

            bool wait;
            if (query.ContainsKey("wait") && Boolean.TryParse(query["wait"], out wait) && wait) {
                while (_done < 100)
                    yield return TM.Sleep(currentThread, 1000);
            }

            int done = _done;
            if (done < 100) {
                yield return TM.Return(currentThread, new JsonResponse(new DirectionsResponse(done, _id)));
            } else {
                var directions = new DirectionsResponse(100, _id);
                directions.Result = new Directions(Paths);
                yield return TM.Return(currentThread, new JsonResponse(directions));
            }
        }

        public IEnumerable<ThreadInfo> GetDirections(ThreadManager TM, int to, Dictionary<string, string> query) {
            var currentThread = TM.CurrentThread;

            _settings = new DirectionsSettings(query);

            yield return TM.Await(currentThread, GetShortestPathToLocation(TM, to));
            try {
                TM.GetResult(currentThread);
            } catch {
                _messageFindingDone = 1;
            }

            _pathFindingDone = 1;

            if (Valid) {
                yield return TM.Await(currentThread, GetDirectionMessages(TM, Paths, d => _messageFindingDone = d));
            }

            _messageFindingDone = 1;
            yield return TM.Sleep(currentThread, 60000);
            _directions.Remove(_id);

            yield return TM.Return(currentThread);
        }

        #region Shortest Path
        public IEnumerable<ThreadInfo> GetShortestPathToNode(ThreadManager TM, int toNode) {
            var currentThread = TM.CurrentThread;

            if (Start.Id == toNode) {
                // Already at goal
                Valid = true;
                _pathFindingDone = 1;
                yield return TM.Return(currentThread, new object());
            }

            var goalNodes = new Dictionary<int, SortedSet<Node>>();
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetNode", new SqlParameter("@node", toNode));
            var nodeData = TM.GetResult<DataTable>(currentThread).Rows[0];

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetNodePartitions", new SqlParameter("@node", toNode));
            using (var table = TM.GetResult<DataTable>(currentThread)) {
                foreach (DataRow row in table.Rows) {
                    var node = new Node(nodeData);
                    node.Partition = (int)row["partition"];
                    goalNodes[node.Partition.Value] = new SortedSet<Node>(new NodeComparer());
                    goalNodes[node.Partition.Value].Add(node);
                }
            }

            yield return TM.Await(currentThread, GetShortestPath(TM, goalNodes));
            yield return TM.Return(currentThread);
        }

        public IEnumerable<ThreadInfo> GetShortestPathToLocation(ThreadManager TM, int toLoc) {
            var currentThread = TM.CurrentThread;

            // See if the start node is already in the goal location
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationNodes", new SqlParameter("@location", toLoc));
            using (var table = TM.GetResult<DataTable>(currentThread)) {
                if (table.Rows.Count == 0) {
                    // Error - no nodes in goal location
                    Valid = false;
                    _pathFindingDone = 1;
                    yield return TM.Return(currentThread, new object());
                }

                foreach (DataRow row in table.Rows) {
                    if (new Node(row).Id == Start.Id) {
                        // Already at goal
                        Valid = true;
                        _pathFindingDone = 1;
                        yield return TM.Return(currentThread, new object());
                    }
                }
            }

            // Get the nodes on the boundary of the goal location
            var goalNodes = new Dictionary<int, SortedSet<Node>>();
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationBoundaryNodes", new SqlParameter("@location", toLoc));
            using (var table = TM.GetResult<DataTable>(currentThread)) {
                if (table.Rows.Count == 0) {
                    // Error - no nodes on goal location's boundary
                    yield return TM.Return(currentThread, new object());
                }

                foreach (DataRow row in table.Rows) {
                    var node = new Node(row);
                    if (!goalNodes.ContainsKey(node.Partition.Value)) {
                        goalNodes[node.Partition.Value] = new SortedSet<Node>(new NodeComparer());
                    }
                    goalNodes[node.Partition.Value].Add(node);
                }
            }

            yield return TM.Await(currentThread, GetShortestPath(TM, goalNodes));
            yield return TM.Return(currentThread);
        }

        private IEnumerable<ThreadInfo> GetShortestPath(ThreadManager TM, Dictionary<int, SortedSet<Node>> goalNodes) {
            var currentThread = TM.CurrentThread;

            // Get the possible partition paths
            Stack<PartitionPass> partitions = new Stack<PartitionPass>();
            partitions.Push(new PartitionPass(-1, Start, 0));
            yield return TM.Await(currentThread, GetPartitionPaths(TM, partitions, goalNodes));
            var passes = TM.GetResult<List<List<PartitionPass>>>(currentThread);

            SortedSet<TargetPath> queue = new SortedSet<TargetPath>(new TargetPathComparer());
            Dictionary<int, ShortestPath> shortestPaths = new Dictionary<int, ShortestPath>();

            shortestPaths[Start.Id] = new ShortestPath();
            foreach (var pass in passes) {
                var targetPath = new TargetPath(Start, 0, pass, queue, _settings);
                if (targetPath.BestDist <= _maxDist)
                    queue.Add(targetPath);
            }

            while (true) {
                if (!queue.Any()) {
                    Valid = false;
                    _pathFindingDone = 1;
                    yield return TM.Return(currentThread, new object());
                }
                var currentPath = queue.First();

                _pathFindingDone = Math.Max(_pathFindingDone, currentPath.DistSoFar * currentPath.DistSoFar / (currentPath.BestDist * currentPath.BestDist));

                if (goalNodes.Any(kvp => kvp.Value.Contains(currentPath.CurrentNode))) {
                    End = currentPath.CurrentNode;
                    TotalDist = 0;
                    var currentNode = currentPath.CurrentNode;
                    var prevNode = shortestPaths[currentNode.Id].PrevNode;
                    Paths.Add(new DirectionPath(currentNode.Lat, currentNode.Lon, null, true, null, currentNode.Altitude, currentNode.Location, currentNode.Outside, shortestPaths[currentNode.Id].Id, shortestPaths[currentNode.Id].Forward));
                    while (currentNode.Id != Start.Id) {
                        Paths.Insert(0, new DirectionPath(prevNode.Lat, prevNode.Lon, null, false, null, prevNode.Altitude, prevNode.Location, prevNode.Outside, shortestPaths[currentNode.Id].Id, shortestPaths[currentNode.Id].Forward));
                        TotalDist += shortestPaths[currentNode.Id].MyDist;
                        currentNode = prevNode;
                        prevNode = shortestPaths[currentNode.Id].PrevNode;
                    }
                    Paths.First().Flag = true;
                    Valid = true;
                    _pathFindingDone = 1;
                    yield return TM.Return(currentThread, new object());
                }

                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetPathsFromNode",
                    new SqlParameter("@node", currentPath.CurrentNode.Id),
                    new SqlParameter("@part", currentPath.PartitionsLeft.First().Partition));
                using (var table = TM.GetResult<DataTable>(currentThread)) {
                    foreach (DataRow row in table.Rows) {
                        var path = new PathToNode(row, currentPath.CurrentNode.Id, currentPath.PartitionsLeft.First().Partition);
                        double hDist;
                        double pathDist = _settings.WeightedDist(currentPath.CurrentNode, path.Node, out hDist);

                        if (shortestPaths.ContainsKey(path.Node.Id)) {
                            if (shortestPaths[path.Node.Id].TotalDist > currentPath.DistSoFar + pathDist) {
                                shortestPaths[path.Node.Id].SetPrevNode(currentPath.CurrentNode, pathDist, hDist, path.Path.Id, path.Forward, shortestPaths);
                            } else if (shortestPaths[path.Node.Id].TotalDist < currentPath.DistSoFar + pathDist) {
                                continue;
                            }
                            // TODO
                        } else {
                            shortestPaths[path.Node.Id] = new ShortestPath(currentPath.CurrentNode, pathDist, hDist, path.Path.Id, path.Forward, shortestPaths);
                            // TODO
                        }

                        TargetPath targetPath;
                        if (path.Node.Id == currentPath.PartitionsLeft.First().Target.Id) {
                            targetPath = new TargetPath(path.Node, shortestPaths[path.Node.Id].TotalDist, currentPath.PartitionsLeft.Skip(1).ToList(), queue, _settings);
                        } else {
                            targetPath = new TargetPath(path.Node, shortestPaths[path.Node.Id].TotalDist, currentPath.PartitionsLeft, queue, _settings);
                        }
                        if (targetPath.BestDist <= _maxDist)
                            queue.Add(targetPath);
                    }
                }

                queue.Remove(currentPath);
            }
        }

        private IEnumerable<ThreadInfo> GetPartitionPaths(ThreadManager TM, Stack<PartitionPass> partitions, Dictionary<int, SortedSet<Node>> goalNodes) {
            var currentThread = TM.CurrentThread;

            var currentNode = partitions.Peek().Target;
            var paths = new List<List<PartitionPass>>();

            // Travel to other partitions, and get the paths through them
            yield return TM.Await(currentThread, GetNodePartitions(TM, currentNode, "spGetNodePartitions"));
            var nodePartitions = TM.GetResult<SortedSet<int>>(currentThread).Except(partitions.Select((PartitionPass pp) => pp.Partition));
            foreach (int partition in nodePartitions) {
                // See if any goal nodes are in this partition
                if (goalNodes.ContainsKey(partition)) {
                    foreach (var goalNode in goalNodes[partition]) {
                        partitions.Push(new PartitionPass(partition, goalNode, _settings.WeightedDist(currentNode, goalNode)));
                        var path = partitions.Reverse().Skip(1).ToList();
                        paths.Add(path);
                        partitions.Pop();
                    }
                }

                // Go to all the boundaries of this partition
                yield return TM.Await(currentThread, GetPartitionBoundaries(TM, partition));
                var targetNodes = TM.GetResult<SortedSet<Node>>(currentThread);
                if (goalNodes.ContainsKey(partition))
                    targetNodes.ExceptWith(goalNodes[partition]);
                targetNodes.Remove(currentNode);
                foreach (var targetNode in targetNodes) {
                    partitions.Push(new PartitionPass(partition, targetNode, _settings.WeightedDist(currentNode, targetNode)));
                    yield return TM.Await(currentThread, GetPartitionPaths(TM, partitions, goalNodes));
                    paths.AddRange(TM.GetResult<List<List<PartitionPass>>>(currentThread));
                    partitions.Pop();
                }
            }

            yield return TM.Return(currentThread, paths);
        }

        private IEnumerable<ThreadInfo> GetNodePartitions(ThreadManager TM, Node node, string procedure) {
            var currentThread = TM.CurrentThread;

            var partitions = new SortedSet<int>();
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, procedure, new SqlParameter("@node", node.Id));
            using (var partitionsTable = TM.GetResult<DataTable>(currentThread)) {
                foreach (DataRow partitionRow in partitionsTable.Rows) {
                    partitions.Add((int)partitionRow["partition"]);
                }
            }

            yield return TM.Return(currentThread, partitions);
        }

        private IEnumerable<ThreadInfo> GetPartitionBoundaries(ThreadManager TM, int partition) {
            var currentThread = TM.CurrentThread;

            var nodes = new SortedSet<Node>(new NodeComparer());
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetPartitionBoundaries", new SqlParameter("@partition", partition));
            using (var boundariesTable = TM.GetResult<DataTable>(currentThread)) {
                foreach (DataRow boundaryRow in boundariesTable.Rows) {
                    nodes.Add(new Node(boundaryRow));
                }
            }

            yield return TM.Return(currentThread, nodes);
        }

        public delegate void ChangedPathHandler(double totalDist);

        private class ShortestPath {
            public ShortestPath() {
                PrevNode = null;
                MyDist = 0;
                TotalDist = 0;
                HDist = 0;
                Id = 0;
                Forward = true;
            }

            public ShortestPath(Node prevNode, double mydist, double hdist, int id, bool forward, Dictionary<int, ShortestPath> shortestPaths) {
                PrevNode = prevNode;
                MyDist = mydist;
                HDist = hdist;
                Id = id;
                Forward = forward;

                shortestPaths[prevNode.Id].Changed += DistChanged;
                TotalDist = shortestPaths[PrevNode.Id].TotalDist + mydist;
            }

            public Node PrevNode { get; private set; }
            public double MyDist { get; set; }
            public double TotalDist { get; set; }
            public double HDist { get; set; }
            public int Id { get; set; }
            public bool Forward { get; set; }
            public event ChangedPathHandler Changed;

            public void SetPrevNode(Node newPrevNode, double newDist, double newHDist, int newId, bool newForward, Dictionary<int, ShortestPath> shortestPaths) {
                shortestPaths[PrevNode.Id].Changed -= DistChanged;
                shortestPaths[newPrevNode.Id].Changed += DistChanged;

                PrevNode = newPrevNode;
                MyDist = newDist;
                HDist = newHDist;
                Id = newId;
                Forward = newForward;
                TotalDist = shortestPaths[PrevNode.Id].TotalDist + newDist;

                if (Changed != null) Changed(TotalDist);
            }

            public void DistChanged(double totalDist) {
                TotalDist = totalDist + MyDist;
                if (Changed != null) Changed(TotalDist);
            }
        }

        private class TargetPath {
            private static int _currentId = 0;

            public TargetPath(Node start, double distSoFar, List<PartitionPass> partitions, SortedSet<TargetPath> queue, DirectionsSettings settings) {
                Id = _currentId;
                _currentId++;
                CurrentNode = start;
                DistSoFar = distSoFar;
                PartitionsLeft = partitions;
                BestDist = PartitionsLeft.Sum(pp => pp.BestDist) + distSoFar;
                if (partitions.Any()) BestDist += settings.WeightedDist(start, partitions.First().Target);
                PathQueue = queue;
            }

            public int Id { get; private set; }
            public Node CurrentNode { get; set; }
            public List<PartitionPass> PartitionsLeft { get; private set; }
            public double BestDist { get; set; }
            public double DistSoFar { get; set; }
            public SortedSet<TargetPath> PathQueue { get; private set; }

            public void DistChanged(double distChange) {
                PathQueue.Remove(this);
                BestDist += distChange;
                DistSoFar += distChange;
                PathQueue.Add(this);
            }
        }

        private class PartitionPass {
            public PartitionPass(int partition, Node target, double bestDist) {
                Partition = partition;
                Target = target;
                BestDist = bestDist;
            }

            public int Partition { get; private set; }
            public Node Target { get; private set; }
            public double BestDist { get; private set; }
        }

        private class NodeComparer : IComparer<Node> {
            public int Compare(Node node1, Node node2) {
                return node1.Id.CompareTo(node2.Id);
            }
        }

        private class TargetPathComparer : IComparer<TargetPath> {
            public int Compare(TargetPath tp1, TargetPath tp2) {
                return Math.Sign(tp1.BestDist.CompareTo(tp2.BestDist)) * 1000000 + tp1.Id - tp2.Id;
            }
        }

        private class PathToNode {
            public PathToNode(DataRow row, int fromNode, int partition) {
                Node = new Node(row);
                Path = new Path(row, fromNode, Node.Id, partition);
                Forward = (bool)row["forward"];
            }

            public Path Path { get; set; }
            public Node Node { get; set; }
            public bool Forward { get; set; }
        }
        #endregion

        #region Direction Messages
        public static IEnumerable<ThreadInfo> GetDirectionMessages(ThreadManager TM, List<DirectionPath> paths, Action<double> setDone) {
            var currentThread = TM.CurrentThread;

            for (int i = 0; i < paths.Count; i++) {
                setDone((double)(i * i) / (double)(paths.Count * paths.Count));
                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetDirection",
                    new SqlParameter("@startpath", paths[i].Id));
                using (var directionTable = TM.GetResult<DataTable>(currentThread)) {
                    foreach (DataRow directionRow in directionTable.Rows) {
                        yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetDirectionPaths",
                            new SqlParameter("@direction", (int)directionRow["id"]));
                        using (var pathTable = TM.GetResult<DataTable>(currentThread)) {
                            bool match = true;
                            int dir = 0;
                            int j = i;
                            foreach (DataRow pathRow in pathTable.Rows) {
                                if (dir != 0) {
                                    if (j < 0 || j >= paths.Count || (int)pathRow["path"] != paths[j].Id) {
                                        match = false;
                                        break;
                                    }
                                    j += dir;
                                } else {
                                    if (i > 0 && (int)pathRow["path"] == paths[i - 1].Id) {
                                        dir = -1;
                                        j = i - 2;
                                    } else if (i < paths.Count - 1 && (int)pathRow["path"] == paths[i + 1].Id) {
                                        dir = 1;
                                        j = i + 2;
                                    } else {
                                        match = false;
                                        break;
                                    }
                                }
                            }
                            if (match) {
                                // TODO: Deal with within's
                                bool forward = dir == 0 ? paths[i].Forward : dir > 0;
                                int index = Math.Min(j - dir, i) + (int)directionRow["nodeoffset"];
                                if (forward) {
                                    if (!directionRow.IsNull("message1")) {
                                        paths[index].Dir = (string)directionRow["message1"];
                                        paths[index].Action = (string)directionRow["action1"];
                                    }
                                } else {
                                    if (!directionRow.IsNull("message1")) {
                                        paths[index].Dir = (string)directionRow["message2"];
                                        paths[index].Action = (string)directionRow["action2"];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            yield return TM.Return(currentThread, new object());

        }
        #endregion
    }

    public class DirectionsSettings {
        public double StairsMult;
        public double OutsideMult;
        public bool CanUseElev;

        public double StairsDownMult { get { return StairsMult / 2; } }

        public DirectionsSettings(Dictionary<string, string> query) {
            if (!query.ContainsKey("stairmult") || !Double.TryParse(query["stairmult"], out StairsMult))
                StairsMult = 1;
            if (!query.ContainsKey("outsidemult") || !Double.TryParse(query["outsidemult"], out OutsideMult))
                OutsideMult = 1;
            if (!query.ContainsKey("useelev") || !Boolean.TryParse(query["useelev"], out CanUseElev))
                CanUseElev = false;
        }

        public double WeightedDist(Node node1, Node node2) {
            double hDist;
            return WeightedDist(node1, node2, out hDist);
        }

        public double WeightedDist(Node node1, Node node2, out double hDist) {
            hDist = node1.HDistanceTo(node2);
            double vDist = node2.Altitude - node1.Altitude;
            double angle = hDist != 0 ? Math.Asin(vDist / hDist) : vDist > 0 ? Math.PI / 2 : vDist < 0 ? -Math.PI / 2 : 0;

            double result;
            if (Math.Abs(angle) <= Program.MaxSlopeAngle) {
                result = Math.Sqrt(hDist * hDist + vDist * vDist);
            } else if (Math.Abs(angle) < Program.StairAngle) {
                result = SlightAngleWeightedDist(hDist, vDist);
            } else {
                result = SteepAngleWeightedDist(vDist);
            }

            return result;
        }

        private double SlightAngleWeightedDist(double hDist, double vDist) {
            double slopeHDist = (Math.Abs(vDist) - hDist * Program.StairRatio) / (Program.MaxSlopeRatio - Program.StairRatio);
            double slopeVDist = slopeHDist * Program.MaxSlopeRatio * Math.Sign(vDist);
            return Math.Sqrt(slopeHDist * slopeHDist + slopeVDist * slopeVDist) + SteepAngleWeightedDist(vDist - slopeVDist);
        }

        private double SteepAngleWeightedDist(double vDist) {
            bool goingUp = vDist > 0;
            vDist = Math.Abs(vDist);
            if ((!goingUp && StairsDownMult < Program.UseStairsStairMultiplier) || StairsMult < Program.UseStairsStairMultiplier) {
                double hDist = vDist / Program.StairRatio;
                return Math.Sqrt(hDist * hDist + vDist * vDist) + Math.Floor(vDist / Program.StairHeight) * (goingUp ? StairsMult : StairsDownMult);
            } else {
                double hDist = vDist / Program.MaxSlopeRatio;
                return Math.Sqrt(hDist * hDist + vDist * vDist);
            }
        }
    }
}
