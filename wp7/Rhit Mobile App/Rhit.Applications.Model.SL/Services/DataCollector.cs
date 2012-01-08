using System.Collections.Generic;
using System.Net;
using System.Windows.Threading;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services.Requests;
using System;
using System.Threading;

namespace Rhit.Applications.Model.Services {
    public class DataCollector {
        #region Private Fields
        private static DataCollector _instance;
        #endregion

        #region Events

        #region VersionUpdate
        public event VersionEventHandler VersionUpdate;
        protected virtual void OnVersionUpdate(VersionEventArgs e) {
            if(VersionUpdate != null) VersionUpdate(this, e);
        }
        #endregion


        public event ServiceEventHandler UpdateAvailable;

        public event SearchEventHandler SearchResultsAvailable;

        public event AuthenticationEventHandler LoginResultsReturned;

        public event StoredProcEventHandler StoredProcReturned;
        public event ServiceEventHandler ServerErrorReturned;
        #endregion

        private DataCollector() {
            ResponseHandler.ResponseReceived += new ServerEventHandler(ResponseReceived);
            //ResponseHandler.AllResponseReceived += new ServerEventHandler(GeoService_AllResponseReceived);
            //ResponseHandler.TopResponseReceived += new ServerEventHandler(GeoService_TopResponseReceived);
            //ResponseHandler.SearchResponseReceived += new ServerEventHandler(GeoService_SearchResponseReceived);
            BaseAddress = "";
            Version = 0;
            UpToDate = false;
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

        public bool UpToDate { get; set; }
        #endregion

        private ServerObject GetServerObject(ServerEventArgs e) {
            //TODO: Use response to do display error messages
            HttpStatusCode response = e.ServerResponse;

            ServerObject serverObject = e.ResponseObject;
            if(serverObject == null) return null; //TODO: Raise erorr or something
            return serverObject;
        }

        private void ResponseReceived(object sender, ServerEventArgs e) {
            ServerObject response = GetServerObject(e);

            switch(e.Type) {
                case ResponseType.All:
                    HandleAllResponse(response);
                    break;
                case ResponseType.Top:
                    HandleTopResponse(response);
                    break;
                case ResponseType.Search:
                    HandleSearchResponse(response);
                    break;
                case ResponseType.Authentication:
                    HandleLoginResponse(response);
                    break;
                case ResponseType.StoredProc:
                    HandleStoredProcResponse(response);
                    break;
                case ResponseType.Error:
                    HandleErrorResponse(response);
                    break;
                default:
                    HandleLocationsResponse(response);
                    break;
            }
        }

        #region Response Handlers
        private void HandleLoginResponse(ServerObject response) {
            Connection.ServiceToken = response.Token;
            Connection.Expiration = response.Expiration;
            AuthenticationEventArgs args = new AuthenticationEventArgs() {
                Authorized = true,
                Expiration = response.Expiration,
                Token = response.Token
            };
            OnLoginRequestReturned(args);
        }

        private void HandleErrorResponse(ServerObject response) {
            //TODO: Provide More information about the error
            OnServerError(new ServiceEventArgs());
        }

        private void HandleLocationsResponse(ServerObject response) {
            List<RhitLocation> locations = ServerObject.GetLocations(response.Locations);
            DataStorage.Instance.MergeData(locations, StorageKey.All);
            OnUpdated(new ServiceEventArgs());
        }

        private void HandleAllResponse(ServerObject response) {
            if(response == null || response.Locations == null || response.Locations.Count == 0) {
                UpToDate = true;
                return;
            }
            List<RhitLocation> locations = ServerObject.GetLocations(response.Locations);
            DataStorage.Instance.OverwriteData(locations, StorageKey.All);
            Version = response.Version;
            DataStorage.Version = Version;
            OnUpdated(new ServiceEventArgs());
            UpToDate = true;
        }

        private void HandleTopResponse(ServerObject response) {
            if(response == null || response.Locations == null || response.Locations.Count == 0) {
                UpToDate = true;
                return;
            }
            List<RhitLocation> locations = ServerObject.GetLocations(response.Locations);
            DataStorage.Instance.OverwriteData(locations, StorageKey.Top);
            Version = response.Version;
            DataStorage.Version = Version;
            OnUpdated(new ServiceEventArgs());
            UpToDate = true;
        }

        private void HandleSearchResponse(ServerObject response) {
            SearchEventArgs args = new SearchEventArgs() {
                Places = ServerObject.GetLocations(response.Locations),
            };
            OnSearchUpdated(args);
        }

        private void HandleStoredProcResponse(ServerObject response) {
            StoredProcEventArgs args = new StoredProcEventArgs() {
                Columns = response.Columns,
                Table = response.Table
            };
            OnStoredProcReturned(args);
        }
        #endregion

        protected virtual void OnUpdated(ServiceEventArgs e) {
            if(UpdateAvailable != null) UpdateAvailable(this, e);
        }

        protected virtual void OnSearchUpdated(SearchEventArgs e) {
            if(SearchResultsAvailable != null) SearchResultsAvailable(this, e);
        }

        protected virtual void OnLoginRequestReturned(AuthenticationEventArgs e) {
            if(LoginResultsReturned != null) LoginResultsReturned(this, e);
        }

        protected virtual void OnStoredProcReturned(StoredProcEventArgs e) {
            if(StoredProcReturned != null) StoredProcReturned(this, e);
        }

        protected virtual void OnServerError(ServiceEventArgs e) {
            if(ServerErrorReturned != null) ServerErrorReturned(this, e);
        }


        #region Public Methods
        public void Login(string username, string password) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin(username, password);
            Connection.MakeRequest(request, RequestType.Login);
        }

