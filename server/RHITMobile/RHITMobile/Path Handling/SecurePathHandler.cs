using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;

namespace RHITMobile {
    public class SecurePathHandler : PathHandler {
        public override IEnumerable<ThreadInfo> HandlePath(ThreadManager TM, bool isSSL, IEnumerable<string> path, Dictionary<string, string> query, NameValueCollection headers, object state) {
            var currentThread = TM.CurrentThread;

            if (!isSSL)
                throw new BadRequestException("SSL required for this request.");

            yield return TM.Await(currentThread, VerifyHeaders(TM, headers, state));

            yield return TM.Await(currentThread, base.HandlePath(TM, true, path, query, headers, TM.GetResult(currentThread)));

            yield return TM.Return(currentThread);
        }

        public override IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, state);
        }
    }
}
