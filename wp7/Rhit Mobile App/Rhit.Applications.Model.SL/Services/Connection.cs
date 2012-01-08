using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Threading;
using Rhit.Applications.Model.Services.Requests;
using System.Windows;
using System.Threading;

namespace Rhit.Applications.Model.Services {
    public static class Connection {
        public static bool TestConnection() {
            if(!NetworkInterface.GetIsNetworkAvailable())
                return false;
            return true;
        }

        public static string ServiceToken { get; set; }

        public static Guid ServiceTokenGuid {
            get { return new Guid(ServiceToken); }
        }

        public static DateTime Expiration { get; set; }

        /// <summary>
        /// Used to send information back to the GUI thread.
        /// </summary>
        public static Dispatcher Dispatcher { get; private set; }

        public static void SetDispatcher(Dispatcher dispatcher) {
            Dispatcher = dispatcher;
        }

        public static IAsyncResult MakeRequest(RequestPart requestPart, RequestType type) {
            ServiceRequest request = new ServiceRequest(requestPart, type);
            return MakeRequest(request);
        }

        private static IAsyncResult MakeRequest(ServiceRequest request) {
            if(Dispatcher == null) throw new ArgumentNullException("The static property 'Dispatcher' must not be null in order to receive a response");
            return request.Send(ResponseHandler.RequestCallback);
        }

        public static IAsyncResult MakeLocationChangeRequest(RequestPart requestPart, RequestType type, int locationId) {
            ServiceRequest request = new ServiceRequest(requestPart, type);
            request.UserMetaData["LocationId"] = locationId;
            return MakeRequest(request);
        }
    }
}