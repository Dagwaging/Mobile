using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RHITMobile.Secure.Data;

namespace RHITMobile.Secure.Data_Import
{
    class CourseCsvParser : BannerCsvParser<Course>
    {
        public CourseCsvParser(String path)
            : base(path)
        { }

        protected override Course convertRecord(String[] fields)
        {
            Course res = new Course();

            int i = 0;

            res.Term = TermCode;
            res.CRN = toInt(fields[i++]);

            res.Course = fields[i++];
            res.Title = fields[i++];
            res.Instructor = fields[i++];
            res.Credit = toInt(fields[i++]);

            String schedule = fields[i++];
            res.Schedule = convertSchedule(schedule);

            String[] finalArgs = fields[i++].Split('/');
            trim(finalArgs);
            res.FinalDay = finalArgs[0][0];
            res.FinalHour = toInt(finalArgs[1]);
            res.FinalRoom = finalArgs[2];

            res.Enrolled = toInt(fields[i++]);
            res.MaxEnrollment = toInt(fields[i++]);
            res.Comments = fields[i++];

            return res;
        }

        private List<CourseTime> convertSchedule(String schedule)
        {
            List<CourseTime> res = new List<CourseTime>();

            String[] scheduleSpots = schedule.Split(':');
            trim(scheduleSpots);

            foreach (String spot in scheduleSpots)
            {
                String[] spotArgs = spot.Split('/');
                trim(spotArgs);

                String days = spotArgs[0];
                int hour = toInt(spotArgs[1]);
                String room = spotArgs[2];

                foreach (char c in days)
                {
                    CourseTime time = new CourseTime();
                    time.Day = c;
                    time.Period = hour;
                    time.Room = room;
                    res.Add(time);
                }
            }

            return res;
        }
    }
}
