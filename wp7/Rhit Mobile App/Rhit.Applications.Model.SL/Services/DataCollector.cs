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
        public event ServiceEventHandler UpdateAvailable;
        public event SearchEventHandler SearchResultsAvailable;
        public event AuthenticationEventHandler LoginRequestReturned;
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
            if(LoginRequestReturned != null) LoginRequestReturned(this, e);
        }

        protected virtual void OnStoredProcReturned(StoredProcEventArgs e) {
            if(StoredProcReturned != null) StoredProcReturned(this, e);
        }

        protected virtual void OnServerError(ServiceEventArgs e) {
            if(ServerErrorReturned != null) ServerErrorReturned(this, e);
        }


        #region Public Methods
        public void Login(Dispatcher dispatcher, string username, string password) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin(username, password);
            Connection.MakeRequest(dispatcher, request);
        }

        public void SearchLocations(Dispatcher dispatcher, string search) {
            RequestPart request = new RequestBuilder(BaseAddress, search).Locations.Data.All;
            Connection.MakeRequest(dispatcher, request, true, null);
        }

        public List<RhitLocation> GetAllLocations(Dispatcher dispatcher) {
            return GetAllLocations(dispatcher, true);
        }

        public List<RhitLocation> GetAllLocations(Dispatcher dispatcher, bool withDescription) {
            //Note: This does not make service calls if the version is up to date and it has all of the locations
            //Note: Does not guarantee that all locations have descriptions
            if(dispatcher != null && (!UpToDate || !DataStorage.IsAllFull)) {
                RequestPart request;
                if(!withDescription) request = new RequestBuilder(BaseAddress).Locations.Data.All.NoDesc;
                else request = new RequestBuilder(BaseAddress).Locations.Data.All;
                Connection.MakeRequest(dispatcher, request);
            }
            return new List<RhitLocation>(DataStorage.Instance.AllLocations.Values);
        }

        public List<RhitLocation> GetTopLocations(Dispatcher dispatcher) {
            if(!UpToDate) {
                RequestPart request = new RequestBuilder(BaseAddress, Version).Locations.Data.Top;
                Connection.MakeRequest(dispatcher, request);
            }
            return new List<RhitLocation>(DataStorage.Instance.TopLocations.Values);
        }

        public RhitLocation GetLocation(Dispatcher dispatcher, int id) {
            if(DataStorage.Instance.AllLocations.ContainsKey(id))
                return DataStorage.Instance.AllLocations[id];
            RequestPart request = new RequestBuilder(BaseAddress).Locations.Data.Id(id);
            Connection.MakeRequest(dispatcher, request);
            return null;
        }

        public List<RhitLocation> GetChildLocations(Dispatcher dispatcher, RhitLocation parent) {
            return GetChildLocations(dispatcher, parent.Id);
        }

        private List<RhitLocation> GetChildren(int parentId) {
            List<RhitLocation> locations = new List<RhitLocation>();
            foreach(RhitLocation location in DataStorage.Instance.AllLocations.Values) {
                if(location.ParentId == parentId) locations.Add(location);
            }
            return locations;
        }

        private List<RhitLocation> GetLocations(bool departable) {
            List<RhitLocation> locations = new List<RhitLocation>();
            foreach(RhitLocation location in DataStorage.Instance.AllLocations.Values) {
                if(!departable || location.IsDepartable) locations.Add(location);
            }
            return locations;
        }

        public List<RhitLocation> GetChildLocations(Dispatcher dispatcher, int parentId) {
            List<RhitLocation> locations = GetChildren(parentId);
            if(locations == null || locations.Count == 0) {
                RequestPart request = new RequestBuilder(BaseAddress).Locations.Data.Within(parentId);
                Connection.MakeRequest(dispatcher, request);
                return null;
            }
            return locations;
        }

        public List<RhitLocation> GetLocationNames(Dispatcher dispatcher) {
            return GetLocationNames(dispatcher, false);
        }

        public List<RhitLocation> GetLocationNames(Dispatcher dispatcher, bool departable) {
            if(!DataStorage.IsAllFull) {
                RequestPart request;
                if(!departable) request = new RequestBuilder(BaseAddress).Locations.Names;
                else request = new RequestBuilder(BaseAddress).Locations.Names.Departable;
                Connection.MakeRequest(dispatcher, request);
                return GetLocations(departable);
            }
            return GetLocations(departable);
        }

        public void GetLocationDescription(Dispatcher dispatcher, RhitLocation location) {
            GetLocationDescription(dispatcher, location.Id);
        }

        public void GetLocationDescription(Dispatcher dispatcher, int id) {
            //Note: Assumes that the location doesn't already have its description
            RequestPart request = new RequestBuilder(BaseAddress).Locations.Desc(id);
            Connection.MakeRequest(dispatcher, request);
        }

        public List<RhitLocation> GetMapAreas(Dispatcher dispatcher) {
            if(!UpToDate) {
                RequestPart request = new RequestBuilder(BaseAddress, Version).Locations.Data.Top;
                Connection.MakeRequest(dispatcher, request);
            }
            List<RhitLocation> locations = new List<RhitLocation>();
            foreach(RhitLocation location in DataStorage.Instance.TopLocations.Values)
                if(location.Locations != null && location.Locations.Count > 0)
                    locations.Add(location);
            return locations;
        }

        public List<RhitLocation> GetQuickList(Dispatcher dispatcher) {
            if(!UpToDate) {
                RequestPart request = new RequestBuilder(BaseAddress, Version).Locations.Data.Top;
                Connection.MakeRequest(dispatcher, request);
            }
            List<RhitLocation> locations = new List<RhitLocation>();
            foreach(RhitLocation location in DataStorage.Instance.TopLocations.Values)
                if(location.Type == LocationType.OnQuickList) locations.Add(location);
            return locations;
        }

        public List<RhitLocation> GetPointsOfInterest(Dispatcher dispatcher) {
            if(!UpToDate) {
                RequestPart request = new RequestBuilder(BaseAddress, Version).Locations.Data.Top;
                Connection.MakeRequest(dispatcher, request);
            }
            List<RhitLocation> locations = new List<RhitLocation>();
            foreach(RhitLocation location in DataStorage.Instance.TopLocations.Values)
                if(location.Type == LocationType.PointOfInterest) locations.Add(location);
            return locations;
        }

        public void ExecuteStoredProcedure(Dispatcher dispatcher, string procedure, Dictionary<string, object> query) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin(Connection.ServiceTokenGuid, procedure);
            foreach (var pair in query) {
                request = request.AddQueryParameter(pair.Key, pair.Value);
            }
            Connection.MakeRequest(dispatcher, request);
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
        #endregion
    }
}
