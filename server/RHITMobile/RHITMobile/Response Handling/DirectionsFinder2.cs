using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace RHITMobile {
    public class DirectionsFinder2 : PathHandler {
        private static Hash<DirectionsFinder2> _directions = new Hash<DirectionsFinder2>(16);

        public static void EnqueueMonitors(ThreadManager TM) {
            TM.Enqueue(_directions.LookForNewIndex(TM), ThreadPriority.Low);
            TM.Enqueue(_directions.CheckForIncreaseSize(TM), ThreadPriority.Low);
        }

        public static void WriteStatus() {
            Console.Write("Directions in progress: ");
            _directions.WriteStatus();
        }

        public static DirectionsFinder2 GetFinder(int id) {
            return _directions[id];
        }

        private double _pathFindingDone = 0;
        private double _messageFindingDone = 0;
        private int _done { get { return (int)(_pathFindingDone * 80 + _messageFindingDone * 20); } }
        private int _id;

        private Node _start;
        private DirectionsSettings _settings;
        private List<Path> _paths = new List<Path>();

        public DirectionsFinder2(DataRow row) {
            _id = _directions.Insert(this);
            _start = new Node(row);
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;

            if (true)//((int)state >= 0)
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
                directions.Result = new Directions(_start.Pos, _paths);
                yield return TM.Return(currentThread, new JsonResponse(directions));
            }
        }

        public IEnumerable<ThreadInfo> GetDirections(ThreadManager TM, int to, Dictionary<string, string> query) {
            var currentThread = TM.CurrentThread;

            _settings = new DirectionsSettings(query);

            yield return TM.Await(currentThread, GetShortestPath(TM, to));

            _pathFindingDone = 100;

            //yield return TM.Await(currentThread, GetDirectionMessages(TM));

            _messageFindingDone = 100;
            yield return TM.Sleep(currentThread, 60000);
            _directions.Remove(_id);

            yield return TM.Return(currentThread);
        }

        SortedSet<TargetPath> PathQueue = new SortedSet<TargetPath>(new TargetPathComparer());
        private IEnumerable<ThreadInfo> GetShortestPath(ThreadManager TM, int to) {
            var currentThread = TM.CurrentThread;

            // See if the start node is already in the goal location
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationNodes", new SqlParameter("@location", to));
            using (var table = TM.GetResult<DataTable>(currentThread)) {
                if (table.Rows.Count == 0) {
                    // Error - no nodes in goal location
                    yield return TM.Return(currentThread);
                }

                foreach (DataRow row in table.Rows) {
                    if (new Node(row).Id == _start.Id) {
                        // Already at goal
                        yield return TM.Return(currentThread);
                    }
                }
            }

            // Get the nodes on the boundary of the goal location
            var goalNodes = new Dictionary<int, SortedSet<Node>>();
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationBoundaryNodes", new SqlParameter("@location", to));
            using (var table = TM.GetResult<DataTable>(currentThread)) {
                if (table.Rows.Count == 0) {
                    // Error - no nodes on goal location's boundary
                    yield return TM.Return(currentThread);
                }

                foreach (DataRow row in table.Rows) {
                    var node = new Node(row);
                    if (!goalNodes.ContainsKey(node.Partition.Value)) {
                        goalNodes[node.Partition.Value] = new SortedSet<Node>(new NodeComparer());
                    }
                    goalNodes[node.Partition.Value].Add(node);
                }
            }

            // Get the possible partition paths
            List<PartitionPass> partitions = new List<PartitionPass>();
            Node currentNode = _start;
            while (true) {
                yield return TM.Await(currentThread, GetNodePartitions(TM, currentNode, -1, "spGetNodePartitions"));
                var nodePartitions = TM.GetResult<SortedSet<int>>(currentThread);
            }
        }

        private IEnumerable<ThreadInfo> GetNodePartitions(ThreadManager TM, Node node, int skipPartition, string procedure) {
            var currentThread = TM.CurrentThread;

            var partitions = new SortedSet<int>();
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, procedure, new SqlParameter("@node", node.Id));
            using (var partitionsTable = TM.GetResult<DataTable>(currentThread)) {
                foreach (DataRow partitionRow in partitionsTable.Rows) {
                    int partition = (int)partitionRow["partition"];
                    if (partition != skipPartition)
                        partitions.Add(partition);
                }
            }

            yield return TM.Return(currentThread, partitions);
        }

        private class ShortestPath {

        }

        private class TargetPath {
            private static int _currentId = 0;

            public TargetPath(Node start, List<PartitionPass> partitions, SortedSet<TargetPath> queue) {
                Id = _currentId;
                _currentId++;
                CurrentNode = start;
                PartitionsLeft = new List<PartitionPass>(partitions);
                BestDist = PartitionsLeft.Sum(pp => pp.BestDist);
                PathQueue = queue;
            }

            public int Id { get; private set; }
            public Node CurrentNode { get; set; }
            public List<PartitionPass> PartitionsLeft { get; private set; }
            public double BestDist { get; set; }
            public SortedSet<TargetPath> PathQueue { get; private set; }

            public void DistChanged() {
                PathQueue.Remove(this);
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
            double hDist = node1.HDistanceTo(node2);
            double vDist = node2.Alt - node1.Alt;
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

        public double WeightedDist(Path path) {
            double result = Math.Sqrt(path.HDist * path.HDist + path.VDist * path.VDist);
            if (path.Stairs > 0)
                result += path.Stairs * StairsMult;
            else if (path.Stairs < 0)
                result += -path.Stairs * StairsDownMult;
            return result * (OutsideMult * path.Outside + (1 - path.Outside));
        }
    }
}
