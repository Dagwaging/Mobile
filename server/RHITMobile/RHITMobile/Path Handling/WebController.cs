﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RHITMobile
{
    public static class WebController
    {
        public static bool[] RequestsBeingHandled = new bool[10];
        public static int RequestCounter = 0;

        public static IEnumerable<ThreadInfo> HandleClients(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;

            while (true)
            {
                int tries = 1;
                HttpListener listener = null;
                while (true)
                {
                    try
                    {
                        listener = new HttpListener();
                        listener.Prefixes.Add("http://+:5600/");
                        Console.WriteLine("[{0}]\nAttempt #{1} to start HttpListener...", DateTime.Now, tries);
                        listener.Start();
                        Console.WriteLine("HttpListener successfully started.\n");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("HttpListener failed to start with exception:\nMessage: {0}\nStack trace:\n{1}\n", ex.Message, ex.StackTrace);
                        tries++;
                    }
                    if (tries > 5)
                    {
                        Console.WriteLine("[{0}]\nMaximum number of attempts reached.  Try again later.\n", DateTime.Now);
                        yield return TM.Return(currentThread);
                    }
                    else
                    {
                        Console.WriteLine("[{0}]\nWaiting 5 seconds before attemping again...", DateTime.Now);
                        yield return TM.Sleep(currentThread, 5000);
                    }
                }
                Console.WriteLine("[{0}]\nWaiting for requests...\n", DateTime.Now);
                while (true)
                {
                    yield return TM.WaitForClient(currentThread, listener);
                    HttpListenerContext context = null;
                    try
                    {
                        context = TM.GetResult<HttpListenerContext>(currentThread);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("HttpListener threw an exception while waiting for a client:\nMessage: {0}\nStack trace:\n{1}\n\nGoing to restart the HttpListener...", ex.Message, ex.StackTrace);
                        break;
                    }
                    TM.Enqueue(HandleRequest(TM, context));
                }
                listener.Close();
            }
        }

        private static PathHandler _handler = new InitialPathHandler();

        private static IEnumerable<ThreadInfo> HandleRequest(ThreadManager TM, HttpListenerContext context)
        {
            var currentThread = TM.CurrentThread;

            int requestNum = 0;
            string line = "+------------------------------------------------------------------------------+";
            Console.Write(line);
            string url;
            if (context.Request.RawUrl.StartsWith("/admin/authenticate/"))
                url = String.Format("[{0}] {1}", DateTime.Now, "/admin/authenticate/...");
            else
                url = String.Format("[{0}] {1}", DateTime.Now, context.Request.RawUrl);
            if (url.Length > 76)
                url = url.Substring(0, 73) + "...";
            Console.Write("| " + url.PadRight(76, ' ') + " |");

            requestNum = RequestCounter;
            int i = 0;
            while (i < 10 && RequestsBeingHandled[requestNum]) {
                i++;
                RequestCounter = (RequestCounter + 1) % 10;
                requestNum = RequestCounter;
            }
            if (i == 10)
                requestNum = 10;

            RequestCounter = (RequestCounter + 1) % 10;

            if (requestNum < 10)
                line = line.Remove(requestNum * 8 + 1, 6).Insert(requestNum * 8 + 1, "+    +");
            Console.Write(line);

            if (requestNum < 10)
                RequestsBeingHandled[requestNum] = true;

            if (requestNum < 10)
                for (int j = 0; j < 10; j++)
                    if (requestNum == j)
                        Console.Write("  \\  /  ");
                    else if (RequestsBeingHandled[j])
                        Console.Write("   ||   ");
                    else
                        Console.Write("        ");

            for (int j = 0; j < 10; j++)
                if (RequestsBeingHandled[j])
                    Console.Write("   ||   ");
                else
                    Console.Write("        ");

            JsonResponse result = null;

            var path = Uri.UnescapeDataString(context.Request.Url.LocalPath).Split('/').SkipWhile(String.IsNullOrEmpty).TakeWhile(s => !String.IsNullOrEmpty(s));
            Dictionary<string, string> query = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(context.Request.Url.Query))
            {
                var querySplit = Uri.UnescapeDataString(context.Request.Url.Query).Split('?', '&', ';').Skip(1);
                foreach (string field in querySplit)
                {
                    int equalsPos = field.IndexOf('=');
                    if (equalsPos == -1 || equalsPos == 0 || equalsPos == field.Length - 1)
                    {
                        result = new JsonResponse(HttpStatusCode.BadRequest);
                        break;
                    }
                    query.Add(field.Substring(0, equalsPos), field.Substring(equalsPos + 1, field.Length - equalsPos - 1));
                }
            }

            if (result == null)
            {
                yield return TM.Await(currentThread, _handler.HandlePath(TM, path, query, new object()));
                try
                {
                    result = TM.GetResult<JsonResponse>(currentThread);
                }
                catch (Exception)
                {
                    result = new JsonResponse(HttpStatusCode.InternalServerError);
                }
            }

            var encoding = new ASCIIEncoding();
            byte[] b;
            if (result.Json != null)
            {
                b = encoding.GetBytes(result.Json.Serialize());
            }
            else
            {
                b = encoding.GetBytes(result.Message);
            }

            if (requestNum < 10)
                RequestsBeingHandled[requestNum] = false;

            try {
                context.Response.OutputStream.Write(b, 0, b.Length);
                context.Response.StatusCode = (int)result.StatusCode;
                context.Response.Close();

                if (requestNum < 10) {
                    for (int j = 0; j < 10; j++)
                        if (requestNum == j)
                            Console.Write("  DONE  ");
                        else if (RequestsBeingHandled[j])
                            Console.Write("   ||   ");
                        else
                            Console.Write("        ");
                }
            } catch {
                if (requestNum < 10) {
                    for (int j = 0; j < 10; j++)
                        if (requestNum == j)
                            Console.Write(" CANCEL ");
                        else if (RequestsBeingHandled[j])
                            Console.Write("   ||   ");
                        else
                            Console.Write("        ");
                }
            }

            if (requestNum < 10) {
                for (int j = 0; j < 10; j++)
                    if (RequestsBeingHandled[j])
                        Console.Write("   ||   ");
                    else
                        Console.Write("        ");
            }

            yield return TM.Return(currentThread, null);
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

        public static IEnumerable<ThreadInfo> WriteLines(ThreadManager TM) {
            var currentThread = TM.CurrentThread;

            while (true) {
                yield return TM.Sleep(currentThread, 1000);
                for (int j = 0; j < 10; j++)
                    if (RequestsBeingHandled[j])
                        Console.Write("   ||   ");
                    else
                        Console.Write("        ");
            }
        }
    }

    public class InitialPathHandler : PathHandler
    {
        public InitialPathHandler()
        {
            Redirects.Add("locations", new LocationsHandler());
            Redirects.Add("directions", new DirectionsHandler());
            Redirects.Add("tours", new ToursHandler());
            Redirects.Add("admin", new AdminHandler());
            Redirects.Add("services", new ServicesHandler());
            Redirects.Add("clientaccesspolicy.xml", new ClientAccessPolicyHandler());
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state)
        {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new JsonResponse(new VersionResponse()));
        }
    }

    public class JsonResponse
    {
        public JsonResponse(JsonObject obj)
        {
            Json = obj;
            StatusCode = HttpStatusCode.OK;
            Message = "";
        }

        public JsonResponse(HttpStatusCode code)
        {
            Json = null;
            StatusCode = code;
            Message = "";
        }

        public JsonResponse(string message)
        {
            Json = null;
            StatusCode = HttpStatusCode.OK;
            Message = message;
        }

        public JsonObject Json { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
