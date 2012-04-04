﻿using RHITMobile.Secure.Data;
using System.Collections.Generic;
namespace RHITMobile.Secure
{
    public partial class Banner
    {
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