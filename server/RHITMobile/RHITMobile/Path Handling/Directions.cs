using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Data.SqlClient;
using System.Data;

namespace RHITMobile {
    public class DirectionsHandler : PathHandler {
        public DirectionsHandler() {
            Redirects.Add("status", new DirectionsStatusHandler());
            Redirects.Add("fromloc", new DirectionsFromLocHandler());
            Redirects.Add("fromgps", new DirectionsFromGpsHandler());
            Redirects.Add("testing", new TestingDirectionsHandler());
        }
    }

    public class DirectionsStatusHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleIntPath(ThreadManager TM, int value, object state) {
            var currentThread = TM.CurrentThread;
            IntRedirect = DirectionsFinder.GetFinder(value);
            if (IntRedirect == null)
                throw new BadRequestException(currentThread, "Invalid Directions ID.");
            yield return TM.Return(currentThread, -1);
        }
    }

    public class DirectionsFromLocHandler : PathHandler {
        public DirectionsFromLocHandler() {
            IntRedirect = new DirectionsToHandler();
        }

        protected override IEnumerable<ThreadInfo> HandleIntPath(ThreadManager TM, int value, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationDepartNode",
                new SqlParameter("@location", value));
            var table = TM.GetResult<DataTable>(currentThread);
            if (table.Rows.Count == 0)
                throw new BadRequestException(currentThread, "Cannot depart from this location.");

            yield return TM.Return(currentThread, new DirectionsFinder(table.Rows[0]));
        }
    }

    public class DirectionsFromGpsHandler : PathHandler {
        public DirectionsFromGpsHandler() {
            FloatRedirect = new DirectionsFromGpsLatHandler();
        }

        protected override IEnumerable<ThreadInfo> HandleFloatPath(ThreadManager TM, double value, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, value);
        }
    }

    public class DirectionsFromGpsLatHandler : PathHandler {
        public DirectionsFromGpsLatHandler() {
            FloatRedirect = new DirectionsToHandler();
        }

        protected override IEnumerable<ThreadInfo> HandleFloatPath(ThreadManager TM, double lon, object state) {
            var currentThread = TM.CurrentThread;
            float lat = (float)state;

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetOutsideNodesNearLat",
                new SqlParameter("@lat", lat));
            var table = TM.GetResult<DataTable>(currentThread);
            if (table.Rows.Count == 0)
                throw new BadRequestException(currentThread, "No outside nodes are in the system.");

            Node closestNode = null;
            double closestDist = Double.MaxValue;
            foreach (DataRow row in table.Rows) {
                var node = new Node(row);
                if (node.HDistanceTo(lat, node.Lon) >= closestDist) break;
                double dist = node.HDistanceTo(lat, lon);
                if (dist < closestDist) {
                    closestDist = dist;
                    closestNode = node;
                }
            }

            yield return TM.Return(currentThread, new DirectionsFinder(closestNode));
        }
    }

    public class DirectionsToHandler : PathHandler {
        public DirectionsToHandler() {
            Redirects.Add("toloc", new DirectionsToLocHandler());
        }
    }

    public class DirectionsToLocHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleIntPath(ThreadManager TM, int value, object state) {
            var currentThread = TM.CurrentThread;
            IntRedirect = (DirectionsFinder)state;
            yield return TM.Return(currentThread, value);
        }
    }
}
