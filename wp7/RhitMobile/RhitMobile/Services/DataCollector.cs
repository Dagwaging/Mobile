using System.Collections.Generic;
using System.Net;
using System.Windows.Threading;
using RhitMobile.Events;
using RhitMobile.ObjectModel;
using RhitMobile.Services.Requests;
using System;

namespace RhitMobile.Services {
    /// <summary>
    /// Singleton class used to make service calls.
    /// </summary>
    public class DataCollector {
        #region Private Fields
        private static DataCollector _instance;
        #endregion

        #region Events
        public event ServiceEventHandler UpdateAvailable;
        public event SearchEventHandler SearchResultsAvailable;
        #endregion

        private DataCollector() {
            ResponseHandler.ResponseReceived += new ServerEventHandler(GeoService_ResponseReceived);
            ResponseHandler.AllResponseReceived += new ServerEventHandler(GeoService_AllResponseReceived);
            ResponseHandler.TopResponseReceived += new ServerEventHandler(GeoService_TopResponseReceived);
            ResponseHandler.SearchResponseReceived += new ServerEventHandler(GeoService_SearchResponseReceived);
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
            List<RhitLocation> locations = ServerObject.GetLocations(response.Locations);
            DataStorage.Instance.MergeData(locations, StorageKey.All);
            OnUpdated(new ServiceEventArgs());
        }

        private void GeoService_AllResponseReceived(object sender, ServerEventArgs e) {
            ServerObject response = GetServerObject(e);
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

        private void GeoService_TopResponseReceived(object sender, ServerEventArgs e) {
            ServerObject response = GetServerObject(e);
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

        private void GeoService_SearchResponseReceived(object sender, ServerEventArgs e) {
            ServerObject response = GetServerObject(e);
            
            SearchEventArgs args = new SearchEventArgs() {
                Places = ServerObject.GetLocations(response.Locations),
            };
            OnSearchUpdated(args);
        }

        protected virtual void OnUpdated(ServiceEventArgs e) {
            if(UpdateAvailable != null) UpdateAvailable(this, e);
        }

        protected virtual void OnSearchUpdated(SearchEventArgs e) {
            if(SearchResultsAvailable != null) SearchResultsAvailable(this, e);
        }
        #endregion

        #region Public Methods
        public void SearchLocations(Dispatcher dispatcher, string search) {
            RequestPart request = new RequestBuilder(BaseAddress, search).Locations.Data.All;
            GeoService.MakeRequest(dispatcher, request, true);
        }

        public List<RhitLocation> GetAllLocations(Dispatcher dispatcher) {
            return GetAllLocations(dispatcher, true);
        }

        public List<RhitLocation> GetAllLocations(Dispatcher dispatcher, bool withDescription) {
            //Note: This only makes service calls if the version is up to date and it has all of the locations
            //Note: Does not guarantee that all locations have descriptions
            if(!UpToDate || !DataStorage.IsAllFull) {
                RequestPart request;
                if(!withDescription) request = new RequestBuilder(BaseAddress).Locations.Data.All.NoDesc;
                else request = new RequestBuilder(BaseAddress).Locations.Data.All;
                GeoService.MakeRequest(dispatcher, request);
            }
            return new List<RhitLocation>(DataStorage.Instance.AllLocations.Values);
        }

        public List<RhitLocation> GetTopLocations(Dispatcher dispatcher) {
            if(!UpToDate) {
                RequestPart request = new RequestBuilder(BaseAddress, Version).Locations.Data.Top;
                GeoService.MakeRequest(dispatcher, request);
            }
            return new List<RhitLocation>(DataStorage.Instance.TopLocations.Values);
        }

        public RhitLocation GetLocation(Dispatcher dispatcher, int id) {
            if (DataStorage.Instance.AllLocations.ContainsKey(id))
                return DataStorage.Instance.AllLocations[id];
            RequestPart request = new RequestBuilder(BaseAddress).Locations.Data.Id(id);
            GeoService.MakeRequest(dispatcher, request);
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
                GeoService.MakeRequest(dispatcher, request);
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
                GeoService.MakeRequest(dispatcher, request);
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
            GeoService.MakeRequest(dispatcher, request);
        }

        public List<RhitLocation> GetMapAreas(Dispatcher dispatcher) {
            if(!UpToDate) {
                RequestPart request = new RequestBuilder(BaseAddress, Version).Locations.Data.Top;
                GeoService.MakeRequest(dispatcher, request);
            }
            List<RhitLocation> locations = new List<RhitLocation>();
            foreach(RhitLocation location in DataStorage.Instance.TopLocations.Values)
                if(location.Locations != null && location.Locations.Count > 0)
                    locations.Add(location);
            return locations;
        }

        public List<RhitLocation> GetQuikList(Dispatcher dispatcher) {
            if(!UpToDate) {
                RequestPart request = new RequestBuilder(BaseAddress, Version).Locations.Data.Top;
                GeoService.MakeRequest(dispatcher, request);
            }
            List<RhitLocation> locations = new List<RhitLocation>();
            foreach(RhitLocation location in DataStorage.Instance.TopLocations.Values)
                if(location.OnQuikList) locations.Add(location);
            return locations;
        }

        public List<RhitLocation> GetPointsOfInterest(Dispatcher dispatcher) {
            if(!UpToDate) {
                RequestPart request = new RequestBuilder(BaseAddress, Version).Locations.Data.Top;
                GeoService.MakeRequest(dispatcher, request);
            }
            List<RhitLocation> locations = new List<RhitLocation>();
            foreach(RhitLocation location in DataStorage.Instance.TopLocations.Values)
                if(location.IsPOI) locations.Add(location);
            return locations;
        }
        #endregion
    }
}
