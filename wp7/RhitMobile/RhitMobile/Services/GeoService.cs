using System;
using System.Net;
using System.Windows.Threading;
using RhitMobile.Services.Requests;

namespace RhitMobile.Services {
    /// <summary>
    /// Singleton class for obtaining location data.
    /// </summary>
    public class GeoService {
        /// <summary>
        /// Used to send information back to the GUI thread.
        /// </summary>
        public static Dispatcher Dispatcher { get; private set; }

        public static void MakeRequest(Dispatcher dispatcher, RequestPart url) {
            GeoService.MakeRequest(dispatcher, url, false);
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
            request.AllowAutoRedirect = true;

            if (isSearch)
                request.BeginGetResponse(new AsyncCallback(ResponseHandler.SearchRequestCallback), request);
            else if(url is AllRequestPart)
                request.BeginGetResponse(new AsyncCallback(ResponseHandler.AllRequestCallback), request);
            else if(url is TopRequestPart)
                request.BeginGetResponse(new AsyncCallback(ResponseHandler.TopRequestCallback), request);
        }
    }
}