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
        void ForceUpdate();

        [OperationContract]
        User GetUser(string username);
    }

    public class WebService : IWebService
    {
        public void ForceUpdate()
        {
            //Data_Import.Importer importer = new Data_Import.Importer(new NullLogger(), "C:\\InputData");
            //importer.ImportData();
        }

        public User GetUser(string username)
        {
            return DB.Instance.GetUser(username);
        }
    }
}
