using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RHITMobile
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string Ping(string name);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ServiceImplementation : IService
    {
        public string Ping(string name)
        {
            return "Hello, " + name;
        }
    }

    public static class WebController
    {
        public static IEnumerable<ulong> HandleClients(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;

            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5600/");
            listener.Start();
            while (true)
            {
                yield return TM.WaitForClient(currentThread, listener);
                var context = TM.GetResult<HttpListenerContext>();
                TM.Enqueue(HandleRequests(TM, context));
            }
        }

        public static Dictionary<string, Func<ThreadManager, IEnumerable<string>, Dictionary<string, string>, IEnumerable<ulong>>> RequestHandlers =
            new Dictionary<string, Func<ThreadManager, IEnumerable<string>, Dictionary<string, string>, IEnumerable<ulong>>>()
        {
            { "mapareas", MapAreas.HandleMapAreasRequest },
        };

        private static IEnumerable<ulong> HandleRequests(ThreadManager TM, HttpListenerContext context)
        {
            var currentThread = TM.CurrentThread;
            // http://localhost:5600/favicon.ico
            var path = context.Request.Url.LocalPath.ToLower().Split('/').Skip(1);
            Dictionary<string, string> query = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(context.Request.Url.Query))
            {
                var querySplit = context.Request.Url.Query.Split('?', '&', ';').Skip(1);
                foreach (string field in querySplit)
                {
                    int equalsPos = field.IndexOf('=');
                    query.Add(field.Substring(0, equalsPos), field.Substring(equalsPos + 1, field.Length - equalsPos - 1));
                }
            }
            JsonResponse result = null;
            if (!path.Any())
            {
                result = new JsonResponse(new MessageResponse("Server is active."));
            }
            else
            {
                if (RequestHandlers.ContainsKey(path.First()))
                {
                    yield return TM.Await(currentThread, RequestHandlers[path.First()](TM, path.Skip(1), query));
                    result = TM.GetResult<JsonResponse>();
                }
                else
                {
                    result = new JsonResponse(HttpStatusCode.BadRequest);
                }
            }

            if (result.Json != null)
            {
                var encoding = new ASCIIEncoding();
                byte[] b = encoding.GetBytes(result.Json.Serialize());
                context.Response.OutputStream.Write(b, 0, b.Length);
            }
            context.Response.StatusCode = (int)result.StatusCode;
            context.Response.Close();
            yield return TM.Return(currentThread);
        }

        public static string MakePath(this IEnumerable<string> paths)
        {
            var builder = new StringBuilder();
            foreach (var path in paths)
            {
                builder.Append('/');
                builder.Append(path);
            }
            return builder.ToString();
        }
    }

    public class JsonResponse
    {
        public JsonResponse(JsonObject obj)
        {
            Json = obj;
            StatusCode = HttpStatusCode.OK;
        }

        public JsonResponse(HttpStatusCode code)
        {
            Json = null;
            StatusCode = code;
        }

        public JsonObject Json { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
