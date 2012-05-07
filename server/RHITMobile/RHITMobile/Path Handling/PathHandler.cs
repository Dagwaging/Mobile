using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;

namespace RHITMobile {
    public abstract class PathHandler {
        private Dictionary<string, PathHandler> _redirects = new Dictionary<string, PathHandler>();
        protected Dictionary<string, PathHandler> Redirects {
            get {
                return _redirects;
            }
        }

        protected PathHandler IntRedirect { get; set; }
        protected PathHandler FloatRedirect { get; set; }
        protected PathHandler UnknownRedirect { get; set; }

        public virtual IEnumerable<ThreadInfo> HandlePath(ThreadManager TM, bool isSSL, IEnumerable<string> path, Dictionary<string, string> query, NameValueCollection headers, HttpListenerContext context, object state) {
            var currentThread = TM.CurrentThread;

            yield return TM.Await(currentThread, VerifyHeaders(TM, headers, state)); //TODO: Remove
            state = TM.GetResult(currentThread); //TODO: Remove

            if (path.Any()) {
                if (Redirects.ContainsKey(path.First())) {
                    yield return TM.Await(currentThread, Redirects[path.First()].HandlePath(TM, isSSL, path.Skip(1), query, headers, context, state));
                } else {
                    int intValue;
                    if (Int32.TryParse(path.First(), out intValue)) {
                        yield return TM.Await(currentThread, HandleIntPath(TM, intValue, state));
                        yield return TM.Await(currentThread, (IntRedirect ?? FloatRedirect ?? UnknownRedirect).HandlePath(TM, isSSL, path.Skip(1), query, headers, context, TM.GetResult(currentThread)));
                    } else {
                        double floatValue;
                        if (Double.TryParse(path.First(), out floatValue)) {
                            yield return TM.Await(currentThread, HandleFloatPath(TM, floatValue, state));
                            yield return TM.Await(currentThread, (FloatRedirect ?? UnknownRedirect).HandlePath(TM, isSSL, path.Skip(1), query, headers, context, TM.GetResult(currentThread)));
                        } else {
                            yield return TM.Await(currentThread, HandleUnknownPath(TM, path.First(), state));
                            yield return TM.Await(currentThread, UnknownRedirect.HandlePath(TM, isSSL, path.Skip(1), query, headers, context, TM.GetResult(currentThread)));
                        }
                    }
                }
            } else {
                yield return TM.Await(currentThread, HandleNoPathExtended(TM, query, context, state));
            }

            yield return TM.Return(currentThread);
        }

        protected virtual IEnumerable<ThreadInfo> HandleIntPath(ThreadManager TM, int value, object state) {
            var currentThread = TM.CurrentThread;
            if (IntRedirect != null) {
                yield return TM.Return(currentThread, value);
            } else {
                yield return TM.Await(currentThread, HandleFloatPath(TM, value, state));
                yield return TM.Return(currentThread);
            }
        }

        protected virtual IEnumerable<ThreadInfo> HandleFloatPath(ThreadManager TM, double value, object state) {
            var currentThread = TM.CurrentThread;
            if (FloatRedirect != null) {
                yield return TM.Return(currentThread, value);
            } else {
                yield return TM.Await(currentThread, HandleUnknownPath(TM, value.ToString(), state));
                yield return TM.Return(currentThread);
            }
        }

        protected virtual IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, string path, object state) {
            var currentThread = TM.CurrentThread;
            if (UnknownRedirect == null)
                throw new BadRequestException(currentThread, "Invalid path: /{0}", path);
            yield return TM.Return(currentThread, path);
        }

        protected virtual IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;
            throw new BadRequestException(currentThread, "Invalid end of path.");
        }

        protected virtual IEnumerable<ThreadInfo> HandleNoPathExtended(ThreadManager TM, Dictionary<string, string> query, HttpListenerContext context, object state) {
            return HandleNoPath(TM, query, state);
        }

        public virtual IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, state);
        } // TODO: Remove
    }
}
