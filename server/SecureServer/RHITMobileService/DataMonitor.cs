using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using RHITMobile.Secure.Data_Import;

namespace RHITMobile.Secure
{
    class DataMonitor
    {
        private EventLog log;

        private String inputPath;
        private FileSystemWatcher fsWatcher;

        public DataMonitor(EventLog log)
        {
            this.log = log;
        }

        public void Start()
        {
            Cleanup();

            try
            {
                inputPath = ConfigurationManager.AppSettings["InputPath"];
            }
            catch (ConfigurationErrorsException)
            {
                log.WriteEntry("InputPath not specified, unable to update Banner data", EventLogEntryType.Error);
                return;
            }

            fsWatcher = new FileSystemWatcher();
            fsWatcher.Path = inputPath;
            fsWatcher.NotifyFilter = NotifyFilters.CreationTime |
                NotifyFilters.LastWrite |
                NotifyFilters.Attributes |
                NotifyFilters.Security |
                NotifyFilters.Size;

            fsWatcher.Changed += new FileSystemEventHandler(fsWatcher_Notify);
            fsWatcher.Created += new FileSystemEventHandler(fsWatcher_Notify);
            fsWatcher.Deleted += new FileSystemEventHandler(fsWatcher_Notify);

            fsWatcher.EnableRaisingEvents = true;

            log.WriteEntry("Setup filesystem monitor for " + fsWatcher.Path, EventLogEntryType.Information);

            //TODO start a slow polling monitor
        }

        void fsWatcher_Notify(object sender, FileSystemEventArgs e)
        {
            Importer importer = new Importer(log, inputPath);
            importer.ImportData();
        }

        public void Stop()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (fsWatcher != null)
            {
                fsWatcher.EnableRaisingEvents = false;
                fsWatcher.Dispose();
                fsWatcher = null;
            }
        }
    }
}
