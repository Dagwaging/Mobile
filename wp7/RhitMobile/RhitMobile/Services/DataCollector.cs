using System.Collections.Generic;
using System.Net;
using System.Windows.Threading;
using RhitMobile.Events;
using RhitMobile.ObjectModel;

namespace RhitMobile.Services {
    /// <summary>
    /// Singleton class used to make service calls.
    /// </summary>
    public class DataCollector {
        #region Private Fields
        private static DataCollector _instance;
        #endregion

        #region Events
        public event ServerEventHandler UpdateAvailable;
        #endregion

        private DataCollector() {
            GeoService.Instance.ResponseReceived += new ServerEventHandler(GeoService_ResponseReceived);
        }

        #region Public Properties
        public static DataCollector Instance {
            get {
                if(_instance == null) _instance = new DataCollector();
                return _instance;
            }
        }
        #endregion

        #region Private/Protected Methods
        /// <summary>
        /// Event callback to handler service responses.
        /// </summary>
        private void GeoService_ResponseReceived(object sender, ServerEventArgs e) {
            //TODO: Use response to do display error messages
            HttpStatusCode response = e.ServerResponse;

            ServerObject serverObject = e.ResponseObject;
            if(serverObject == null) return;
            StateManagment.SaveState(null, "MapAreas", serverObject);
            OnUpdated(e);
        }

        protected virtual void OnUpdated(ServerEventArgs e) {
            if(UpdateAvailable != null) UpdateAvailable(this, e);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Makes a service call to get/update all of the available map data.
        /// </summary>
        /// <param name="dispatcher">Used to send the data back to the GUI thread</param>
        /// <returns>Map data that is currently in isolated storage</returns>
        public List<RhitLocation> GetLocations(Dispatcher dispatcher) {
            //TODO: Is this making the call too often?
            //Should it be only when the app starts up?
            //But then what if the first one fails?
            ServerObject serverObject = StateManagment.LoadState<ServerObject>(null, "MapAreas", null);
            if(serverObject == null) {
                GeoService.Instance.MakeRequest(dispatcher);
                return null;
            } 
            GeoService.Instance.MakeRequest(dispatcher, serverObject.Version);
            return serverObject.GetLocations();
        }
        #endregion
    }
}
