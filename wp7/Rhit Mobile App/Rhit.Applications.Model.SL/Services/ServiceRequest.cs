using System;
using System.Net;
using Rhit.Applications.Model.Services.Requests;
using System.Collections.Generic;

namespace Rhit.Applications.Model.Services {
    public class ServiceRequest {
        public ServiceRequest() {
            Sent = false;
            Requests = new Queue<HttpWebRequest>();
            UserMetaData = new Dictionary<string, object>();
        }

        public ServiceRequest(RequestPart request, RequestType type) : this() {
            AddRequest(request);
            Type = type;
        }

        public ServiceRequest(RequestType type) : this() {
            Type = type;
        }

        public bool Sent { get; private set; }

        public void AddRequest(RequestPart request) {
            if(Sent) throw new InvalidOperationException("Cannot add a request to a ServiceRequest that has started.");
            HttpWebRequest _request;
            try {
                _request = (HttpWebRequest) WebRequest.Create(request.ToString());
            } catch(Exception e) { throw e; }
            _request.Method = "POST";
            _request.ContentType = "application/json; charset=utf-8";
            Requests.Enqueue(_request);
        }

        public void AddRequest(RequestPart request, RequestType type) {
            Type = type;
            AddRequest(request);
        }

        private Queue<HttpWebRequest> Requests { get; set; }

        public RequestType Type { get; private set; }

        private Action<AsyncServiceResult> Callback { get; set; }

        public IAsyncResult Send(Action<AsyncServiceResult> callback) {
            if(Requests.Count <= 0) throw new InvalidOperationException("No requests were added to the ServiceRequest.");
            if(Sent) throw new InvalidOperationException("Cannot send a ServiceRequest that has already started.");
            if(callback == null) throw new ArgumentNullException("Callback cannot be null.");
            Callback = callback;
            HttpWebRequest request = Requests.Dequeue();
            Sent = true;
            return request.BeginGetResponse(new AsyncCallback(InternalCallback), request);
        }

        private void InternalCallback(IAsyncResult result) {
            if(Requests.Count > 0) {
                HttpWebRequest request = Requests.Dequeue();
                request.BeginGetResponse(new AsyncCallback(InternalCallback), request);
            }
            else Callback(new AsyncServiceResult(result, this));
        }

        public Dictionary<string, object> UserMetaData { get; private set; }
    }
}
