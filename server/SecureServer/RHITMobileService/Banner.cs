using RHITMobile.Secure.Data;
using System.Collections.Generic;
namespace RHITMobile.Secure
{
    public partial class Banner
    {
        partial class spGetRoomScheduleDataTable
        {
            public RoomSchedule[] Schedule
            {
                get
                {
                    List<RoomSchedule> schedule = new List<RoomSchedule>();

                    foreach (var row in this)
                    {
                        RoomSchedule item = new RoomSchedule();

                        item.Term = row.term;
                        item.CRN = row.crn;
                        item.Day = row.day[0];
                        item.StartPeriod = row.startperiod;
                        item.EndPeriod = row.endperiod;

                        schedule.Add(item);
                    }
                    return schedule.ToArray();
                }
            }
        }
    
        partial class spGetCourseScheduleDataTable
        {
            public CourseTime[] Schedule
            {
                get
                {
                    List<CourseTime> schedule = new List<CourseTime>();

                    foreach (var row in this)
                    {
                        CourseTime item = new CourseTime();

                        item.Day = row.day[0];
                        item.StartPeriod = row.startperiod;
                        item.EndPeriod = row.endperiod;
                        item.Room = row.room;

                        schedule.Add(item);
                    }
                    return schedule.ToArray();
                }
            }
        }
    
        partial class spGetCourseEnrollmentDataTable
        {
            public string[] Enrollment
            {
                get
                {
                    List<string> enrollment = new List<string>();

                    foreach (var row in this)
                        enrollment.Add(row.username);

                    return enrollment.ToArray();
                }
            }
        }
    
        partial class spGetUserEnrollmentDataTable
        {
            public UserEnrollment[] Enrollment
            {
                get
                {
                    List<UserEnrollment> enrollment = new List<UserEnrollment>();

                    foreach (var row in this)
                    {
                        UserEnrollment item = new UserEnrollment();

                        item.Term = row.term;
                        item.CRN = row.crn;

                        enrollment.Add(item);
                    }
                    return enrollment.ToArray();
                }
            }
        }

        partial class spGetInstructorScheduleDataTable
        {
            public UserEnrollment[] Schedule
            {
                get
                {
                    List<UserEnrollment> enrollment = new List<UserEnrollment>();

                    foreach (var row in this)
                    {
                        UserEnrollment item = new UserEnrollment();

                        item.Term = row.term;
                        item.CRN = row.crn;

                        enrollment.Add(item);
                    }
                    return enrollment.ToArray();
                }
            }
        }
    
        partial class spGetCourseDataTable
        {
            public Course Course
            {
                get
                {
                    if (Count < 1)
                        return null;

                    var row = this[0];

                    Course course = new Course();

                    course.Term = row.term;
                    course.CRN = row.crn;

                    course.Name = row.course;
                    course.Title = row.title;

                    if (!row.IsinstructorNull())
                        course.Instructor = row.instructor;

                    course.Credit = row.credits;

                    if (!row.IsfinaldayNull())
                        course.FinalDay = row.finalday[0];
                    else
                        course.FinalDay = '\0';

                    if (!row.IsfinalhourNull())
                        course.FinalHour = row.finalhour;
                    else
                        course.FinalHour = -1;

                    if (!row.IsfinalroomNull())
                        course.FinalRoom = row.finalroom;

                    course.Enrolled = row.enrolled;
                    course.MaxEnrollment = row.maxenrolled;

                    if (!row.IscommentsNull())
                        course.Comments = row.comments;

                    return course;
                }
            }
        }

        partial class spSearchCoursesDataTable
        {
            public Course[] Courses
            {
                get
                {
                    List<Course> courses = new List<Course>();

                    foreach (var row in this)
                    {
                        Course course = new Course();

                        course.Term = row.term;
                        course.CRN = row.crn;

                        course.Name = row.course;
                        course.Title = row.title;

                        if (!row.IsinstructorNull())
                            course.Instructor = row.instructor;

                        course.Credit = row.credits;

                        if (!row.IsfinaldayNull())
                            course.FinalDay = row.finalday[0];
                        else
                            course.FinalDay = '\0';

                        if (!row.IsfinalhourNull())
                            course.FinalHour = row.finalhour;
                        else
                            course.FinalHour = -1;

                        if (!row.IsfinalroomNull())
                            course.FinalRoom = row.finalroom;

                        course.Enrolled = row.enrolled;
                        course.MaxEnrollment = row.maxenrolled;

                        if (!row.IscommentsNull())
                            course.Comments = row.comments;

                        courses.Add(course);
                    }

                    return courses.ToArray();
                }
            }
        }
    
