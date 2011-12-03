using System.Collections.Generic;
using System.Net;
using System.Windows.Threading;
using System;
using Rhit.Admin.Model.Events;
using Rhit.Admin.Model.Services.Requests;
using System.Windows;


namespace Rhit.Admin.Model.Services {
    public class DataCollector {
        private static DataCollector _instance;

        public event ServiceEventHandler UpdateAvailable;
        
        private DataCollector() {
            ResponseHandler.ResponseReceived += new ServerEventHandler(GeoService_ResponseReceived);
            BaseAddress = "";
            Version = 0;
        }

        #region Public Properties
        /// <summary>
        /// The base address of the service.
        /// </summary>
        public string BaseAddress { get; set; }

        public static DataCollector Instance {
            get {
                if(_instance == null) _instance = new DataCollector();
                return _instance;
            }
        }

        public double Version { get; set; }
        #endregion

        #region Private/Protected Methods
        private ServerObject GetServerObject(ServerEventArgs e) {
            //TODO: Use response to do display error messages
            HttpStatusCode response = e.ServerResponse;

            ServerObject serverObject = e.ResponseObject;
            if(serverObject == null) return null; //TODO: Raise erorr or something
            return serverObject;
        }

        private void GeoService_ResponseReceived(object sender, ServerEventArgs e) {
            ServerObject response = GetServerObject(e);
            if(response == null || response.Locations == null || response.Locations.Count == 0) { return; }

            ServiceEventArgs args = new ServiceEventArgs() {
                Locations = ServerObject.GetLocations(response.Locations),
            };
            OnUpdated(args);
        }

        protected virtual void OnUpdated(ServiceEventArgs e) {
            if(UpdateAvailable != null) UpdateAvailable(this, e);
        }
        #endregion

        public void RetrieveAllLocations(Dispatcher dispatcher) {
            RequestPart request = new RequestBuilder(BaseAddress).Locations.Data.All;
            Connection.MakeRequest(dispatcher, request);
        }
    }
}
