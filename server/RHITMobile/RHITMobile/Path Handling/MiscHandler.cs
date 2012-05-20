using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RHITMobile {
    public class FaviconHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.OK));
        }
    }

    public class StatusHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new JsonResponse(String.Format(
                "Last request:\t{0}",
                WebController.PrevRequest)));
        }
    }
}
