using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace RHITMobile.Secure
{
    [ServiceContract]
    public interface IWebService
    {
        [OperationContract]
        void forceUpdate();

        [OperationContract]
        int getUserCount();
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
    }
}
