using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace RHITMobile
{
    public static class Program
    {
        public static void Main()
        {
            try
            {
                UpdateServerVersion();
            }
            catch
            {
                Console.WriteLine("Could not get version.");
                return;
            }
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
                try
                {
                    switch (request)
                    {
                        case "help":
                            Console.WriteLine("update\nstatus\nexecutions\nqueues\nthreads\ndirections");
                            break;
                        case "update":
                            UpdateServerVersion();
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An exception occurred: {0}\nStack trace:\n{1}", ex.Message, ex.StackTrace);
                }
                Console.WriteLine();
            }
        }

        public static void UpdateServerVersion()
        {
            using (var fileReader = new StreamReader("version.txt"))
            {
                ServerVersion = Double.Parse(fileReader.ReadLine());
            }
            Console.WriteLine("Update successful.");
        }

        public static double ServerVersion;
        public static string ConnectionString = @"Data Source=mobilewin.csse.rose-hulman.edu\RHITMobile;Initial Catalog=MapData;User Id=server;Password=rhitMobile56";
    }
}