using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestClient.RHIT.Private;

namespace TestClient
{
    class Program
    {
        private static string token;
        private static int term;
        private static IWebService service = new WebServiceClient();

        static void Main(string[] args)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = ReadPassword();

            var auth = service.Login(username, password);

            if (auth == null)
            {
                Console.WriteLine("Unable to login");
                Environment.Exit(1);
            }
            token = auth.Token;

            {
                var terms = service.GetTerms(token);
                term = terms.Last();
                Console.WriteLine(string.Join(", ", terms));
                Console.WriteLine("Using term {0}", term);
            }

            int option;
            do
            {
                Console.WriteLine("1. Get server status");
                Console.WriteLine("2. Request update");
                Console.WriteLine("3. Search Users");
                Console.WriteLine("4. Lookup User");
                Console.WriteLine("5. Search Courses");
                Console.WriteLine("6. Lookup Course");
                Console.WriteLine("9. Quit");
                Console.Write("> ");
                option = int.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        ServerStatus();
                        break;
                    case 2:
                        RequestUpdate();
                        break;
                    case 3:
                        SearchUser();
                        break;
                    case 4:
                        LookupUser();
                        break;
                    case 5:
                        SearchCourses();
                        break;
                    case 6:
                        LookupCourse();
                        break;
                }

            } while (option != 9);

            service.Logout(token);
        }

        static void ServerStatus()
        {
            var state = service.GetState();

            Console.WriteLine("Last update:       {0}", state.LastUpdateTime);
            Console.WriteLine("Parse Errors:      {0}", state.ParseErrors);
            Console.WriteLine("Update Queued:     {0}", state.IsUpdateQueued);
            Console.WriteLine("Active Requests:   {0}", state.ActiveRequests);
            Console.WriteLine("Active User Count: {0}", state.ActiveUserCount);
            Console.WriteLine("Records Affected:  {0}", state.LastRecordsAffected);
            Console.WriteLine("Uptime:            {0}", state.Uptime);
            Console.WriteLine("Request Count:     {0}", state.RequestCount);
        }

        static void RequestUpdate()
        {
            service.RequestUpdate();
        }

        static void SearchUser()
        {
            Console.Write("Search: ");
            string search = Console.ReadLine();
            var users = service.SearchUsers(token, search);

            foreach (var user in users)
                PrintUser(user);
        }

        static void LookupUser()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            var user = service.GetUser(token, username);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return;
            }

            PrintUser(user);

            var userEnrollment = service.GetUserEnrollment(token, term, username);
            if (userEnrollment != null && userEnrollment.Length > 0)
            {
                Console.WriteLine("Student Schedule: ");
                PrintEnrollment(userEnrollment);
            }

            var profEnrollment = service.GetInstructorSchedule(token, term, username);
            if (profEnrollment != null && profEnrollment.Length > 0)
            {
                Console.WriteLine("Instructor Schedule: ");
                PrintEnrollment(profEnrollment);
            }
        }

        static void SearchCourses()
        {
            Console.Write("Search: ");
            string search = Console.ReadLine();
            var courses = service.SearchCourses(token, term, search);

            foreach (var course in courses)
                PrintCourse(course);
        }

        static void LookupCourse()
        {
            Console.Write("crn: ");
            int crn = int.Parse(Console.ReadLine());
            var course = service.GetCourse(token, term, crn);

            PrintCourse(course);

            var enrollment = service.GetCourseEnrollment(token, term, crn);
            Console.WriteLine("Students: {0}", string.Join(", ", enrollment));

            var schedule = service.GetCourseSchedule(token, term, crn);
            Console.WriteLine("Schedule:");
            foreach (var meeting in schedule)
                Console.WriteLine("- {0} from {1} to {2} in {3}", meeting.Day, meeting.StartPeriod, meeting.EndPeriod, meeting.Room);
        }

        static void PrintCourse(Course course)
        {
            Console.WriteLine("{0} {1}", course.Name, course.Title);
            Console.WriteLine("CRN:          {0}", course.CRN);
            Console.WriteLine("Enrollment:   {0}/{1}", course.Enrolled, course.MaxEnrollment);
            Console.WriteLine("Instructor:   {0}", course.Instructor);
            Console.WriteLine("Credit Hours: {0}", course.Credit);
            Console.WriteLine("Final Info:   {0}{1} in {2}", course.FinalDay, course.FinalHour, course.FinalRoom);
            Console.WriteLine("Comments:     {0}", course.Comments);
        }

        static void PrintUser(User user)
        {
            Console.WriteLine("Username:    {0}", user.Username);
            Console.WriteLine("Alias:       {0}", user.Alias);
            Console.WriteLine("Mailbox:     {0}", user.Mailbox);
            Console.WriteLine("Major:       {0}", user.Major);
            Console.WriteLine("Class:       {0}", user.Class);
            Console.WriteLine("Year:        {0}", user.Year);
            Console.WriteLine("Name:        {0} {1} {2}", user.FirstName, user.MiddleName, user.LastName);
            Console.WriteLine("Deptartment: {0}", user.Department);
            Console.WriteLine("Phone:       {0}", user.Phone);
            Console.WriteLine("Room:        {0}", user.Room);
            Console.WriteLine();
        }

        static void PrintEnrollment(UserEnrollment[] enrollment)
        {
            foreach (var item in enrollment)
            {
                var course = service.GetCourse(token, item.Term, item.CRN);
                Console.WriteLine("{0} {1}", course.Name, course.Title);
            }
        }

        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    password += info.KeyChar;
                    info = Console.ReadKey(true);
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring
                        (0, password.Length - 1);
                    }
                    info = Console.ReadKey(true);
                }
            }
            //for (int i = 0; i < password.Length; i++)
            //    Console.Write("*");
            Console.WriteLine();
            return password;
        }

    }
}
