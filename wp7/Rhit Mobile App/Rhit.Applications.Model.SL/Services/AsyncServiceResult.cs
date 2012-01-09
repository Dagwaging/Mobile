using System;
using System.Threading;
using System.Net;

namespace Rhit.Applications.Model.Services {
    public class AsyncServiceResult {
        public AsyncServiceResult(IAsyncResult result, ServiceRequest request) {
            Request = request;
            BaseResult = result;
        }

        public IAsyncResult BaseResult { get; private set; }

        public ServiceRequest Request { get; private set; }

        public HttpWebRequest GetWebRequest() {
            return (HttpWebRequest) BaseResult.AsyncState;
        }
    }
}
