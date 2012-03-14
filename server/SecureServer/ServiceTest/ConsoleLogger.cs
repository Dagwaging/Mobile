using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RHITMobile.Secure;

namespace ServiceTest
{
    class ConsoleLogger : Logger
    {
        public void Info(string message)
        {
            System.Console.Error.WriteLine("INFO: " + message);
        }

        public void Error(string message)
        {
            System.Console.Error.WriteLine("ERROR: " + message);
        }

        public void Error(string message, Exception ex)
        {
            System.Console.Error.WriteLine("ERROR: " + message);
            System.Console.Error.WriteLine("Exception details: " + ex.ToString());
        }
    }
}
