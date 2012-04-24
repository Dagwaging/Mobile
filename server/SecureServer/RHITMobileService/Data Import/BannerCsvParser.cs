using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using System.IO;

namespace RHITMobile.Secure.Data_Import
{

    public abstract class BannerCsvParser<T> : IEnumerable<T>, IEnumerator<T> where T:class
    {
        private Logger _log;

        private T current;
        private TextFieldParser parser;
        private int termCode;

        public int TermCode { get { return termCode; } }

        public int ParseErrors { get; private set; }

        public BannerCsvParser(Logger log, String path)
        {
            _log = log;
            parser = new TextFieldParser(path);
            parser.HasFieldsEnclosedInQuotes = false;
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
                try
                {
                    LineNumber = parser.LineNumber;
                    String[] fields = parser.ReadFields();
                    if (fields != null)
                    {
                        trim(fields);

                        try
                        {
                            res = convertRecord(fields);
                        }
                        catch (Exception e)
                        {
                            //skip the record
                            if (ParseErrors < 3)
                                _log.Error(string.Format("Failed to convert record at line {0}", LineNumber), e);
                            ParseErrors++;
                        }
                    }
                }
                catch (Exception e)
                {
                    //skip the record
                    if (ParseErrors < 3)
                        _log.Error(string.Format("Failed to read record at line {0}", LineNumber), e);
                    ParseErrors++;
                }
            } while (res == null && hasMore());

            return res;
        }

        protected abstract T convertRecord(String[] fields);

        public long LineNumber { get; private set; }

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