        public void GetVersion() {
            RequestPart request = new RequestBuilder(BaseAddress).Version;
            Connection.MakeRequest(request, RequestType.Version);
        }

        public void GetAllLocations() { GetAllLocations(true); }

        public void GetAllLocations(bool withDescription) {
            RequestPart request;
            if(!withDescription) request = new RequestBuilder(BaseAddress).Locations.Data.All.NoDesc;
            else request = new RequestBuilder(BaseAddress).Locations.Data.All;
            Connection.MakeRequest(request, RequestType.AllLocations);
        }

        public void SearchLocations(Dispatcher dispatcher, string search) {
            RequestPart request = new RequestBuilder(BaseAddress, search).Locations.Data.All;
            Connection.MakeRequest(request, RequestType.LocationSearch);
        }

        public void GetTopLocations(Dispatcher dispatcher) {
            RequestPart request = new RequestBuilder(BaseAddress, Version).Locations.Data.Top;
            Connection.MakeRequest(request, RequestType.TopLocations);
        }

        public void GetLocation(int id) {
            RequestPart request = new RequestBuilder(BaseAddress).Locations.Data.Id(id);
            Connection.MakeRequest(request, RequestType.Location);
        }

        public void GetChildLocations(RhitLocation parent) { GetChildLocations(parent.Id); }


        public void GetChildLocations(int parentId) {
            RequestPart request = new RequestBuilder(BaseAddress).Locations.Data.Within(parentId);
            Connection.MakeRequest(request, RequestType.InternalLocations);
        }

        public void GetLocationNames() {
                RequestPart request = new RequestBuilder(BaseAddress).Locations.Names;
                Connection.MakeRequest(request, RequestType.Names);
        }

        public void GetDescription(RhitLocation location) {
            GetDescription(location.Id);
        }

        public void GetDescription(int id) {
            RequestPart request = new RequestBuilder(BaseAddress).Locations.Desc(id);
            Connection.MakeRequest(request, RequestType.LocationDescription);
        }

        public void CreateLocation(int id, string name, double latitude, double longitude, int floor) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin(Connection.ServiceTokenGuid, "spCreateLocation");
            request = request.AddQueryParameter("location", id);
            request = request.AddQueryParameter("name", name);
            request = request.AddQueryParameter("lat", latitude);
            request = request.AddQueryParameter("lon", longitude);
            request = request.AddQueryParameter("floor", floor);
            Connection.MakeLocationChangeRequest(request, RequestType.LocationCreation, id);
        }

        public void DeleteLocation(int id) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin(Connection.ServiceTokenGuid, "spDeleteLocation");
            request = request.AddQueryParameter("location", id);
            Connection.MakeLocationChangeRequest(request, RequestType.DeleteLocation, id);
        }

        public void MoveLocation(int id, double latitude, double longitude) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin(Connection.ServiceTokenGuid, "spMoveLocationCenter");
            request = request.AddQueryParameter("location", id);
            request = request.AddQueryParameter("lat", latitude);
            request = request.AddQueryParameter("lon", longitude);
            Connection.MakeLocationChangeRequest(request, RequestType.MoveLocation, id);
        }

        public void ExecuteBatchStoredProcedure(Dispatcher dispatcher, List<KeyValuePair<string, Dictionary<string, object>>> executions) {
            var batchRequest = new Rhit.Applications.Model.Services.Requests.BatchRequest(dispatcher);
            foreach (var execution in executions) {
                RequestPart request = new RequestBuilder(BaseAddress).Admin(Connection.ServiceTokenGuid, execution.Key);
                foreach (var parameter in execution.Value) {
                    request = request.AddQueryParameter(parameter.Key, parameter.Value);
                }
                batchRequest.AddRequest(request);
            }
            batchRequest.Start();
        }

        public void UpdateServerVersion(Dispatcher dispatcher) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin(Connection.ServiceTokenGuid, Version);
            Connection.MakeRequest(request, RequestType.IncrementVersion);
        }
        #endregion
    }
}
