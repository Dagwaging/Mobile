using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RHITMobile
{
    public abstract class PathHandler
    {
        private Dictionary<string, PathHandler> _redirects = new Dictionary<string, PathHandler>();
        protected Dictionary<string, PathHandler> Redirects
        {
            get
            {
                return _redirects;
            }
        }

        public IEnumerable<ThreadInfo> HandlePath(ThreadManager TM, IEnumerable<string> path, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;

            if (path.Any())
            {
                if (Redirects.ContainsKey(path.First()))
                {
                    yield return TM.Await(currentThread, Redirects[path.First()].HandlePath(TM, path.Skip(1), query));
                }
                else
                {
                    yield return TM.Await(currentThread, HandleUnknownPath(TM, path, query));
                }
            }
            else
            {
                yield return TM.Await(currentThread, HandleNoPath(TM, query));
            }

            yield return TM.Return(currentThread);
        }

        protected virtual IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, IEnumerable<string> path, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
        }

        protected virtual IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
        }
    }
}
