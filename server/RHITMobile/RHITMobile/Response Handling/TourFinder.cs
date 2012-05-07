using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace RHITMobile {
    public class TourFinder : PathHandler {
        private static Hash<TourFinder> _tours = new Hash<TourFinder>(16);

        public static void EnqueueMonitors(ThreadManager TM) {
            TM.Enqueue(_tours.LookForNewIndex(TM), ThreadPriority.Low);
            TM.Enqueue(_tours.CheckForIncreaseSize(TM), ThreadPriority.Low);
        }

        public static void WriteStatus() {
            Console.Write("Tours in progress: ");
            _tours.WriteStatus();
        }

        public static TourFinder GetFinder(int id) {
            return _tours[id];
        }

        private double _pathFindingDone = 0;
        private double _messageFindingDone = 0;
        private int _done { get { return (int)(_pathFindingDone * 90 + _messageFindingDone * 10); } }
        private int _id;

        private Node _start;
        private DirectionsSettings _settings;
        private List<DirectionPath> _paths = new List<DirectionPath>();
        private double _length = 60;

        public TourFinder(DataRow row) {
            _id = _tours.Insert(this);
            _start = new Node(row);
            Tags = new List<int>();
        }

        public TourFinder(Node start) {
            _id = _tours.Insert(this);
            _start = start;
            Tags = new List<int>();
        }

        public List<int> Tags { get; set; }
        public List<LocationRank> Locations { get; set; }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;

            if ((bool)state) {
                TM.Enqueue(GetTour(TM, query));
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
                directions.Result = new Directions(_paths);
                yield return TM.Return(currentThread, new JsonResponse(directions));
            }
        }

        private IEnumerable<ThreadInfo> GetTour(ThreadManager TM, Dictionary<string, string> query) {
            var currentThread = TM.CurrentThread;

            yield return TM.Await(currentThread, GetTourPaths(TM, query));
            try {
                TM.GetResult(currentThread);
            } catch {
                _messageFindingDone = 1;
            }

            _pathFindingDone = 1;

            if (_messageFindingDone != 1) {
                yield return TM.Await(currentThread, DirectionsFinder.GetDirectionMessages(TM, _paths, d => _messageFindingDone = d));
            }

            _messageFindingDone = 1;

            yield return TM.Sleep(currentThread, 60000);
            _tours.Remove(_id);

            yield return TM.Return(currentThread, new object());
        }

        private IEnumerable<ThreadInfo> GetTourPaths(ThreadManager TM, Dictionary<string, string> query) {
            var currentThread = TM.CurrentThread;

            _settings = new DirectionsSettings(query);

            if (query.ContainsKey("length"))
                Double.TryParse(query["length"], out _length);

            double maxDist = _length * 60 * 3; // minutes x (60 seconds / minute) x (3 feet / second)
            double totalDist = 0;

            var pathsBetweenLocs = new Dictionary<int, DirectionsFinder>();
            pathsBetweenLocs.Add(_start.Id, new DirectionsFinder(_start, _settings, maxDist));

            for (int locNum = 0; locNum < Locations.Count; locNum++) {
                _pathFindingDone = (double)(locNum * locNum) / (double)(Locations.Count * Locations.Count);

                var location = Locations[locNum];

                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spIsNodeInLocation",
                    new SqlParameter("node", _start.Id),
                    new SqlParameter("location", location.LocId));
                using (var table = TM.GetResult<DataTable>(currentThread)) {
                    if ((bool)table.Rows[0][0]) break;
                }

                int numPaths = pathsBetweenLocs.Count;
                var pathsToReplace = new List<KeyValuePair<double, int>>();

                foreach (var pathBetweenLocs in pathsBetweenLocs.Values) {
                    double distChange = pathBetweenLocs.Start.HDistanceTo(location.Center.Lat, location.Center.Lon)
                        + pathsBetweenLocs[pathBetweenLocs.End.Id].Start.HDistanceTo(location.Center.Lat, location.Center.Lon)
                        - pathBetweenLocs.TotalDist;
                    pathsToReplace.Add(new KeyValuePair<double, int>(distChange, pathBetweenLocs.Start.Id));
                }

                pathsToReplace = pathsToReplace.OrderBy(kvp => kvp.Key).ToList();

                double minDistChange = Double.MaxValue;
                DirectionsFinder bestPathToReplace = null;
                DirectionsFinder newPath1 = null;
                DirectionsFinder newPath2 = null;
                for (int i = 0; i < 3 && i < numPaths; i++) {
                    var pathToReplace = pathsBetweenLocs[pathsToReplace[i].Value];
                    var finder1 = new DirectionsFinder(pathToReplace.Start, _settings, maxDist - totalDist + pathToReplace.TotalDist);
                    yield return TM.Await(currentThread, finder1.GetShortestPathToLocation(TM, location.LocId));

                    if (finder1.Valid) {
                        var endNode = pathsBetweenLocs[pathToReplace.End.Id].Start.Id;

                        var finder2 = new DirectionsFinder(finder1.End, _settings, maxDist - totalDist + pathToReplace.TotalDist - finder1.TotalDist);
                        yield return TM.Await(currentThread, finder2.GetShortestPathToNode(TM, endNode));

                        if (finder2.Valid) {
                            if (finder1.TotalDist + finder2.TotalDist - pathToReplace.TotalDist < minDistChange) {
                                minDistChange = finder1.TotalDist + finder2.TotalDist - pathToReplace.TotalDist;
                                bestPathToReplace = pathToReplace;
                                newPath1 = finder1;
                                newPath2 = finder2;
                            }
                        }
                    }
                }

                if (bestPathToReplace != null) {
                    pathsBetweenLocs[bestPathToReplace.Start.Id] = newPath1;
                    pathsBetweenLocs[newPath1.End.Id] = newPath2;
                    totalDist += newPath1.TotalDist + newPath2.TotalDist - bestPathToReplace.TotalDist;
                }
            }

            _pathFindingDone = 1;

            var currentNode = _start;
            do {
                _paths.AddRange(pathsBetweenLocs[currentNode.Id].Paths.Take(pathsBetweenLocs[currentNode.Id].Paths.Count - 1));
                currentNode = pathsBetweenLocs[currentNode.Id].End;
            } while (currentNode.Id != _start.Id);

            _paths.Add(new DirectionPath(_start.Lat, _start.Lon, null, true, null, _start.Altitude, _start.Location, _start.Outside));

            yield return TM.Return(currentThread);
        }
    }
}
