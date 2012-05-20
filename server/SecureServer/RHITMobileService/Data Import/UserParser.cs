using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RHITMobile.Secure.Data;

namespace RHITMobile.Secure.Data_Import
{
    public class UserCsvParser : BannerCsvParser<User>
    {
        public UserCsvParser(Logger log, String path)
            : base(log, path)
        { }

        protected override User convertRecord(String[] fields)
        {
            User res = new User();

            int i = 0;

            //Skip potentially blank columns that may or may not exist
            int gid;
            if (!int.TryParse(fields[0], out gid))
            {
                i = 1;
            }

            res.ID = fields[i++];
            res.Username = fields[i++].ToLower();
            res.Alias = optionalField(fields[i++].ToLower());
            {
                String mailbox = fields[i++];
                mailbox = mailbox.Replace("CM", "").Trim();
                res.Mailbox = toInt(mailbox);
            }
            res.Major = optionalField(fields[i++]);
            res.Class = optionalField(fields[i++]);
            res.Year = optionalField(fields[i++]);
            res.Advisor = optionalField(fields[i++].ToLower());
            res.LastName = fields[i++];
            res.FirstName = fields[i++];
            res.MiddleName = optionalField(fields[i++]);
            res.Department = optionalField(fields[i++]);
            res.Phone = optionalField(fields[i++]);
            res.Room = optionalField(fields[i++]);

            return res;
        }
    }
}
