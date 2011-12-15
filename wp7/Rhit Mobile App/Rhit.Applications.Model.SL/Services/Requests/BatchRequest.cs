using System;
using System.Collections.Generic;
using Rhit.Applications.Model.Services.Requests;
using Rhit.Applications.Model.Services;
using System.Windows.Threading;

namespace Rhit.Applications.Model.Services.Requests {
    public class BatchRequest {
        private Queue<RequestPart> _queue = new Queue<RequestPart>();
        private Dispatcher _dispatcher;
        
        public bool HasStarted { get; private set; }

        public BatchRequest(Dispatcher dispatcher) {
            HasStarted = false;
            _dispatcher = dispatcher;
        }

        public void AddRequest(RequestPart request) {
            if (!HasStarted)
                _queue.Enqueue(request);
            else
                throw new InvalidOperationException("Cannot add a request to a BatchRequest that has started.");
        }

        public void Start() {
            if (!HasStarted) {
                HasStarted = true;
                SyncCallback(null);
            }
            else
                throw new InvalidOperationException("Cannot start a BatchRequest that has already started.");
        }

        public void SyncCallback(IAsyncResult result) {
            if (_queue.Count > 0) {
                var request = _queue.Dequeue();
                Connection.MakeRequest(_dispatcher, request, false, this.SyncCallback);
            }
        }
    }
}
