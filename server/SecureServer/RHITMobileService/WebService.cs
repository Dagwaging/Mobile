using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using RHITMobile.Secure.Data;

namespace RHITMobile.Secure
{
    [ServiceContract]
    public interface IWebService
    {
        [OperationContract]
        void forceUpdate();

        [OperationContract]
        int getUserCount();

        [OperationContract]
        String[] getUserNames();

        [OperationContract]
        User getUser(string username);
    }

    public class WebService : IWebService
    {
        public void forceUpdate()
        {
            Data_Import.Importer importer = new Data_Import.Importer(new NullLogger(), "C:\\InputData");
            importer.ImportData();
        }

        public int getUserCount()
        {
            return Data_Import.Importer.users.Count;
        }

        public String[] getUserNames()
        {
            List<String> res = new List<String>();
            foreach (User user in Data_Import.Importer.users.Take(20))
            {
                res.Add(user.Username);
            }
            res.Sort();
            return res.ToArray();
        }

        public User getUser(string username)
        {
            foreach (User user in Data_Import.Importer.users)
            {
                if (user.Username == username.ToLower())
                    return user;
            }
            return null;
        }
    }
}
