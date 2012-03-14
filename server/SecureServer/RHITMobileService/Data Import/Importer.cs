using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using RHITMobile.Secure.Data;

namespace RHITMobile.Secure.Data_Import
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
            Dictionary<String, String> idToUsername = new Dictionary<String, String>();

            string[] userpaths = Directory.GetFiles(inputPath, "*.usr");
            foreach (string userpath in userpaths)
            {
                UserCsvParser parser = new UserCsvParser(userpath);
                int count = 0;
                foreach (User user in parser)
                {
                    idToUsername.Add(user.ID, user.Username);

                    //TODO something
                    count++;
                }
                log.WriteEntry("Read " + count + " user entries for term " + parser.TermCode, EventLogEntryType.Information);
            }
            
            string[] coursepaths = Directory.GetFiles(inputPath, "*.cls");
            foreach (string coursepath in coursepaths)
            {
                CourseCsvParser parser = new CourseCsvParser(coursepath);
                int count = 0;
                foreach (Course course in parser)
                {
                    //TODO something
                    count++;
                }
                log.WriteEntry("Read " + count + " course entries for term " + parser.TermCode, EventLogEntryType.Information);
            }
            
        }
    }
}
