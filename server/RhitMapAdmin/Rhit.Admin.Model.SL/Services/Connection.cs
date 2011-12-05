using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Threading;
using Rhit.Admin.Model.Services.Requests;
using System.Windows;

namespace Rhit.Admin.Model.Services {
    public static class Connection {
        public static bool TestConnection() {
            if(!NetworkInterface.GetIsNetworkAvailable())
                return false;
            return true;
        }

        /// <summary>
        /// Used to send information back to the GUI thread.
        /// </summary>
        public static Dispatcher Dispatcher { get; private set; }

        public static void MakeRequest(Dispatcher dispatcher, RequestPart url) {
            Connection.MakeRequest(dispatcher, url, false);
        }

        public static void MakeRequest(Dispatcher dispatcher, RequestPart url, bool isSearch) {
            Dispatcher = dispatcher;
            HttpWebRequest request;
            try {
                request = (HttpWebRequest) WebRequest.Create(url.ToString());
            } catch {
                //TODO: Actually do something here
                //Raise error or notify what happened
                return;
            }
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            //request.AllowAutoRedirect = true;

            if(isSearch)
                request.BeginGetResponse(new AsyncCallback(ResponseHandler.SearchRequestCallback), request);
            else if(url is AllRequestPart)
                request.BeginGetResponse(new AsyncCallback(ResponseHandler.AllRequestCallback), request);
            else if(url is TopRequestPart)
                request.BeginGetResponse(new AsyncCallback(ResponseHandler.TopRequestCallback), request);
        }
    }
}