        partial class spSearchUsersDataTable
        {
            public User[] Users
            {
                get
                {
                    List<User> users = new List<User>();

                    foreach (var row in this)
                    {
                        User user = new User();

                        user.Username = row.username;

                        if (!row.IsemailNull())
                            user.Alias = row.email;

                        if (!row.IscmNull())
                            user.Mailbox = row.cm;
                        else
                            user.Mailbox = -1;

                        if (!row.IsmajorsNull())
                            user.Major = row.majors;

                        if (!row.Is_classNull())
                            user.Class = row._class;

                        if (!row.IsyearNull())
                            user.Year = row.year;

                        if (!row.IsadvisorNull())
                            user.Advisor = row.advisor;

                        user.LastName = row.lastname;
                        user.FirstName = row.firstname;

                        if (!row.IsmiddlenameNull())
                            user.MiddleName = row.middlename;

                        if (!row.IsdepartmentNull())
                            user.Department = row.department;

                        if (!row.IstelephoneNull())
                            user.Phone = row.telephone;

                        if (!row.IsofficeNull())
                            user.Room = row.office;
                        users.Add(user);
                    }

                    return users.ToArray();
                }
            }
        }
    
        partial class spGetUserDataTable
        {
            public User User
            {
                get
                {
                    if (Count < 1)
                        return null;

                    var row = this[0];
                    User user = new User();

                    user.Username = row.username;

                    if (!row.IsemailNull())
                        user.Alias = row.email;

                    if (!row.IscmNull())
                        user.Mailbox = row.cm;
                    else
                        user.Mailbox = -1;

                    if (!row.IsmajorsNull())
                        user.Major = row.majors;

                    if (!row.Is_classNull())
                        user.Class = row._class;

                    if (!row.IsyearNull())
                        user.Year = row.year;

                    if (!row.IsadvisorNull())
                        user.Advisor = row.advisor;

                    user.LastName = row.lastname;
                    user.FirstName = row.firstname;

                    if (!row.IsmiddlenameNull())
                        user.MiddleName = row.middlename;

                    if (!row.IsdepartmentNull())
                        user.Department = row.department;

                    if (!row.IstelephoneNull())
                        user.Phone = row.telephone;

                    if (!row.IsofficeNull())
                        user.Room = row.office;

                    return user;

                }
            }
        }

        partial class spGetTermsDataTable
        {
            public int[] Terms
            {
                get
                {
                    List<int> terms = new List<int>();

                    foreach (var row in this)
                    {
                        terms.Add(row.term);
                    }
                    return terms.ToArray();
                }
            }
        }
    }
}

namespace RHITMobile.Secure.BannerTableAdapters
{
    public partial class QueriesTableAdapter
    {
        public int AddUser(bool s, User user)
        {
            int? mailbox = user.Mailbox;
            if (mailbox < 1)
                mailbox = null;

            return spInsertUser(s, user.Username, user.Alias, mailbox, user.Major, user.Class, user.Year, user.LastName, user.FirstName, user.MiddleName, user.Department, user.Phone, user.Room);
        }
        
        public int AddCourse(bool s, Course course)
        {
            string finalDay = course.FinalDay.ToString();
            int? finalHour = course.FinalHour;
            string finalRoom = course.FinalRoom;

            if (course.FinalDay == char.MinValue || finalHour == 0)
            {
                finalDay = null;
                finalHour = null;
                finalRoom = null;
            }

            return spInsertCourse(s, course.Term, course.CRN, course.Name, course.Title, course.Instructor, course.Credit, finalDay, finalHour, finalRoom, course.Enrolled, course.MaxEnrollment, course.Comments);
        }

        public int AddCourseSchedule(bool s, Course course)
        {
            int res = 0;
            foreach (CourseTime time in course.Schedule)
            {
                res += spInsertCourseSchedule(s, course.Term, course.CRN, time.Day.ToString(), time.StartPeriod, time.EndPeriod, time.Room);
            }
            return res;
        }

        public int AddUserEnrollment(bool s, Enrollment enrollment)
        {
            int res = 0;
            foreach (int crn in enrollment.CRNs)
            {
                res += spInsertEnrollment(s, enrollment.Username, enrollment.Term, crn);
            }
            return res;
        }
    }
}
