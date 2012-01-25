using System.Collections.Generic;
using System.Windows.Threading;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services.Requests;
using System;


#if WINDOWS_PHONE
using System.Device.Location;
#else
using Microsoft.Maps.MapControl;
#endif

namespace Rhit.Applications.Model.Services {
    public class DataCollector {
        #region Events
        #region VersionUpdate
        public event VersionEventHandler VersionUpdate;
        protected virtual void OnVersionUpdate(ServiceEventArgs e, double serverVersion, double servicesVersion) {
            VersionEventArgs args = new VersionEventArgs(e) {
                ServerVersion = serverVersion,
                ServicesVersion = servicesVersion,
            };
            if(VersionUpdate != null) VersionUpdate(this, args);
        }
        #endregion

        #region LocationUpdate
        public event LocationEventHandler LocationUpdate;
        protected virtual void OnLocationUpdate(ServiceEventArgs e, RhitLocation location) {
            LocationEventArgs args = new LocationEventArgs(e) {
                Location = location,
            };
            if(LocationUpdate != null) LocationUpdate(this, args);
        }
        #endregion

        #region LocationsReturned
        public event LocationsEventHandler LocationsReturned;
        protected virtual void OnLocationsUpdate(ServiceEventArgs e, IList<RhitLocation> locations) {
            LocationsEventArgs args = new LocationsEventArgs(e) {
                Locations = locations,
            };
            if(LocationsReturned != null) LocationsReturned(this, args);
        }
        #endregion

        #region LocationDeleted
        public event LocationEventHandler LocationDeleted;
        protected virtual void OnLocationDeletion(ServiceEventArgs e, RhitLocation location) {
            LocationEventArgs args = new LocationEventArgs(e) {
                Location = location,
            };
            if(LocationDeleted != null) LocationDeleted(this, args);
        }
        #endregion

        #region LoginResultsReturned
        public event AuthenticationEventHandler LoginResultsReturned;
        protected virtual void OnLoginResultsReturned(ServiceEventArgs e, string token, DateTime expiration) {
            AuthenticationEventArgs args = new AuthenticationEventArgs(e) {
                Token = token,
                Expiration = expiration,
            };
            if(LoginResultsReturned != null) LoginResultsReturned(this, args);
        }
        #endregion

        #region ServerErrorReturned
        public event ServiceEventHandler ServerErrorReturned;
        protected virtual void OnServerErrorReturned(ServiceEventArgs args) {
            if(ServerErrorReturned != null) ServerErrorReturned(this, args);
        }
        #endregion
        #endregion

        private DataCollector() {
            ResponseHandler.ResponseReceived += new ServiceEventHandler(ResponseReceived);
            BaseAddress = "";
        }

        #region Instance
        private static DataCollector _instance;
        public static DataCollector Instance {
            get {
                if(_instance == null) _instance = new DataCollector();
                return _instance;
            }
        }
        #endregion

        public string BaseAddress { get; set; }

        public static double Version { get; private set; }

        private void SetVersion(double version, ServiceEventArgs args) {
            if(version == Version) return;
            Version = version;
            OnVersionUpdate(args, version, 0);
        }

        private void ResponseReceived(object sender, ServiceEventArgs eventArgs) {

            switch(eventArgs.Type) {
                case ResponseType.AllLocations:
                case ResponseType.TopLocations:
                case ResponseType.InternalLocations:
                case ResponseType.LocationsSearch:
                    HandleLocationsResponse(eventArgs);
                    break;

                case ResponseType.Location:
                case ResponseType.DeleteLocation:
                    HandleLocationResponse(eventArgs);
                    break;

                case ResponseType.ChangeCorners:
                case ResponseType.MoveLocation:
                case ResponseType.LocationCreation:
                case ResponseType.LocationUpdate:
                    HandleChangeLocationResponse(eventArgs);
                    break;

                case ResponseType.IncrementVersion:
                    GetVersion();
                    break;

                case ResponseType.Version:
                    SetVersion(eventArgs.ResponseObject.ServerVersion, eventArgs);
                    break;

                case ResponseType.Login:
                    HandleLoginResponse(eventArgs);
                    break;

                case ResponseType.PathData:
                    HandlePathDataResponse(eventArgs);
                    break;

                case ResponseType.ServerError:
                case ResponseType.ConnectionError:
                    OnServerErrorReturned(eventArgs);
                    break;

            }
        }

        #region Response Handlers
        private void HandleLocationsResponse(ServiceEventArgs eventArgs) {
            ServerObject response = eventArgs.ResponseObject;
            if(response.Locations == null || response.Locations.Count <= 0) return;
            List<RhitLocation> locations = ServerObject.GetLocations(response.Locations);
            SetVersion(response.Version, eventArgs);
            OnLocationsUpdate(eventArgs, locations);
        }

        private void HandleChangeLocationResponse(ServiceEventArgs eventArgs) {
            var dict = eventArgs.Request.UserMetaData;
            if(dict.ContainsKey("LocationId")) GetLocation((int) dict["LocationId"]);
            if(dict.ContainsKey("OldLocationId")) GetLocation((int) dict["OldLocationId"]);
            if(dict.ContainsKey("NewLocationId")) GetLocation((int) dict["NewLocationId"]);
        }

        private void HandleLoginResponse(ServiceEventArgs eventArgs) {
            ServerObject response = eventArgs.ResponseObject;
            Connection.ServiceToken = response.Token;
            Connection.Expiration = response.Expiration;
            OnLoginResultsReturned(eventArgs, response.Token, response.Expiration);
        }

