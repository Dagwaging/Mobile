using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using RHITMobile.Secure;

namespace RHITMobile.Secure
{
    public partial class WindowsService : ServiceBase
    {
        public static String SERVICE_NAME = "RHITMobileSecureServer";

        public ServiceHost ServiceHost { get; private set; }
        public DataMonitor DataMonitor { get; private set; }

        public static WindowsService Instance { get; private set; }

        public Logger Logger { get; private set; }

        public WindowsService()
        {
            InitializeComponent();
            ServiceName = SERVICE_NAME;
            Instance = this;
        }

        protected override void OnStart(string[] args)
        {
            Cleanup();

            Logger = new EventLogger(EventLog);

            ServiceHost = new ServiceHost(typeof(WebService));
            ServiceHost.Open();

            DataMonitor = new DataMonitor(Logger);
            if (!DataMonitor.Start())
            {
                ExitCode = 1;
                Stop();
            }
        }

        protected override void OnStop()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (ServiceHost != null)
            {
                ServiceHost.Close();
                ServiceHost = null;
            }

            if (DataMonitor != null)
            {
                DataMonitor.Stop();
                DataMonitor = null;
            }
        }

        private DateTime _startTime = DateTime.UtcNow;
        public TimeSpan Uptime
        {
            get
            {
                return DateTime.UtcNow.Subtract(_startTime);
            }
        }

        public static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--standalone")
            {
                var service = new WindowsService();
                service.OnStart(args);
                Console.WriteLine("Running server...");
                Console.ReadLine();
                service.OnStop();
                return;
            }

            ServiceBase.Run(new WindowsService());
        }
    }
}
