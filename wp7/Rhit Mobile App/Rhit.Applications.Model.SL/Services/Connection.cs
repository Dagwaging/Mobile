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

        public static void MakeRequest(Dispatcher dispatcher, RequestPart url) {
            Connection.MakeRequest(dispatcher, url, false, false);
        }

        public static IAsyncResult MakeRequest(Dispatcher dispatcher, RequestPart url, bool isSearch, bool isSync) {
            if(dispatcher != null) Dispatcher = dispatcher;
            HttpWebRequest request;
            try {
                request = (HttpWebRequest) WebRequest.Create(url.ToString());
            } catch {
                //TODO: Actually do something here
                //Raise error or notify what happened
                return null;
            }
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            if(isSync)
                return request.BeginGetResponse(new AsyncCallback(SyncCallback), request);
            else if(isSearch)
                return request.BeginGetResponse(new AsyncCallback(ResponseHandler.SearchRequestCallback), request);
            else if(url is AllRequestPart)
                return request.BeginGetResponse(new AsyncCallback(ResponseHandler.AllRequestCallback), request);
            else if(url is TopRequestPart)
                return request.BeginGetResponse(new AsyncCallback(ResponseHandler.TopRequestCallback), request);
            else return request.BeginGetResponse(new AsyncCallback(ResponseHandler.RequestCallback), request);
        }

        private static bool _syncWait = false;
        public static void SyncCallback(IAsyncResult result) {
            _syncWait = false;
        }

        public static void MakeSyncRequest(Dispatcher dispatcher, RequestPart url, bool isSearch) {
            _syncWait = true;
            var asyncResult = MakeRequest(dispatcher, url, isSearch, true);
            while (_syncWait) Thread.Sleep(100);
        }
    }
}