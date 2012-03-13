using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace RHITMobileService.Data_Import
{
    class Importer
    {
        private EventLog log;
        private String inputPath;

        public Importer(EventLog log, String inputPath)
        {
            this.log = log;
            this.inputPath = inputPath;
        }

        public void ImportData()
        {
            string[] userpaths = Directory.GetFiles(inputPath, "*.usr");
            foreach (string userpath in userpaths)
            {
                UserCsvParser parser = new UserCsvParser(userpath);
                int count = 0;
                foreach (User user in parser)
                {
                    //TODO something
                    count++;
                }
                log.WriteEntry("Read " + count + " user entries for term " + parser.TermCode, EventLogEntryType.Information);

            }
            
        }
    }
}
