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
                db.ClearData();

                {
                    List<User> users = new List<User>();
                    Dictionary<int, String> termUserMap = new Dictionary<int, String>();
                    String[] userpaths = Directory.GetFiles(inputPath, "*.usr");
                    foreach (String path in userpaths)
                    {
                        using (UserCsvParser parser = new UserCsvParser(path))
                        {
                            termUserMap.Add(parser.TermCode, path);
                        }
                    }
                    String userpath = termUserMap[termUserMap.Keys.Max()];
                    using (UserCsvParser parser = new UserCsvParser(userpath))
                    {
                        foreach (User user in parser)
                        {
                            idToUsername.Add(user.ID, user.Username);
                            users.Add(user);
                            db.AddUser(user);
                        }
                        foreach (User user in users)
                        {
                            if (user.Advisor != null && user.Advisor != "")
                            {
                                db.SetAdvisor(user);
                            }
                        }
                        log.Info("Read " + users.Count + " user entries for term " + parser.TermCode);
                    }
                }
                {
                    String[] coursepaths = Directory.GetFiles(inputPath, "*.cls");
                    foreach (String coursepath in coursepaths)
                    {
                        using (CourseCsvParser parser = new CourseCsvParser(coursepath))
                        {
                            int count = 0;
                            foreach (Course course in parser)
                            {
                                db.AddCourse(course);
                                count++;
                            }
                            log.Info("Read " + count + " course entries for term " + parser.TermCode);
                        }
                    }
                }
                {
                    String[] enrollmentpaths = Directory.GetFiles(inputPath, "*.ssf");
                    foreach (String enrollmentpath in enrollmentpaths)
                    {
                        using (EnrollmentCsvParser parser = new EnrollmentCsvParser(enrollmentpath, idToUsername))
                        {
                            int count = 0;
                            foreach (Enrollment course in parser)
                            {
                                db.AddUserEnrollment(course);
                                count++;
                            }
                            log.Info("Read " + count + " enrollment entries for term " + parser.TermCode);
                        }
                    }
                }
                db.Flip();
            }

            log.Info("Imported Banner Data in " + DateTime.Now.Subtract(startTime).ToString());
        }
    }
}
