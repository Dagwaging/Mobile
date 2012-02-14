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
            if (IntRedirect == null) {
                yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
            }
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
            if (table.Rows.Count > 0) {
                yield return TM.Return(currentThread, new DirectionsFinder(table.Rows[0]));
            } else {
                yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
            }
        }
    }

    public class DirectionsFromGpsHandler : PathHandler {
    }

    public class DirectionsToHandler : PathHandler {
        public DirectionsToHandler() {
            Redirects.Add("toloc", new DirectionsToLocHandler());
            Redirects.Add("tobath", new DirectionsToBathHandler());
            Redirects.Add("toprinter", new DirectionsToPrinterHandler());
        }
    }

    public class DirectionsToLocHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleIntPath(ThreadManager TM, int value, object state) {
            var currentThread = TM.CurrentThread;
            IntRedirect = (DirectionsFinder)state;
            yield return TM.Return(currentThread, value);
        }
    }

    public class DirectionsToBathHandler : PathHandler {
    }

    public class DirectionsToPrinterHandler : PathHandler {
    }
}
