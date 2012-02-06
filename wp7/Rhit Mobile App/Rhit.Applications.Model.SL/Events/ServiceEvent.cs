using System;
using System.Net;
using Rhit.Applications.Model.Services;

namespace Rhit.Applications.Model.Events {
    public delegate void ServiceEventHandler(Object sender, ServiceEventArgs e);

    public class ServiceEventArgs : EventArgs {
        public ServerObject ResponseObject { get; set; }

        public Exception Error { get; set; }

        public ServiceRequest Request { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public ResponseType Type { get; set; }

        public string RawResponse { get; set; }

        public void Copy(ServiceEventArgs args) {
            ResponseObject = args.ResponseObject;
            Error = args.Error;
            Request = args.Request;
            StatusCode = args.StatusCode;
            Type = args.Type;
            RawResponse = args.RawResponse;
        }
    }
}
