using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

namespace RHITMobile.Secure.Data_Import
{

    public abstract class BannerCsvParser<T> : IEnumerable<T>, IEnumerator<T> where T:class
    {
        private T current;
        private TextFieldParser parser;
        private int termCode;

        public int TermCode { get { return termCode; } }

        public BannerCsvParser(String path)
        {
            parser = new TextFieldParser(path);
            parser.SetDelimiters("|");
            termCode = int.Parse(parser.ReadLine());
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
                trim(fields);

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

        public void trim(String[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = fields[i].Trim();
                if (Regex.IsMatch(fields[i], "^(&nbsp|&nbsp;)+$"))
                    fields[i] = "";
            }
        }

        public int toInt(String field)
        {
            int result;
            if (!int.TryParse(field, out result))
                return -1;

            return result;
        }
    }
}
