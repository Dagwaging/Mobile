using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Threading;
using RhitMobile.Events;

namespace RhitMobile.Services {
    /// <summary>
    /// Singleton class for obtaining location data.
    /// </summary>
    public class GeoService {
        #region Private Fields
        private static GeoService _instance;
        #endregion

        #region Events
        public event ServerEventHandler ResponseReceived;
        #endregion

        private GeoService() { }

        #region Private Properties
        /// <summary>
        /// Used to send information back to the GUI thread.
        /// </summary>
        private Dispatcher Dispatcher { set; get; }
        #endregion

        #region Public Properties
        /// <summary>
        /// The base address of the service.
        /// </summary>
        public string BaseAddress { get; set; }

        public static GeoService Instance {
            get {
                if(_instance == null)
                    _instance = new GeoService();
                return _instance;
            }
        }
        #endregion

        #region Private/Protected Methods
        /// <summary>
        /// Callback invoked after the completion of a service request.
        /// Parses the response and raises the ResponseReceived event.
        /// </summary>
        /// <param name="asyncResult">
        /// Callback information from the asynchronous process.
        /// </param>
        private void RequestCallback(IAsyncResult asyncResult) {
            HttpWebRequest request = (HttpWebRequest) asyncResult.AsyncState;
            HttpWebResponse response;
            try {
                response = (HttpWebResponse) request.EndGetResponse(asyncResult);
            } catch(WebException e) {
                response = (HttpWebResponse) e.Response;
            }
            ServerObject obj = null;
            if(response.StatusCode == HttpStatusCode.OK) {
                using(StreamReader reader = new StreamReader(response.GetResponseStream())) {
                    string responseString = reader.ReadToEnd();
                    reader.Close();
                    using(var ms = new MemoryStream(Encoding.Unicode.GetBytes(responseString))) {
                        var serializer = new DataContractJsonSerializer(typeof(ServerObject));
                        obj = (ServerObject) serializer.ReadObject(ms);
                    }
                }
            }
            ServerEventArgs args = new ServerEventArgs() {
                ResponseObject = obj,
                ServerResponse = response.StatusCode,
            };
            response.Close();
            Dispatcher.BeginInvoke(new Action(() => OnResponse(args)));
        }
        
        protected virtual void OnResponse(ServerEventArgs e) {
            if (ResponseReceived != null) ResponseReceived(this, e);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Makes a service call to obtain the latest version of all the data.
        /// </summary>
        /// <param name="dispatcher">Used to send the data back to the GUI thread</param>
        public void MakeRequest(Dispatcher dispatcher) {
            MakeRequest(dispatcher, -1);
        }

        /// <summary>
        /// Makes a service call to obtain the latest version of all the data.
        /// </summary>
        /// <param name="dispatcher">Used to send the data back to the GUI thread</param>
        /// <param name="version">Version of the current data in the phone</param>
        public void MakeRequest(Dispatcher dispatcher, double version) {
            Dispatcher = dispatcher;

            string requestString;
            if(version > 0) requestString = BaseAddress + "/mapareas?version=" + version.ToString();
            else requestString = BaseAddress + "/mapareas";
            HttpWebRequest request;
            try {
                request = (HttpWebRequest) WebRequest.Create(requestString);
            } catch {
                //TODO: Actually do something here
                //Raise error or notify what happened
                return;
            }
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.AllowAutoRedirect = true;

            //TODO: Do I need to have error handling here?
            // IAsyncResult result = (IAsyncResult) request.BeginG...
            request.BeginGetResponse(new AsyncCallback(RequestCallback), request);
        }
        #endregion
    }
}