using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RHITMobile.Secure.Data;
using System.Transactions;

namespace RHITMobile.Secure.Data_Import
{
    public class Importer
    {
        private Logger log;
        private String inputPath;

        private static int ERRORS_MAX = 3;

        public Importer(Logger log, String inputPath)
        {
            this.log = log;
            this.inputPath = inputPath;
        }

        public void ImportData()
        {
            DateTime startTime = DateTime.Now;
            Dictionary<String, String> idToUsername = new Dictionary<String, String>();

            DB db = DB.Instance;
            using (var writeLock = db.AcquireWriteLock())
            {
                ParseErrors = 0;
                db.ClearData();

                {
                    List<User> users = new List<User>();
                    Dictionary<int, String> termUserMap = new Dictionary<int, String>();
                    String[] userpaths = Directory.GetFiles(inputPath, "*.usr");
                    foreach (String path in userpaths)
                    {
                        using (UserCsvParser parser = new UserCsvParser(log, path))
                        {
                            termUserMap.Add(parser.TermCode, path);
                        }
                    }
                    String userpath = termUserMap[termUserMap.Keys.Max()];
                    using (UserCsvParser parser = new UserCsvParser(log, userpath))
                    {
                        int errors = 0;
                        foreach (User user in parser)
                        {
                            idToUsername.Add(user.ID, user.Username);
                            try
                            {
                                db.AddUser(user);
                                users.Add(user);
                            }
                            catch (Exception e)
                            {
                                if (errors < ERRORS_MAX)
                                    log.Error(string.Format("Error importing user (line {0})", parser.LineNumber), e);
                                errors++;
                            }
                        }
                        if (errors > 0)
                        {
                            log.Error(string.Format("Failed to import {0} users", errors));
                            ParseErrors += errors;
                        }

                        errors = 0;
                        foreach (User user in users)
                        {
                            if (user.Advisor != null && user.Advisor != "")
                            {
                                try
                                {
                                    db.SetAdvisor(user);
                                }
                                catch (Exception e)
                                {
                                    if (errors < ERRORS_MAX)
                                        log.Error(string.Format("Error setting user's advisor (user {0})", user.Username), e);
                                    errors++;
                                }
                            }
                        }
                        if (errors > 0)
                        {
                            log.Error(string.Format("Failed to set {0} user's advisor", errors));
                            ParseErrors += errors;
                        }
                        ParseErrors += parser.ParseErrors;
                        log.Info("Read " + users.Count + " user entries for term " + parser.TermCode);
                    }
                }
                {
                    String[] coursepaths = Directory.GetFiles(inputPath, "*.cls");
                    foreach (String coursepath in coursepaths)
                    {
                        using (CourseCsvParser parser = new CourseCsvParser(log, coursepath))
                        {
                            int count = 0;
                            int errors = 0;
                            foreach (Course course in parser)
                            {
                                try
                                {
                                    db.AddCourse(course);
                                    count++;
                                }
                                catch (Exception e)
                                {
                                    if (errors < ERRORS_MAX)
                                        log.Error(string.Format("Error importing course (line {0})", parser.LineNumber), e);
                                    errors++;
                                }
                            }
                            if (errors > 0)
                            {
                                log.Error(string.Format("Failed to import {0} courses", errors));
                                ParseErrors += errors;
                            }
                            ParseErrors += parser.ParseErrors;
                            log.Info("Read " + count + " course entries for term " + parser.TermCode);
                        }
                    }
                }
                {
                    String[] enrollmentpaths = Directory.GetFiles(inputPath, "*.ssf");
                    foreach (String enrollmentpath in enrollmentpaths)
                    {
                        using (EnrollmentCsvParser parser = new EnrollmentCsvParser(log, enrollmentpath, idToUsername))
                        {
                            int count = 0;
                            int errors = 0;
                            foreach (Enrollment course in parser)
                            {
                                try
                                {
                                    db.AddUserEnrollment(course);
                                    count++;
                                }
                                catch (Exception e)
                                {
                                    if (errors < ERRORS_MAX)
                                        log.Error(string.Format("Error importing enrollment entry (line {0})", parser.LineNumber), e);
                                    errors++;
                                }
                            }
                            if (errors > 0)
                            {
                                log.Error(string.Format("Failed to import {0} enrollment entries", errors));
                                ParseErrors += errors;
                            }
                            ParseErrors += parser.ParseErrors;
                            log.Info("Read " + count + " enrollment entries for term " + parser.TermCode);
                        }
                    }
                }
                db.Flip();
            }

            if (ParseErrors > 0)
                log.Error(string.Format("{0} errors were encountered while importing banner data", ParseErrors));
            log.Info("Imported Banner Data in " + DateTime.Now.Subtract(startTime).ToString());
        }

        public int ParseErrors { get; private set; }
    }
}
