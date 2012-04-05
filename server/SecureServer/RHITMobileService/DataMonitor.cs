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
        private Logger _log;

        private FileSystemWatcher _fsWatcher;
        private DataUpdater _dataUpdater;
        private UpdateTimer _updateTimer;

        public DataMonitor(Logger log)
        {
            _log = log;
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
                _log.Error("InputPath not specified, unable to update Banner data", ex);
                return false;
            }

            _dataUpdater = new DataUpdater(_log, inputPath);

            _fsWatcher = new FileSystemWatcher();
            _fsWatcher.Path = inputPath;
            _fsWatcher.NotifyFilter = NotifyFilters.CreationTime |
                NotifyFilters.LastWrite |
                NotifyFilters.Attributes |
                NotifyFilters.Security |
                NotifyFilters.Size;

            _fsWatcher.Changed += new FileSystemEventHandler(fsWatcher_Notify);
            _fsWatcher.Created += new FileSystemEventHandler(fsWatcher_Notify);
            _fsWatcher.Deleted += new FileSystemEventHandler(fsWatcher_Notify);

            _fsWatcher.EnableRaisingEvents = true;

            _log.Info("Setup filesystem monitor for " + _fsWatcher.Path);

            _updateTimer = new UpdateTimer(_dataUpdater);

            return true;
        }

        void fsWatcher_Notify(object sender, FileSystemEventArgs e)
        {
            _dataUpdater.Update();
        }

        public void Stop()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (_fsWatcher != null)
            {
                _fsWatcher.EnableRaisingEvents = false;
                _fsWatcher.Dispose();
                _fsWatcher = null;
            }

            if (_updateTimer != null)
            {
                _updateTimer.Dispose();
                _updateTimer = null;
            }

            if (_dataUpdater != null)
            {
                _dataUpdater.Stop();
                _dataUpdater = null;
            }
        }
        
        public void Dispose()
        {
            Stop();
        }
    }

    class UpdateTimer : IDisposable
    {
        private DataUpdater _updater;
        private Timer _timer;

        public UpdateTimer(DataUpdater updater)
        {
            _updater = updater;
            _timer = new Timer(new TimerCallback(TimerElapsed), null, new TimeSpan(0, 0, 5), new TimeSpan(24, 0, 0));
        }

        private void TimerElapsed(object state)
        {
            _updater.Update();
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }

    class DataUpdater
    {
        private Logger _log;
        private String _inputPath;

        private Thread _updateThread;
        private UpdaterThreadData _updaterThreadData;
        private AutoResetEvent _updateEvent;

        public DataUpdater(Logger log, String inputPath)
        {
            _log = log;
            _inputPath = inputPath;

            _updaterThreadData = new UpdaterThreadData();
            _updateEvent = new AutoResetEvent(false);
            _updateThread = new Thread(new ThreadStart(UpdaterThread));
            _updateThread.Start();
        }

        public void Update()
        {
            lock (_updaterThreadData)
            {
                if (!_updaterThreadData.updateQueued)
                {
                    _log.Info("Update Queued");
                }
                _updaterThreadData.updateQueued = true;
            }
            _updateEvent.Set();
        }

        private bool CheckUpdateQueued()
        {
            lock (_updaterThreadData)
            {
                bool res = _updaterThreadData.updateQueued;
                if (res)
                    _updaterThreadData.updateQueued = false;
                return res;
            }
        }

        private void UpdaterThread()
        {
            while (true)
            {
                //wait for an update to be queued
                while (!CheckUpdateQueued())
                {
                    _updateEvent.WaitOne();
                }

                //import the data
                Thread.BeginCriticalRegion();
                try
                {
                    Importer importer = new Importer(_log, _inputPath);
                    importer.ImportData();
                }
                catch (ThreadAbortException)
                {
                    _log.Info("Aborted data import");
                }
                catch (Exception ex)
                {
                    _log.Error("Data import failed", ex);
                }
                Thread.EndCriticalRegion();

                //wait for a few hours in between requests
                Thread.Sleep(TimeSpan.FromHours(4));
            }
        }
        
        public void Stop()
        {
            lock (_updaterThreadData)
            {
                _updaterThreadData.updateQueued = false;
            }
            _updateThread.Abort();
            _updateThread = null;
        }

        private class UpdaterThreadData
        {
            public bool updateQueued;
        }
    }
}
