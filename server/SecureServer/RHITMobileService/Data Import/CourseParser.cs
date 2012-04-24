using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RHITMobile.Secure.Data;

namespace RHITMobile.Secure.Data_Import
{
    class CourseCsvParser : BannerCsvParser<Course>
    {
        public CourseCsvParser(Logger log, String path)
            : base(log, path)
        { }

        protected override Course convertRecord(String[] fields)
        {
            Course res = new Course();

            int i = 0;

            res.Term = TermCode;
            res.CRN = toInt(fields[i++]);

            res.Name = fields[i++];
            res.Title = fields[i++];
            res.Instructor = fields[i++];
            res.Credit = toInt(fields[i++]);

            try
            {
                String schedule = fields[i++];
                res.Schedule = convertSchedule(schedule);
            }
            catch (Exception ex)
            {
                if (ParseErrors < 3)
                    Log.Error(string.Format("Failed to parse course schedule (line {0})", LineNumber), ex);
                ParseErrors++;

                res.Schedule = new List<CourseTime>();
            }

            try
            {
                String finalSchedule = fields[i++];
                convertFinalSchedule(finalSchedule, res);
            }
            catch (Exception ex)
            {
                if (ParseErrors < 3)
                    Log.Error(string.Format("Failed to parse final schedule (line {0})", LineNumber), ex);
                ParseErrors++;
            }

            res.Enrolled = toInt(fields[i++]);
            res.MaxEnrollment = toInt(fields[i++]);
            res.Comments = fields[i++];

            return res;
        }

        private void convertFinalSchedule(String finalSchedule, Course course)
        {
            if (string.IsNullOrEmpty(finalSchedule))
                return;

            String[] finalArgs = finalSchedule.Split('/');
            trim(finalArgs);
            if (finalArgs.Length != 3)
                return;
            if (finalArgs.Any(arg => arg == "TBA"))
                return;

            course.FinalDay = finalArgs[0][0];
            course.FinalHour = toInt(finalArgs[1]);
            course.FinalRoom = finalArgs[2];
        }

        private List<CourseTime> convertSchedule(String schedule)
        {
            List<CourseTime> res = new List<CourseTime>();

            if (string.IsNullOrEmpty(schedule))
                return res;

            String[] scheduleSpots = schedule.Split(':');
            trim(scheduleSpots);

            foreach (String spot in scheduleSpots)
            {
                if (string.IsNullOrEmpty(spot))
                    continue;
                if (spot.ToUpper() == "TBA")
                    continue;

                try
                {
                    String[] spotArgs = spot.Split('/');
                    trim(spotArgs);

                    if (spotArgs.Length != 3)
                        continue;
                    if (spotArgs.Any(arg => arg == "TBA"))
                        continue;

                    String days = spotArgs[0];
                    int startperiod;
                    int endperiod;
                    if (spotArgs[1].Contains('-'))
                    {
                        String[] hours = spotArgs[1].Split('-');
                        startperiod = toInt(hours[0]);
                        endperiod = toInt(hours[1]);
                    }
                    else
                    {
                        startperiod = toInt(spotArgs[1]);
                        endperiod = startperiod;
                    }
                    String room = spotArgs[2];

                    foreach (char c in days)
                    {
                        CourseTime time = new CourseTime();
                        time.Day = c;
                        time.StartPeriod = startperiod;
                        time.EndPeriod = endperiod;
                        time.Room = room;
                        res.Add(time);
                    }
                }
                catch (Exception ex)
                {
                    if (ParseErrors < 3)
                        Log.Error(string.Format("Failed to parse course schedule item (line {0})", LineNumber), ex);
                    ParseErrors++;
                }
            }

            return res;
        }
    }
}
