using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace RHITMobile.Secure.Data_Import
{

    public abstract class BannerCsvParser<T> : IEnumerable<T>, IEnumerator<T> where T : BannerCsvRecord
    {
        private T current;
        private TextFieldParser parser;
        private String termCode;

        public String TermCode { get { return termCode; } }

        public BannerCsvParser(String path)
        {
            parser = new TextFieldParser(path);
            parser.SetDelimiters("|");
            termCode = parser.ReadLine();
        }

        private bool hasMore()
        {
            return !parser.EndOfData;
        }

        private T getRecord()
        {
            T res = null;
            do
            {
                String[] fields = parser.ReadFields();
                for (int i = 0; i < fields.Length; i++)
                    fields[i] = fields[i].Trim();

                try
                {
                    res = convertRecord(fields);
                }
                catch (Exception)
                {
                    //skip the record
                }
            } while (res == null && hasMore());

            return res;
        }

        protected abstract T convertRecord(String[] fields);

        public T Current
        {
            get { return current; }
        }

        public void Dispose()
        {
            current = null;
            if (parser != null)
            {
                parser.Close();
                parser = null;
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            current = getRecord();
            return current != null;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }
    }

    public class BannerCsvRecord
    {
    }

    public class User : BannerCsvRecord
    {
        public String ID { get; set; }
        public String Username { get; set; }
        public String Alias { get; set; }
        public String Mailbox { get; set; }
        public String Major { get; set; }
        public String Class { get; set; }
        //Year
        public String LastName { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String Department { get; set; }
        public String Phone { get; set; }
        public String Room { get; set; }
    }

    public class UserCsvParser : BannerCsvParser<User>, IEnumerator<User>
    {
        public UserCsvParser(String path)
            : base(path)
        {
        }

        protected override User convertRecord(String[] fields)
        {
            User res = new User();

            int i = 0;

            res.ID = fields[i++];
            res.Username = fields[i++].ToLower();
            res.Alias = fields[i++].ToLower();
            res.Mailbox = fields[i++];
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
