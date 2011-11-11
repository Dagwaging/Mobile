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
            DirectionsFinder.EnqueueMonitors(TM);
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
                            Console.WriteLine("update\nstatus\nexecutions\nqueues\nthreads\ndirections\nexit");
                            break;
                        case "update":
                            UpdateServerVersion();
                            break;
                        case "status":
                            TM.WriteExecutionStatus();
                            TM.WriteQueueStatus();
                            TM.WriteThreadStatus();
                            DirectionsFinder.WriteStatus();
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
                            DirectionsFinder.WriteStatus();
                            break;
                        case "exit":
                            System.Environment.Exit(0);
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
        public const string ConnectionString = @"Data Source=mobilewin.csse.rose-hulman.edu\RHITMobile;Initial Catalog=MapData;User Id=server;Password=rhitMobile56";
        public const double EarthRadius = 20925524.9;
        public const double DegToRad = Math.PI / 180;
        public const double MaxSlopeAngle = 10 * DegToRad;
        public static double MaxSlopeRatio = Math.Sin(MaxSlopeAngle);
        public const double StairHeight = 7 / 12;
        public const double StairLength = 11 / 12;
        public const double StairRatio = StairHeight / StairLength;
        public static double StairAngle = Math.Asin(StairRatio);

        public static double UseStairsStairMultiplier = (Math.Sqrt(1 + MaxSlopeRatio * MaxSlopeRatio) * StairRatio - Math.Sqrt(1 + StairRatio * StairRatio) * MaxSlopeRatio) * StairHeight / (MaxSlopeRatio * StairRatio);
    }
}