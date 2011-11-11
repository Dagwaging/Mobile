using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Data;
using System.Data.SqlClient;

namespace RHITMobile
{
    public class DirectionsFinder : PathHandler
    {
        private static Hash<DirectionsFinder> _directions = new Hash<DirectionsFinder>(16);

        public static void EnqueueMonitors(ThreadManager TM)
        {
            TM.Enqueue(_directions.LookForNewIndex(TM), ThreadPriority.Low);
            TM.Enqueue(_directions.CheckForIncreaseSize(TM), ThreadPriority.Low);
        }

        public static void WriteStatus()
        {
            Console.Write("Directions in progress: ");
            _directions.WriteStatus();
        }

        public static DirectionsFinder GetFinder(int id)
        {
            return _directions[id];
        }

        private int _done = 0;
        private int _id;

        private Node _start;
        private DirectionsSettings _settings;
        private List<Path> _paths = new List<Path>();

        public DirectionsFinder(DataRow row)
        {
            _id = _directions.Insert(this);
            _start = new Node(row);
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state)
        {
            var currentThread = TM.CurrentThread;

            if (true)//((int)state >= 0)
            {
                TM.Enqueue(GetDirections(TM, (int)state, query));
            }

            yield return TM.Sleep(currentThread, 1000);

            bool wait;
            if (query.ContainsKey("wait") && Boolean.TryParse(query["wait"], out wait) && wait)
            {
                while (_done < 100)
                    yield return TM.Sleep(currentThread, 1000);
            }

            int done = _done;
            if (done < 100)
            {
                yield return TM.Return(currentThread, new JsonResponse(new DirectionsResponse(done, _id)));
            }
            else
            {
                var directions = new DirectionsResponse(100, _id);
                directions.Result = new Directions(_start.Pos, _paths);
                yield return TM.Return(currentThread, new JsonResponse(directions));
            }
        }

        public IEnumerable<ThreadInfo> GetDirections(ThreadManager TM, int to, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;

            _settings = new DirectionsSettings(query);

            yield return TM.Await(currentThread, GetShortestPath(TM, to));

            _done = 80;

            yield return TM.Await(currentThread, GetDirectionMessages(TM));

            _done = 100;
            yield return TM.Sleep(currentThread, 60000);
            _directions.Remove(_id);

            yield return TM.Return(currentThread);
        }

