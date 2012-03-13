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
        public static String SERVICE_NAME = "RHITMobileService";

        public ServiceHost serviceHost = null;
        private DataMonitor dataMonitor = null;

        public WindowsService()
        {
            InitializeComponent();
            ServiceName = SERVICE_NAME;
        }

        protected override void OnStart(string[] args)
        {
            Cleanup();

            serviceHost = new ServiceHost(typeof(WebService));
            serviceHost.Open();

            dataMonitor = new DataMonitor(EventLog);
            dataMonitor.Start();
        }

        protected override void OnStop()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }

            if (dataMonitor != null)
            {
                dataMonitor.Stop();
                dataMonitor = null;
            }
        }

        public static void Main()
        {
            ServiceBase.Run(new WindowsService());
        }
    }
}