        private void HandleLocationResponse(ServiceEventArgs eventArgs) {
            if(eventArgs.Type == ResponseType.DeleteLocation) {
                OnLocationDeletion(eventArgs, new RhitLocation() { Id = (int) eventArgs.Request.UserMetaData["LocationId"], });
                return;
            }
            ServerObject response = eventArgs.ResponseObject;
            if(response.Locations == null || response.Locations.Count <= 0) return;
            List<RhitLocation> locations = ServerObject.GetLocations(response.Locations);
            SetVersion(response.Version, eventArgs);
            OnLocationUpdate(eventArgs, locations[0]);
        }

        private void HandlePathDataResponse(ServiceEventArgs eventArgs) {
            ServerObject response = eventArgs.ResponseObject;
            if(response.Directions == null || response.Directions.Count <= 0) return;
            if(response.Messages == null || response.Messages.Count <= 0) return;
            if(response.Nodes == null || response.Nodes.Count <= 0) return;
            if(response.Partitions == null || response.Partitions.Count <= 0) return;
            if(response.Paths == null || response.Paths.Count <= 0) return;
            SetVersion(response.Version, eventArgs);

            //TODO: Bryan - Send data to ViewModel
            //Example:
            //      List<RhitLocation> locations = ServerObject.GetLocations(response.Locations);
            //      OnLocationsUpdate(eventArgs, locations);
        }
        #endregion

        #region Request Methods
        public void Login(string username, string password) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.Authenticate(username, password);
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

        public void GetPathData() {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.PathData(Connection.ServiceTokenGuid);
            Connection.MakeRequest(request, RequestType.PathData);
        }

        public void CreateLocation(int id, int parentId, string name, double latitude, double longitude, int floor) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spCreateLocation");
            request = request.AddQueryParameter("id", id);
            request = request.AddQueryParameter("name", name);
            request = request.AddQueryParameter("lat", latitude);
            request = request.AddQueryParameter("lon", longitude);
            request = request.AddQueryParameter("floor", floor);
            request = request.AddQueryParameter("parent", parentId);
            Connection.MakeLocationRequest(request, RequestType.LocationCreation, id);
        }

        public void DeleteLocation(int id) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeleteLocation");
            request = request.AddQueryParameter("location", id);
            Connection.MakeLocationRequest(request, RequestType.DeleteLocation, id);
        }

        public void MoveLocation(int id, double latitude, double longitude) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spMoveLocationCenter");
            request = request.AddQueryParameter("location", id);
            request = request.AddQueryParameter("lat", latitude);
            request = request.AddQueryParameter("lon", longitude);
            Connection.MakeLocationRequest(request, RequestType.MoveLocation, id);
        }

#if !WINDOWS_PHONE
        public void ChangeLocationCorners(int id, IList<Location> newCorners) {
            IList<GeoCoordinate> _list = new List<GeoCoordinate>();
            foreach(Location location in newCorners)
                _list.Add(new GeoCoordinate(location));
            ChangeLocationCorners(id, _list);
        }
#endif

        public void ChangeLocationCorners(int id, IList<GeoCoordinate> newCorners) {
            List<RequestPart> requests = new List<RequestPart>();

            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeleteMapAreaCorners");
            request = request.AddQueryParameter("location", id);
            requests.Add(request);

            foreach(GeoCoordinate corner in newCorners) {
                request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spAddMapAreaCorner");
                request = request.AddQueryParameter("location", id);
                request = request.AddQueryParameter("lat", corner.Latitude);
                request = request.AddQueryParameter("lon", corner.Longitude);
                requests.Add(request);
            }

            Connection.MakeLocationRequests(requests, RequestType.ChangeCorners, id);
        }

        public void ChangeLocation(int oldId, int newId, string name, int floor, int parentId, string description, bool labelOnHybrid, int minZoom, LocationType type, IList<ILink> links, IList<string> altNames) {
            List<RequestPart> requests = new List<RequestPart>();
            RequestPart request;

            if(links != null) {
                request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeleteLinks");
                request = request.AddQueryParameter("location", oldId);
                requests.Add(request);

                foreach(ILink _link in links) {
                    request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spAddLink");
                    request = request.AddQueryParameter("location", oldId);
                    request = request.AddQueryParameter("name", _link.Name);
                    request = request.AddQueryParameter("url", _link.Address);
                    requests.Add(request);
                }
            }

            if(altNames != null) {
                request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeleteAlts");
                request = request.AddQueryParameter("location", oldId);
                requests.Add(request);

                foreach(string _name in altNames) {
                    request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spAddAlt");
                    request = request.AddQueryParameter("location", oldId);
                    request = request.AddQueryParameter("altname", _name);
                    requests.Add(request);
                }
            }

            request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spUpdateLocation");
            request = request.AddQueryParameter("id", oldId);
            request = request.AddQueryParameter("name", name);
            request = request.AddQueryParameter("newid", newId);
            request = request.AddQueryParameter("parent", parentId);
            request = request.AddQueryParameter("description", description);
            request = request.AddQueryParameter("labelonhybrid", labelOnHybrid);
            request = request.AddQueryParameter("minzoomlevel", minZoom);
            request = request.AddQueryParameter("type", Location_DC.ConvertTypeToTypeKey(type));
            request = request.AddQueryParameter("floor", floor);
            requests.Add(request);

            Connection.MakeLocationChangeRequest(requests, RequestType.LocationUpdate, oldId, newId);
        }

        public void IncreaseServerVersion() {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.UpdateVersion(Connection.ServiceTokenGuid, Version+0.001);
            Connection.MakeRequest(request, RequestType.IncrementVersion);
        }
        #endregion
    }
}