        #region Shortest Path
        private IEnumerable<ThreadInfo> GetShortestPath(ThreadManager TM, int to)
        {
            var currentThread = TM.CurrentThread;

            // See if the start node is already in the goal location
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationNodes", new SqlParameter("@location", to));
            using (var table = TM.GetResult<DataTable>(currentThread))
            {
                if (table.Rows.Count == 0)
                {
                    // Error - no nodes in goal location
                    yield return TM.Return(currentThread);
                }

                foreach (DataRow row in table.Rows)
                {
                    if (new Node(row).Id == _start.Id)
                    {
                        // Already at goal
                        yield return TM.Return(currentThread);
                    }
                }
            }

            // Get the nodes on the boundary of the goal location
            var goalNodes = new Dictionary<int, SortedSet<Node>>();
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationBoundaryNodes", new SqlParameter("@location", to));
            using (var table = TM.GetResult<DataTable>(currentThread))
            {
                if (table.Rows.Count == 0)
                {
                    // Error - no nodes on goal location's boundary
                    yield return TM.Return(currentThread);
                }

                foreach (DataRow row in table.Rows)
                {
                    var node = new Node(row);
                    if (!goalNodes.ContainsKey(node.Partition.Value))
                    {
                        goalNodes[node.Partition.Value] = new SortedSet<Node>(new NodeComparer());
                    }
                    goalNodes[node.Partition.Value].Add(node);
                }
            }

            // Get the partitions that the starting node is in
            yield return TM.Await(currentThread, GetNodePartitions(TM, _start, -1, "spGetNodePartitions"));
            var startNodePartitions = TM.GetResult<SortedSet<int>>(currentThread);

            // Get the target nodes
            yield return TM.Await(currentThread, GetTargetNodes(TM, goalNodes, startNodePartitions, -1));
            var targetNodes = TM.GetResult<Dictionary<int, KeyValuePair<Node, int>>>(currentThread);

            // IDK stuff
            var queue = new SortedSet<TargetNode>(new TargetNodeComparer());
            foreach (var targetNode in targetNodes)
            {
                queue.Add(new TargetNode(_settings, goalNodes, null, _start, targetNode.Value.Key, targetNode.Value.Value, 0));
            }

            var shortestPaths = new Dictionary<int, Path>();
            var nodesTraveled = new Dictionary<int, SortedSet<int>>();

            while (true)
            {
                var currentNode = queue.First();

                _done = Math.Max(_done, (int)(80 * currentNode.WeightedDistSoFar * currentNode.WeightedDistSoFar / (currentNode.TotalWeightedDist * currentNode.TotalWeightedDist)));

                if (goalNodes.Any(kvp => kvp.Value.Contains(currentNode.Current)))
                {
                    int arrayStart = _paths.Count;
                    int node = currentNode.Current.Id;
                    while (shortestPaths.ContainsKey(node))
                    {
                        var path = shortestPaths[node];
                        _paths.Insert(arrayStart, path);
                        node = path.FromNode;
                    }
                    yield return TM.Return(currentThread);
                }

                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetPathsFromNode",
                    new SqlParameter("@node", currentNode.Current.Id),
                    new SqlParameter("@part", currentNode.Partition));
                using (var table = TM.GetResult<DataTable>(currentThread))
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var path = new Path(row, currentNode.Current, _settings);
                        if ((currentNode.Previous == null || path.ToNode.Id != currentNode.Previous.Id)
                            && path.ToNode.Id != _start.Id)
                        {
                            double weightedDist = currentNode.WeightedDistSoFar + _settings.WeightedDist(path);
                            if (!shortestPaths.ContainsKey(path.ToNode.Id))
                            {
                                shortestPaths[path.ToNode.Id] = path;
                            }
                            else if (shortestPaths[path.ToNode.Id].Id != path.Id)
                            {
                                double totalDistBefore = 0;
                                int node = path.ToNode.Id;
                                while (shortestPaths.ContainsKey(node))
                                {
                                    var path0 = shortestPaths[node];
                                    totalDistBefore += path0.WeightedDist;
                                    node = path0.FromNode;
                                }
                                double totalDistAfter = path.WeightedDist;
                                node = path.FromNode;
                                while (shortestPaths.ContainsKey(node))
                                {
                                    var path0 = shortestPaths[node];
                                    totalDistAfter += path0.WeightedDist;
                                    node = path0.FromNode;
                                }

                                if (totalDistAfter < totalDistBefore)
                                {
                                    shortestPaths[path.ToNode.Id] = path;
                                    var shortestDists = new Dictionary<int, double>();
                                    var changedNodes = new List<TargetNode>();
                                    foreach (var targetNode in queue)
                                    {
                                        double totalDist = GetShortestDist(targetNode.Current.Id, shortestDists, shortestPaths);
                                        if (targetNode.WeightedDistSoFar != totalDist)
                                        {
                                            targetNode.WeightedDistSoFar = totalDist;
                                            changedNodes.Add(targetNode);
                                        }
                                    }
                                    foreach (var targetNode in changedNodes)
                                    {
                                        queue.Remove(targetNode);
                                        while (queue.Contains(targetNode))
                                        {
                                            targetNode.WeightedDistSoFar *= 1.00000001;
                                        }
                                        queue.Add(targetNode);
                                    }
                                }
                            }

                            if (!nodesTraveled.ContainsKey(path.ToNode.Id))
                            {
                                nodesTraveled[path.ToNode.Id] = new SortedSet<int>() { };
                            }
                            else if (nodesTraveled[path.ToNode.Id].Contains(currentNode.Target.Id))
                            {
                                continue;
                            }
                            nodesTraveled[path.ToNode.Id].Add(currentNode.Target.Id);

                            if (path.ToNode.Id == currentNode.Target.Id && !goalNodes.Any(kvp => kvp.Value.Contains(path.ToNode)))
                            {
                                yield return TM.Await(currentThread, GetNodePartitions(TM, path.ToNode, currentNode.Partition, "spGetBoundaryNodePartitions"));
                                var partitions = TM.GetResult<SortedSet<int>>(currentThread);

                                yield return TM.Await(currentThread, GetTargetNodes(TM, goalNodes, partitions, currentNode.Partition));
                                targetNodes = TM.GetResult<Dictionary<int, KeyValuePair<Node, int>>>(currentThread);

                                foreach (var targetNode in targetNodes)
                                {
                                    queue.Add(new TargetNode(_settings, goalNodes, currentNode.Current, path.ToNode, targetNode.Value.Key, targetNode.Value.Value, weightedDist));
                                }
                            }
                            else
                            {
                                queue.Add(new TargetNode(_settings, goalNodes, currentNode.Current, path.ToNode, currentNode.Target, currentNode.Partition, weightedDist));
                            }
                        }
                    }
                }

