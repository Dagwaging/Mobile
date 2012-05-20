using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RHITMobileWindowsService {
    public partial class RHITMobileWindowsService : ServiceBase {
        public RHITMobileWindowsService() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            EventWriter.write("Starting RHITMobile Service", EventLogEntryType.Information);
            var TM = RHITMobile.Program.InitService();
            Task.Factory.StartNew(TM.Start);
        }

        protected override void OnStop() {
            EventWriter.write("Stopping RHITMobile Service", EventLogEntryType.Information);
        }
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
