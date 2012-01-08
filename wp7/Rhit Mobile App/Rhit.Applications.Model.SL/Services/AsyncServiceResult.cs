using System;
using System.Threading;

namespace Rhit.Applications.Model.Services {
    public class AsyncServiceResult : IAsyncResult {
        public AsyncServiceResult(IAsyncResult result, ServiceRequest request) {
            AsyncState = result.AsyncState;
            AsyncWaitHandle = result.AsyncWaitHandle;
            CompletedSynchronously = result.CompletedSynchronously;
            IsCompleted = result.IsCompleted;
            Request = request;
        }

        public ServiceRequest Request { get; set; }

        public object AsyncState {
            get;
            private set;
        }

        public WaitHandle AsyncWaitHandle {
            get;
            private set;
        }

        public bool CompletedSynchronously {
            get;
            private set;
        }

        public bool IsCompleted {
            get;
            private set;
        }
    }
}
