using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RHITMobile
{
    public class ClientAccessPolicyHandler : PathHandler
    {
        public const string File = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" 
            + "<access-policy>\n"
            + "<cross-domain-access>\n"
            + "<policy>\n"
            + "<allow-from http-request-headers=\"*\">\n" 
            + "<domain uri=\"*\"/>\n" 
            + "</allow-from>\n"
            + "<grant-to>\n"
            + "<resource path=\"/\" include-subpaths=\"true\"/>\n" 
            + "</grant-to>\n"
            + "</policy>\n"
            + "</cross-domain-access>\n"
            + "</access-policy>";

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state)
        {
            var currentThread = TM.CurrentThread;

            yield return TM.Return(currentThread, new JsonResponse(File));
        }
    }
}
