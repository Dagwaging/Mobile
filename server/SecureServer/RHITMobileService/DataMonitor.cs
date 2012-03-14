using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using RHITMobile.Secure.Data_Import;
using System.Threading;

namespace RHITMobile.Secure
{
    class DataMonitor : IDisposable
    {
        private Logger log;

        private FileSystemWatcher fsWatcher;
        private DataUpdater dataUpdater;
        private UpdateTimer updateTimer;

        public DataMonitor(Logger log)
        {
            this.log = log;
        }

        public bool Start()
        {
            Cleanup();

            String inputPath;
            try
            {
                inputPath = ConfigurationManager.AppSettings["InputPath"];
            }
            catch (ConfigurationErrorsException ex)
            {
                log.Error("InputPath not specified, unable to update Banner data", ex);
                return false;
            }

            dataUpdater = new DataUpdater(log, inputPath);

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

            log.Info("Setup filesystem monitor for " + fsWatcher.Path);

            updateTimer = new UpdateTimer(dataUpdater);

            return true;
        }

        void fsWatcher_Notify(object sender, FileSystemEventArgs e)
        {
            dataUpdater.Update();
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

            if (updateTimer != null)
            {
                updateTimer.Dispose();
                updateTimer = null;
            }

            if (dataUpdater != null)
            {
                dataUpdater.Stop();
                dataUpdater = null;
            }
        }
        
        public void Dispose()
        {
            Stop();
        }
    }

    class UpdateTimer : IDisposable
    {
        private DataUpdater updater;
        private Timer timer;

        public UpdateTimer(DataUpdater updater)
        {
            this.updater = updater;
            timer = new Timer(new TimerCallback(TimerElapsed), null, new TimeSpan(0, 0, 5), new TimeSpan(24, 0, 0));
        }

        private void TimerElapsed(object state)
        {
            updater.Update();
        }

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }
    }

    class DataUpdater
    {
        private Logger log;
        private String inputPath;

        public DataUpdater(Logger log, String inputPath)
        {
            this.log = log;
            this.inputPath = inputPath;
        }

        public void Update()
        {
            log.Info("Update Queued");
            
            //TODO push into a new thread that limits time between executions
            Importer importer = new Importer(log, inputPath);
            importer.ImportData();
        }
        
        public void Stop()
        {

        }

    }
}
