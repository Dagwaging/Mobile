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
        //TODO use a database
        public static List<User> users;
        public static List<Course> courses;
        public static List<Enrollment> enrollment;

        private Logger log;
        private String inputPath;

        public Importer(Logger log, String inputPath)
        {
            this.log = log;
            this.inputPath = inputPath;
        }

        public void ImportData()
        {
            Dictionary<String, String> idToUsername = new Dictionary<String, String>();

            DB db = DB.Instance;
            db.ClearData();

            {
                users = new List<User>();
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
                courses = new List<Course>();
                String[] coursepaths = Directory.GetFiles(inputPath, "*.cls");
                foreach (String coursepath in coursepaths)
                {
                    using (CourseCsvParser parser = new CourseCsvParser(coursepath))
                    {
                        foreach (Course course in parser)
                        {
                            courses.Add(course);
                            db.AddCourse(course);
                        }
                        log.Info("Read " + courses.Count + " course entries for term " + parser.TermCode);
                    }
                }
            }
            {
                enrollment = new List<Enrollment>();
                String[] enrollmentpaths = Directory.GetFiles(inputPath, "*.ssf");
                foreach (String enrollmentpath in enrollmentpaths)
                {
                    using (EnrollmentCsvParser parser = new EnrollmentCsvParser(enrollmentpath, idToUsername))
                    {
                        foreach (Enrollment course in parser)
                        {
                            enrollment.Add(course);
                            db.AddUserEnrollment(course);
                        }
                        log.Info("Read " + enrollment.Count + " enrollment entries for term " + parser.TermCode);
                    }
                }
            }
            db.Flip();

        }
    }
}
