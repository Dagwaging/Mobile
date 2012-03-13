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
        User getUser(string username);
    }

    public class WebService : IWebService
    {
        public void forceUpdate()
        {
        }

        public int getUserCount()
        {
            return 45;
        }

        public User getUser(string usename)
        {
            return null;
        }
    }
}