                queue.Remove(currentNode);
            }
        }

        private double GetShortestDist(int node, Dictionary<int, double> shortestDists, Dictionary<int, Path> shortestPaths)
        {
            if (shortestPaths.ContainsKey(node))
            {
                if (shortestDists.ContainsKey(node))
                {
                    return shortestDists[node];
                }
                else
                {
                    var path = shortestPaths[node];
                    double dist = GetShortestDist(path.FromNode, shortestDists, shortestPaths) + path.WeightedDist;
                    shortestDists[node] = dist;
                    return dist;
                }
            }
            else
            {
                return 0;
            }
        }

        private IEnumerable<ThreadInfo> GetNodePartitions(ThreadManager TM, Node node, int skipPartition, string procedure)
        {
            var currentThread = TM.CurrentThread;

            var partitions = new SortedSet<int>();
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, procedure, new SqlParameter("@node", node.Id));
            using (var partitionsTable = TM.GetResult<DataTable>(currentThread))
            {
                foreach (DataRow partitionRow in partitionsTable.Rows)
                {
                    int partition = (int)partitionRow["partition"];
                    if (partition != skipPartition)
                        partitions.Add(partition);
                }
            }

            yield return TM.Return(currentThread, partitions);
        }

        private IEnumerable<ThreadInfo> GetTargetNodes(ThreadManager TM, Dictionary<int, SortedSet<Node>> goalNodes, SortedSet<int> nodePartitions, int currentPartition)
        {
            var currentThread = TM.CurrentThread;

            var targetNodes = new Dictionary<int, KeyValuePair<Node, int>>();
            foreach (int partition in nodePartitions)
            {
                if (goalNodes.ContainsKey(partition))
                {
                    foreach (var node in goalNodes[partition])
                    {
                        targetNodes[node.Id] = new KeyValuePair<Node, int>(node, partition);
                    }
                }
            }

            foreach (int partition in nodePartitions)
            {
                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetPartitionBoundaries", new SqlParameter("@partition", partition));
                using (var boundariesTable = TM.GetResult<DataTable>(currentThread))
                {
                    foreach (DataRow boundaryRow in boundariesTable.Rows)
                    {
                        var node = new Node(boundaryRow);
                        targetNodes[node.Id] = new KeyValuePair<Node,int>(node, partition);
                    }
                }
            }

            if (currentPartition >= 0)
            {
                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetPartitionBoundaries", new SqlParameter("@partition", currentPartition));
                using (var boundariesTable = TM.GetResult<DataTable>(currentThread))
                {
                    foreach (DataRow boundaryRow in boundariesTable.Rows)
                    {
                        var node = new Node(boundaryRow);
                        targetNodes.Remove(node.Id);
                    }
                }
            }

            yield return TM.Return(currentThread, targetNodes);
        }

        private class TargetNode
        {
            public TargetNode(DirectionsSettings settings, Dictionary<int, SortedSet<Node>> goalNodes, Node previous, Node current, Node target, int partition, double weightedDistSoFar)
            {
                Settings = settings;
                GoalNodes = goalNodes;
                Previous = previous;
                Current = current;
                Target = target;
                Partition = partition;
                WeightedDistSoFar = weightedDistSoFar;
                MinWeightedDistFromTargetToGoal = goalNodes.Min(kvp => kvp.Value.Min(goalNode => settings.WeightedDist(target, goalNode)));
                MinWeightedDistToTarget = settings.WeightedDist(current, target);
            }

            public DirectionsSettings Settings { get; set; }
            public Dictionary<int, SortedSet<Node>> GoalNodes { get; set; }

            public Node Previous { get; set; }
            public Node Current { get; set; }
            public Node Target { get; set; }
            public int Partition { get; set; }

            public double WeightedDistSoFar { get; set; }
            public double MinWeightedDistFromTargetToGoal { get; set; }
            public double MinWeightedDistToTarget { get; set; }

            public double TotalWeightedDist
            {
                get
                {
                    return WeightedDistSoFar + MinWeightedDistToTarget + MinWeightedDistFromTargetToGoal;
                }
            }
        }

        private class TargetNodeComparer : IComparer<TargetNode>
        {
            public int Compare(TargetNode tn1, TargetNode tn2)
            {
                if (tn1.Current.Id == tn2.Current.Id && tn1.Target.Id == tn2.Target.Id)
                {
                    return 0;
                }
                else
                {
                    int result = tn1.TotalWeightedDist.CompareTo(tn2.TotalWeightedDist);
                    return result == 0 ? Math.Sign(result) * 1000000 + tn1.Current.Id - tn2.Current.Id : result;
                }
            }
        }

        public class NodeComparer : IComparer<Node>
        {
            public int Compare(Node node1, Node node2)
            {
                return node1.Id.CompareTo(node2.Id);
            }
        }
        #endregion

        #region Direction Messages
        public IEnumerable<ThreadInfo> GetDirectionMessages(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;

            for (int i = 0; i < _paths.Count; i++)
            {
                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetDirection",
                    new SqlParameter("@startpath", _paths[i].Id));
                using (var directionTable = TM.GetResult<DataTable>(currentThread))
                {
                    foreach (DataRow directionRow in directionTable.Rows)
                    {
                        yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetDirectionPaths",
                            new SqlParameter("@direction", (int)directionRow["id"]));
                        using (var pathTable = TM.GetResult<DataTable>(currentThread))
                        {
                            bool match = true;
                            int dir = 0;
                            int j = i;
                            foreach (DataRow pathRow in pathTable.Rows)
                            {
                                if (dir != 0)
                                {
                                    if (j < 0 || j >= _paths.Count || (int)pathRow["path"] != _paths[j].Id)
                                    {
                                        match = false;
                                        break;
                                    }
                                    j += dir;
                                }
                                else
                                {
                                    if (i > 0 && (int)pathRow["path"] == _paths[i - 1].Id)
                                    {
                                        dir = -1;
                                        j = i - 2;
                                    }
                                    else if (i < _paths.Count - 1 && (int)pathRow["path"] == _paths[i + 1].Id)
                                    {
                                        dir = 1;
                                        j = i + 2;
                                    }
                                    else
                                    {
                                        match = false;
                                        break;
                                    }
                                }
                            }
                            if (match)
                            {
                                // Deal with within's
                                _paths[Math.Min(j - dir, i)].Dir = dir > 0 ? (string)directionRow["message1"] : (string)directionRow["message2"];
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }

    public class DirectionsSettings
    {
        public double StairsMult;
        public double OutsideMult;
        public bool CanUseElev;

        public double StairsDownMult { get { return StairsMult / 2; } }

        public DirectionsSettings(Dictionary<string, string> query)
        {
            if (!query.ContainsKey("stairmult") || !Double.TryParse(query["stairmult"], out StairsMult))
                StairsMult = 1;
            if (!query.ContainsKey("outsidemult") || !Double.TryParse(query["outsidemult"], out OutsideMult))
                OutsideMult = 1;
            if (!query.ContainsKey("useelev") || !Boolean.TryParse(query["useelev"], out CanUseElev))
                CanUseElev = false;
        }

        public double WeightedDist(Node node1, Node node2)
        {
            double hDist = node1.HDistanceTo(node2);
            double vDist = node2.Alt - node1.Alt;
            double angle = hDist != 0 ? Math.Asin(vDist / hDist) : vDist > 0 ? Math.PI / 2 : vDist < 0 ? -Math.PI / 2 : 0;

            double result;
            if (Math.Abs(angle) <= Program.MaxSlopeAngle)
            {
                result = Math.Sqrt(hDist * hDist + vDist * vDist);
            }
            else if (Math.Abs(angle) < Program.StairAngle)
            {
                result = SlightAngleWeightedDist(hDist, vDist);
            }
            else
            {
                result = SteepAngleWeightedDist(vDist);
            }

            return result;
        }

        private double SlightAngleWeightedDist(double hDist, double vDist)
        {
            double slopeHDist = (Math.Abs(vDist) - hDist * Program.StairRatio) / (Program.MaxSlopeRatio - Program.StairRatio);
            double slopeVDist = slopeHDist * Program.MaxSlopeRatio * Math.Sign(vDist);
            return Math.Sqrt(slopeHDist * slopeHDist + slopeVDist * slopeVDist) + SteepAngleWeightedDist(vDist - slopeVDist);
        }

        private double SteepAngleWeightedDist(double vDist)
        {
            bool goingUp = vDist > 0;
            vDist = Math.Abs(vDist);
            if ((!goingUp && StairsDownMult < Program.UseStairsStairMultiplier) || StairsMult < Program.UseStairsStairMultiplier)
            {
                double hDist = vDist / Program.StairRatio;
                return Math.Sqrt(hDist * hDist + vDist * vDist) + Math.Floor(vDist / Program.StairHeight) * (goingUp ? StairsMult : StairsDownMult);
            }
            else
            {
                double hDist = vDist / Program.MaxSlopeRatio;
                return Math.Sqrt(hDist * hDist + vDist * vDist);
            }
        }

        public double WeightedDist(Path path)
        {
            double result = Math.Sqrt(path.HDist * path.HDist + path.VDist * path.VDist);
            if (path.Stairs > 0)
                result += path.Stairs * StairsMult;
            else if (path.Stairs < 0)
                result += -path.Stairs * StairsDownMult;
            return result * (OutsideMult * path.Outside + (1 - path.Outside));
        }
    }
}
