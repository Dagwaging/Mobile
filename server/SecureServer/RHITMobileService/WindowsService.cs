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

namespace RHITMobileService
{
    public partial class WindowsService : ServiceBase
    {
        public static String SERVICE_NAME = "RHITMobileService";

        public ServiceHost serviceHost = null;

        public WindowsService()
        {
            InitializeComponent();
            ServiceName = SERVICE_NAME;
        }

        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }

            serviceHost = new ServiceHost(typeof(WebService));
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }

        public static void Main()
        {
            ServiceBase.Run(new WindowsService());
        }
    }
}
