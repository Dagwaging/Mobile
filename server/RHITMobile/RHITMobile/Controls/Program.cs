using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Diagnostics;
using RHITMobile.Properties;

namespace RHITMobile {
    /// <summary>
    /// Static class for starting the application, handling console commands, and storing global parameters
    /// </summary>
    public static class Program {
        /// <summary>
        /// Main entry for the application
        /// </summary>
        public static void Main() {
            InitService().Start();
        }

        public static ThreadManager InitService() {
            // Initialize the connection string
            InitConnection();

            try {
                // Get the server version from the external text file
                using (var connection = new SqlConnection(Program.ConnectionString)) {
                    connection.Open();
                    using (var command = connection.CreateCommand()) {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "spGetVersions";
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                switch ((string)reader["name"]) {
                                    case "Locations":
                                        LocationsVersion = (double)reader["version"];
                                        break;
                                    case "Services":
                                        ServicesVersion = (double)reader["version"];
                                        break;
                                    case "Tags":
                                        TagsVersion = (double)reader["version"];
                                        break;
                                    default:
                                        // TODO: Unknown version
                                        break;
                                }
                            }
                        }
                    }
                }
            } catch {
                // If could not get the version, default it to 0.0
                Console.WriteLine("Could not get version.");
            }

            var TM = new ThreadManager();

            // Start the monitors for the Directions and Tours Handlers
            DirectionsFinder.EnqueueMonitors(TM);
            TourFinder.EnqueueMonitors(TM);

            TM.Enqueue(WebController.WriteLines(TM), ThreadPriority.Low);

            // Start the expiration checker for admin logins
            TM.Enqueue(AdminHandler.DeleteExpiredLogins(TM), ThreadPriority.Low);

            // Start the expiration checker for banner logins
            TM.Enqueue(BannerHandler.ClearAuthenticationExpirations(TM), ThreadPriority.Low);

            // Start the listener for console commands
            //TM.Enqueue(HandleConsoleRequests(TM), ThreadPriority.Low);
            TM.Enqueue(Monitors.SetPartitionAndLocationBoundaries(TM));

            // Start the listener for HTTP requests
            TM.Enqueue(WebController.HandleClients(TM));

            return TM;
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
                            Console.WriteLine("status\nexecutions\nqueues\nthreads\ndirections\ntours\nexit");
                            break;
                        case "status":
                            TM.WriteExecutionStatus();
                            TM.WriteQueueStatus();
                            TM.WriteThreadStatus();
                            DirectionsFinder.WriteStatus();
                            TourFinder.WriteStatus();
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
                        case "tours":
                            TourFinder.WriteStatus();
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

        public static IEnumerable<ThreadInfo> UpdateVersion(ThreadManager TM, string name, double version) {
            var currentThread = TM.CurrentThread;
            TM.MakeDbCall(currentThread, Program.ConnectionString, "spUpdateVersion",
                new SqlParameter("name", name),
                new SqlParameter("version", version));
            yield return TM.Return(currentThread);
        }

        public static string GetConnectionString(string username, string password) {
            return String.Format(CustomConnectionString, username, password);
        }

        public static string ConnectionString = null;
        public static void InitConnection() {
            if (ConnectionString == null) {
                ConnectionString = Settings.Default.MapDataConnectionString;
                Console.WriteLine("Attempting to login to SQL Serve...");
                using (var connection = new SqlConnection(ConnectionString)) {
                    connection.Open();
                }

                Console.WriteLine("Login successful.");
                Console.WriteLine();
            }
        }

        public static double LocationsVersion;
        public static double TagsVersion;
        public static double ServicesVersion;
        private static string CustomConnectionString = Settings.Default.AdminConnectionString + @"User Id={0};Password={1};Persist Security Info=true";
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

    public static class EventWriter {
        private static string source = "RHITMobileWindowsService"; //Must be the same as the name of the windows service
        private static string log = "Application";
        static EventWriter() {
            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, log);
        }

        public static void write(string msg, EventLogEntryType type) {
            EventLog.WriteEntry(source, msg, type);
        }
    }
}