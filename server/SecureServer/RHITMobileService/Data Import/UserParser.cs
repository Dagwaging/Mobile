using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RHITMobile.Secure.Data;

namespace RHITMobile.Secure.Data_Import
{
    public class UserCsvParser : BannerCsvParser<User>
    {
        public UserCsvParser(String path)
            : base(path)
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
            res.Alias = fields[i++].ToLower();
            {
                String mailbox = fields[i++];
                mailbox = mailbox.Replace("CM", "").Trim();
                res.Mailbox = toInt(mailbox);
            }
            res.Major = fields[i++];
            res.Class = fields[i++];
            res.Year = fields[i++];
            res.Advisor = fields[i++].ToLower();
            res.LastName = fields[i++];
            res.FirstName = fields[i++];
            res.MiddleName = fields[i++];
            res.Department = fields[i++];
            res.Phone = fields[i++];
            res.Room = fields[i++];

            return res;
        }
    }
}
