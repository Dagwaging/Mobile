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
            // Initialize the connection string
            InitConnection();

            try {
                // Get the server version from the external text file
                UpdateServerVersion();
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
                            Console.WriteLine("update\nstatus\nexecutions\nqueues\nthreads\ndirections\ntours\nexit");
                            break;
                        case "update":
                            UpdateServerVersion();
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

        /// <summary>
        /// Set the server version based on an external text file
        /// </summary>
        public static void UpdateServerVersion() {
            using (var fileReader = new StreamReader("version.txt")) {
                string line;
                while ((line = fileReader.ReadLine()) != null) {
                    switch (line.Substring(0, line.IndexOf(':'))) {
                        case "Locations":
                            LocationsVersion = Double.Parse(line.Substring(line.IndexOf(':') + 1));
                            break;
                        case "Services":
                            ServicesVersion = Double.Parse(line.Substring(line.IndexOf(':') + 1));
                            break;
                        case "Tags":
                            TagsVersion = Double.Parse(line.Substring(line.IndexOf(':') + 1));
                            break;
                    }
                }
            }
            Console.WriteLine("Version update from file successful.");
        }

        public static void WriteVersions(double locations, double services, double tags) {
            LocationsVersion = locations;
            ServicesVersion = services;
            TagsVersion = tags;
            using (var fileWriter = new StreamWriter("version.txt", false)) {
                fileWriter.WriteLine("Locations:\t" + locations);
                fileWriter.WriteLine("Services:\t" + services);
                fileWriter.WriteLine("Tags:\t\t" + tags);
            }
            Console.WriteLine("Version was updated.");
        }

        public static string GetConnectionString(string username, string password) {
            return String.Format(CustomConnectionString, username, password);
        }

        public static string ConnectionString = null;
        public static void InitConnection() {
            if (ConnectionString == null) {
                ConnectionString = @"Data Source=localhost\RHITMobile;Initial Catalog=MapData;Integrated Security=SSPI;Persist Security Info=true";
                try {
                    Console.WriteLine("Attempting to login to SQL Server using Windows credentials...");
                    using (var connection = new SqlConnection(ConnectionString)) {
                        connection.Open();
                    }
                } catch {
                    Console.WriteLine("Could not connect using Windows credentials.");
                    Console.WriteLine();
                    bool success = false;
                    while (!success) {
                        try {
                            Console.Write("Enter the user ID: ");
                            string userId = Console.ReadLine();
                            Console.Write("Enter the password: ");
                            string password = Console.ReadLine();
                            ConnectionString = GetConnectionString(userId, password);
                            using (var connection = new SqlConnection(ConnectionString)) {
                                connection.Open();
                            }
                            success = true;
                        } catch {
                            Console.WriteLine("Incorrect SQL credentials.");
                            Console.WriteLine();
                        }
                    }
                }

                Console.WriteLine("Login successful.");
                Console.WriteLine();
            }
        }

        public static double LocationsVersion;
        public static double ServicesVersion;
        public static double TagsVersion;
        private const string CustomConnectionString = @"Data Source=tcp:mobilewin.csse.rose-hulman.edu,4848\RHITMobile;Initial Catalog=MapData;User Id={0};Password={1};Persist Security Info=true";
        public const double EarthRadius = 20925524.9; // feet
        public const double DegToRad = Math.PI / 180;
        public const double MaxSlopeAngle = 10 * DegToRad; // radians
        public static double MaxSlopeRatio = Math.Sin(MaxSlopeAngle);
        public const double StairHeight = 7 / 12;
        public const double StairLength = 11 / 12;
        public const double StairRatio = StairHeight / StairLength;
        public static double StairAngle = Math.Asin(StairRatio); // radians
        public const int MaxDailySecureServerCalls = 25000;

        public static double UseStairsStairMultiplier = (Math.Sqrt(1 + MaxSlopeRatio * MaxSlopeRatio) * StairRatio - Math.Sqrt(1 + StairRatio * StairRatio) * MaxSlopeRatio) * StairHeight / (MaxSlopeRatio * StairRatio);
    }
}