using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RHITMobile.Secure.Data;

namespace RHITMobile.Secure.Data_Import
{
    class EnrollmentCsvParser: BannerCsvParser<Enrollment>
    {
        private Dictionary<String, String> idToUsername;

        public EnrollmentCsvParser(String path, Dictionary<String, String> idToUsername)
            : base(path)
        {
            this.idToUsername = idToUsername;
        }
        
        protected override Enrollment convertRecord(String[] fields)
        {
            Enrollment res = new Enrollment();

            String id = fields[0];
            res.Username = idToUsername[id];
            res.Term = TermCode;

            res.CRNs = convertCRNs(fields.Skip(1).ToArray());

            return res;
        }

        private List<int> convertCRNs(String[] courses)
        {
            List<int> res = new List<int>();
            
            foreach (String course in courses)
            {
                String[] courseArgs = course.Split('.');
                trim(courseArgs);
                //0 - CRN
                //1 - Course name (CSSE404-01)
                //2 - Credits?
                int crn = toInt(courseArgs[0]);
                if (crn >= 0)
                    res.Add(crn);
            }

            return res;
        }
    }
}
