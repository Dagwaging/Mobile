using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace RHITMobile {
    /// <summary>
    /// Static class for starting the application, handling console commands, and storing global parameters
    /// </summary>
    public static class Program {
        /// <summary>
        /// Main entry for the application
        /// </summary>
        public static void Main() {
            try {
                // Get the server version from the external text file
                UpdateServerVersion();
            } catch {
                // If could not get the version, default it to 0.0
                Console.WriteLine("Could not get version.");
                return;
            }

            var TM = new ThreadManager();

            // Start the monitors for the Directions Handler
            DirectionsFinder.EnqueueMonitors(TM);

            // Start the expiration checker for admin logins
            TM.Enqueue(AdminHandler.DeleteExpiredLogins(TM), ThreadPriority.Low);

            // Start the listener for console commands
            TM.Enqueue(HandleConsoleRequests(TM), ThreadPriority.Low);

            // Start the listener for HTTP requests
            TM.Enqueue(WebController.HandleClients(TM));

            // Start the ThreadManager
            TM.Start(1);
        }

        /// <summary>
        /// Handles console commands
        /// </summary>
        /// <param name="TM">Thread Manager</param>
        /// <returns>No return</returns>
        public static IEnumerable<ThreadInfo> HandleConsoleRequests(ThreadManager TM) {
            var currentThread = TM.CurrentThread;
            while (true) {
                yield return TM.WaitForConsole(currentThread);
                string request = TM.GetResult<string>(currentThread);
                try {
                    switch (request) {
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
                } catch (Exception ex) {
                    Console.WriteLine("An exception occurred: {0}\nStack trace:\n{1}", ex.Message, ex.StackTrace);
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Set the server version based on an external text file
        /// </summary>
        public static void UpdateServerVersion() {
            using (var fileReader = new StreamReader("version.txt")) {
                ServerVersion = Double.Parse(fileReader.ReadLine());
            }
            Console.WriteLine("Update successful.");
        }

        public static void WriteServerVersion(double version) {
            ServerVersion = version;
            using (var fileWriter = new StreamWriter("version.txt", false)) {
                fileWriter.Write(version.ToString());
            }
            Console.WriteLine("Version was updated.");
        }

        public static string GetConnectionString(string username, string password) {
            return String.Format(AdminConnectionString, username, password);
        }

        public static double ServerVersion;
        public const string ConnectionString = @"Data Source=mobilewin.csse.rose-hulman.edu\RHITMobile;Initial Catalog=MapData;User Id=server;Password=rhitMobile56";
        private const string AdminConnectionString = @"Data Source=mobilewin.csse.rose-hulman.edu\RHITMobile;Initial Catalog=MapData;User Id={0};Password={1}";
        public const double EarthRadius = 20925524.9; // feet
        public const double DegToRad = Math.PI / 180;
        public const double MaxSlopeAngle = 10 * DegToRad; // radians
        public static double MaxSlopeRatio = Math.Sin(MaxSlopeAngle);
        public const double StairHeight = 7 / 12;
        public const double StairLength = 11 / 12;
        public const double StairRatio = StairHeight / StairLength;
        public static double StairAngle = Math.Asin(StairRatio); // radians

        public static double UseStairsStairMultiplier = (Math.Sqrt(1 + MaxSlopeRatio * MaxSlopeRatio) * StairRatio - Math.Sqrt(1 + StairRatio * StairRatio) * MaxSlopeRatio) * StairHeight / (MaxSlopeRatio * StairRatio);
    }
}