using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace RHITMobile
{
    public static class Program
    {
        public static void Main()
        {
            var TM = new ThreadManager();
            Directions.EnqueueMonitors(TM);
            TM.Enqueue(HandleConsoleRequests(TM), ThreadPriority.Low);
            TM.Enqueue(WebController.HandleClients(TM));
            TM.Start(1);
        }

        public static IEnumerable<ThreadInfo> HandleConsoleRequests(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;
            while (true)
            {
                yield return TM.WaitForConsole(currentThread);
                string request = TM.GetResult<string>(currentThread);
                switch (request)
                {
                    case "help":
                        Console.WriteLine("status\nexecutions\nqueues\nthreads\ndirections");
                        break;
                    case "status":
                        TM.WriteExecutionStatus();
                        TM.WriteQueueStatus();
                        TM.WriteThreadStatus();
                        Directions.WriteStatus();
                        break;
                    case "executions":
                        TM.WriteExecutionStatus();
                        break;
                    case "queues":
                        TM.WriteQueueStatus();
                        break;
                    case "threads":
                        TM.WriteThreadStatus();
                        break;
                    case "directions":
                        Directions.WriteStatus();
                        break;
                    default:
                        Console.WriteLine("Invalid request.");
                        break;
                }
                Console.WriteLine();
            }
        }

        public static double ServerVersion = 0.11;
        public static string ConnectionString = @"Data Source=mobilewin.csse.rose-hulman.edu\RHITMobile;Initial Catalog=MapData;User Id=server;Password=rhitMobile56";
    }
}