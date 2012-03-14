using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RHITMobile.Secure.Data
{
    public class User
    {
        public String ID { get; set; }
        public String Username { get; set; }
        public String Alias { get; set; }
        public int Mailbox { get; set; }
        public String Major { get; set; }
        public String Class { get; set; }
        public String Year { get; set; }
        public String Advisor { get; set; }
        public String LastName { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String Department { get; set; }
        public String Phone { get; set; }
        public String Room { get; set; }
    }
    
    public class Course
    {
        public int Term { get; set; }
        public int CRN { get; set; }

        public String Name { get; set; }
        public String Title { get; set; }
        public String Instructor { get; set; }
        public int Credit { get; set; }
        public char FinalDay { get; set; }
        public int FinalHour { get; set; }
        public String FinalRoom { get; set; }
        public int Enrolled { get; set; }
        public int MaxEnrollment { get; set; }
        public String Comments { get; set; }

        public List<CourseTime> Schedule { get; set; }
    }

    public class CourseTime
    {
        public char Day { get; set; }
        public int Period { get; set; }
        public String Room { get; set; }
    }

    public class Enrollment
    {
        public String Username { get; set; }
        public int Term { get; set; }
        public List<int> CRNs { get; set; }

    }
}
