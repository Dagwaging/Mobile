using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Threading;
using Rhit.Applications.Model.Services.Requests;
using System.Windows;
using System.Threading;
using System.Collections.Generic;

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

        public static IAsyncResult MakeLocationRequest(RequestPart requestPart, RequestType type, int locationId) {
            ServiceRequest request = new ServiceRequest(requestPart, type);
            request.UserMetaData["LocationId"] = locationId;
            return MakeRequest(request);
        }

        public static IAsyncResult MakeRequests(IList<RequestPart> requests, RequestType type) {
            ServiceRequest request = new ServiceRequest(type);
            foreach(RequestPart req in requests) request.AddRequest(req);
            return MakeRequest(request);
        }

        public static IAsyncResult MakeLocationRequests(IList<RequestPart> requests, RequestType type, int locationId) {
            ServiceRequest request = new ServiceRequest(type);
            foreach(RequestPart req in requests) request.AddRequest(req);
            request.UserMetaData["LocationId"] = locationId;
            return MakeRequest(request);
        }

        public static IAsyncResult MakeLocationChangeRequest(IList<RequestPart> requests, RequestType type, int oldId, int newId) {
            ServiceRequest request = new ServiceRequest(type);
            foreach(RequestPart req in requests) request.AddRequest(req);
            request.UserMetaData["OldLocationId"] = oldId;
            request.UserMetaData["NewLocationId"] = newId;
            return MakeRequest(request);
        }
    }
}