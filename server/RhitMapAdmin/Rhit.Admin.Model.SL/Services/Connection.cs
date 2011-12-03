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
            Dispatcher = dispatcher;
            HttpWebRequest request;
            try {
                request = (HttpWebRequest) WebRequest.Create(url.ToString());
            } catch(Exception ex) {
                //TODO: Actually do something here
                //Raise error or notify what happened
                throw ex;
            }
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            //request.AllowAutoRedirect = true;

            request.BeginGetResponse(new AsyncCallback(ResponseHandler.RequestCallback), request);
        }
    }
}