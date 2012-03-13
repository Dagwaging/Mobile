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

            res.ID = fields[i++];
            res.Username = fields[i++].ToLower();
            res.Alias = fields[i++].ToLower();
            res.Mailbox = toInt(fields[i++]);
            res.Major = fields[i++];
            res.Class = fields[i++];
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
