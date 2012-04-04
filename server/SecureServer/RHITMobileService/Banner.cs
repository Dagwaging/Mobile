using RHITMobile.Secure.Data;
using System.Collections.Generic;
namespace RHITMobile.Secure
{
    public partial class Banner
    {
        partial class GetRoomScheduleDataTable
        {
            public RoomSchedule[] Schedule
            {
                get
                {
                    List<RoomSchedule> schedule = new List<RoomSchedule>();

                    foreach (var row in this)
                    {
                        RoomSchedule item = new RoomSchedule();

                        item.Term = row.Term;
                        item.CRN = row.CRN;
                        item.Day = row.Day[0];
                        item.StartPeriod = row.StartPeriod;
                        item.EndPeriod = row.EndPeriod;

                        schedule.Add(item);
                    }
                    return schedule.ToArray();
                }
            }
        }
    
        partial class GetCourseScheduleDataTable
        {
            public CourseTime[] Schedule
            {
                get
                {
                    List<CourseTime> schedule = new List<CourseTime>();

                    foreach (var row in this)
                    {
                        CourseTime item = new CourseTime();

                        item.Day = row.Day[0];
                        item.StartPeriod = row.StartPeriod;
                        item.EndPeriod = row.EndPeriod;
                        item.Room = row.Room;

                        schedule.Add(item);
                    }
                    return schedule.ToArray();
                }
            }
        }
    
        partial class GetCourseEnrollmentDataTable
        {
            public string[] Enrollment
            {
                get
                {
                    List<string> enrollment = new List<string>();

                    foreach (var row in this)
                        enrollment.Add(row.Username);

                    return enrollment.ToArray();
                }
            }
        }
    
        partial class GetUserEnrollmentDataTable
        {
            public UserEnrollment[] Enrollment
            {
                get
                {
                    List<UserEnrollment> enrollment = new List<UserEnrollment>();

                    foreach (var row in this)
                    {
                        UserEnrollment item = new UserEnrollment();

                        item.Term = row.Term;
                        item.CRN = row.CRN;

                        enrollment.Add(item);
                    }
                    return enrollment.ToArray();
                }
            }
        }
    
        partial class GetCourseDataTable
        {
            public Course Course
            {
                get
                {
                    if (Count < 1)
                        return null;

                    var row = this[0];

                    Course course = new Course();

                    course.Term = row.Term;
                    course.CRN = row.CRN;

                    course.Name = row.Course;
                    course.Title = row.Title;

                    if (!row.IsInstructorNull())
                        course.Instructor = row.Instructor;

                    course.Credit = row.Credits;

                    if (!row.IsFinalDayNull())
                        course.FinalDay = row.FinalDay[0];
                    else
                        course.FinalDay = '\0';

                    if (!row.IsFinalHourNull())
                        course.FinalHour = row.FinalHour;
                    else
                        course.FinalHour = -1;

                    if (!row.IsFinalRoomNull())
                        course.FinalRoom = row.FinalRoom;

                    course.Enrolled = row.Enrolled;
                    course.MaxEnrollment = row.MaxEnrolled;

                    if (!row.IsCommentsNull())
                        course.Comments = row.Comments;

                    return course;
                }
            }
        }

        partial class SearchCoursesDataTable
        {
            public Course[] Courses
            {
                get
                {
                    List<Course> courses = new List<Course>();

                    foreach (var row in this)
                    {
                        Course course = new Course();

                        course.Term = row.Term;
                        course.CRN = row.CRN;

                        course.Name = row.Course;
                        course.Title = row.Title;

                        if (!row.IsInstructorNull())
                            course.Instructor = row.Instructor;

                        course.Credit = row.Credits;

                        if (!row.IsFinalDayNull())
                            course.FinalDay = row.FinalDay[0];
                        else
                            course.FinalDay = '\0';

                        if (!row.IsFinalHourNull())
                            course.FinalHour = row.FinalHour;
                        else
                            course.FinalHour = -1;

                        if (!row.IsFinalRoomNull())
                            course.FinalRoom = row.FinalRoom;

                        course.Enrolled = row.Enrolled;
                        course.MaxEnrollment = row.MaxEnrolled;

                        if (!row.IsCommentsNull())
                            course.Comments = row.Comments;

                        courses.Add(course);
                    }

                    return courses.ToArray();
                }
            }
        }
    
        partial class SearchUsersDataTable
        {
            public User[] Users
            {
                get
                {
                    List<User> users = new List<User>();

                    foreach (var row in this)
                    {
                        User user = new User();

                        user.Username = row.Username;

                        if (!row.IsEmailNull())
                            user.Alias = row.Email;

                        if (!row.IsCMNull())
                            user.Mailbox = row.CM;
                        else
                            user.Mailbox = -1;

                        if (!row.IsMajorNull())
                            user.Major = row.Major;

                        if (!row.IsClassNull())
                            user.Class = row.Class;

                        if (!row.IsYearNull())
                            user.Year = row.Year;

                        if (!row.IsAdvisorNull())
                            user.Advisor = row.Advisor;

                        user.LastName = row.LastName;
                        user.FirstName = row.FirstName;

                        if (!row.IsMiddleNameNull())
                            user.MiddleName = row.MiddleName;

                        if (!row.IsDepartmentNull())
                            user.Department = row.Department;

                        if (!row.IsTelephoneNull())
                            user.Phone = row.Telephone;

                        if (!row.IsRoomNull())
                            user.Room = row.Room;
                        users.Add(user);
                    }

                    return users.ToArray();
                }
            }
        }
    
        partial class GetUserDataTable
        {
            public User User
            {
                get
                {
                    if (Count < 1)
                        return null;

                    Banner.GetUserRow row = this[0];
                    User res = new User();

                    res.Username = row.Username;

                    if (!row.IsEmailNull())
                        res.Alias = row.Email;

                    if (!row.IsCMNull())
                        res.Mailbox = row.CM;
                    else
                        res.Mailbox = -1;

                    if (!row.IsMajorNull())
                        res.Major = row.Major;

                    if (!row.IsClassNull())
                        res.Class = row.Class;

                    if (!row.IsYearNull())
                        res.Year = row.Year;

                    if (!row.IsAdvisorNull())
                        res.Advisor = row.Advisor;

                    res.LastName = row.LastName;
                    res.FirstName = row.FirstName;

                    if (!row.IsMiddleNameNull())
                        res.MiddleName = row.MiddleName;

                    if (!row.IsDepartmentNull())
                        res.Department = row.Department;

                    if (!row.IsTelephoneNull())
                        res.Phone = row.Telephone;

                    if (!row.IsRoomNull())
                        res.Room = row.Room;

                    return res;

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
            return spInsertUser(s, user.Username, user.Alias, user.Mailbox, user.Major, user.Class, user.Year, user.LastName, user.FirstName, user.MiddleName, user.Department, user.Phone, user.Room);
        }
        
        public int AddCourse(bool s, Course course)
        {
            return spInsertCourse(s, course.Term, course.CRN, course.Name, course.Title, course.Instructor, course.Credit, course.FinalDay.ToString(), course.FinalHour, course.FinalRoom, course.Enrolled, course.MaxEnrollment, course.Comments);
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
