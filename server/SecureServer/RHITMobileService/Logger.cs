using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RHITMobile.Secure
{
    public interface Logger
    {
        void Info(string message);
        void Error(string message);
        void Error(string message, Exception ex);
    }

    class EventLogger : Logger
    {
        private EventLog log;

        public EventLogger(EventLog log)
        {
            this.log = log;
        }

        public void Info(string message)
        {
            log.WriteEntry(message, EventLogEntryType.Information);
        }
        
        public void Error(string message)
        {
            log.WriteEntry(message, EventLogEntryType.Error);
        }
        
        public void Error(string message, Exception ex)
        {
            log.WriteEntry(message + "\n\n" + ex.ToString(), EventLogEntryType.Error);
        }
    }

    class NullLogger : Logger
    {
        public void Info(string message)
        { }
        public void Error(string message)
        { }
        public void Error(string message, Exception ex)
        { }
    }
}
