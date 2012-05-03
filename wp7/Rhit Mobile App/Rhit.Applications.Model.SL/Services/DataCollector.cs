using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Browser;
using Rhit.Applications.Models.Events;
using Rhit.Applications.Models.Services.Requests;
using System;


#if WINDOWS_PHONE
using System.Device.Location;
#else
using Microsoft.Maps.MapControl;
#endif

namespace Rhit.Applications.Models.Services {
    public class DataCollector {
        #region Events
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

        #region ServerErrorReturned
        public event ServiceEventHandler ServerErrorReturned;
        protected virtual void OnServerErrorReturned(ServiceEventArgs args) {
            if(ServerErrorReturned != null) ServerErrorReturned(this, args);
        }
        #endregion

        #region Campus Services Events
        #region CampusServicesReturned
        public event CampusServicesEventHandler CampusServicesReturned;
        protected virtual void OnCampusServicesReturned(ServiceEventArgs e, CampusServicesCategory_DC root) {
            CampusServicesEventArgs args = new CampusServicesEventArgs(e) {
                Root = root
            };

            if(CampusServicesReturned != null) CampusServicesReturned(this, args);
        }
        #endregion

        #region CampusServicesUpdateReturned
        public event EventHandler CampusServicesUpdateReturned;
        protected virtual void OnCampusServicesUpdateReturned(EventArgs args)
        {
            if (CampusServicesUpdateReturned != null) CampusServicesUpdateReturned(this, args);
        }
        #endregion
        #endregion

        #region Tour Related Events
        #region ToursReturned
        public event DirectionsEventHandler ToursReturned;
        protected virtual void OnToursReturned(ServiceEventArgs e, IList<DirectionPath_DC> paths) {
            DirectionsEventArgs args = new DirectionsEventArgs(e) {
                Paths = paths,
            };
            if(ToursReturned != null) ToursReturned(this, args);
        }
        #endregion

        #region TagsReturned
        public event TagsEventHandler TagsReturned;
        protected virtual void OnTagsReturned(ServiceEventArgs e, TagsCategory_DC tagRoot) {
            TagsEventArgs args = new TagsEventArgs(e) {
                TagRoot = tagRoot,
            };
            if(TagsReturned != null) TagsReturned(this, args);
        }
        #endregion
        #endregion

        #region DirectionsReturned
        public event DirectionsEventHandler DirectionsReturned;
        protected virtual void OnDirectionsReturned(ServiceEventArgs e, IList<DirectionPath_DC> paths) {
            DirectionsEventArgs args = new DirectionsEventArgs(e) {
                Paths = paths,
            };
            if(DirectionsReturned != null) DirectionsReturned(this, args);
        }
        #endregion

        #region Location Events
        #region LocationUpdate
        public event LocationEventHandler LocationUpdate;
        protected virtual void OnLocationUpdate(ServiceEventArgs e, LocationData location) {
            LocationEventArgs args = new LocationEventArgs(e) {
                Location = location,
            };
            if(LocationUpdate != null) LocationUpdate(this, args);
        }
        #endregion

        #region LocationsReturned
        public event LocationsEventHandler LocationsReturned;
        protected virtual void OnLocationsUpdate(ServiceEventArgs e, IList<LocationData> locations) {
            LocationsEventArgs args = new LocationsEventArgs(e) {
                Locations = locations,
            };
            if(LocationsReturned != null) LocationsReturned(this, args);
        }
        #endregion

        #region SearchResultsReturned
        public event LocationsEventHandler SearchResultsReturned;
        protected virtual void OnSearchResultsReturned(ServiceEventArgs e, IList<LocationData> locations) {
            LocationsEventArgs args = new LocationsEventArgs(e) {
                Locations = locations,
            };
            if(SearchResultsReturned != null) SearchResultsReturned(this, args);
        }
        #endregion

        #region LocationDeleted
        public event LocationEventHandler LocationDeleted;
        protected virtual void OnLocationDeletion(ServiceEventArgs e, LocationData location) {
            LocationEventArgs args = new LocationEventArgs(e) {
                Location = location,
            };
            if(LocationDeleted != null) LocationDeleted(this, args);
        }
        #endregion
        #endregion

        #region PathDataReturned
        public event PathDataEventHandler PathDataReturned;
        protected virtual void OnPathDataReturned(PathDataEventArgs args) {
            if(PathDataReturned != null) PathDataReturned(this, args);
        }
        #endregion

        #region Node Events
        #region NodeCreated
        public event NodeEventHandler NodeCreated;
        protected virtual void OnNodeCreated(ServiceEventArgs e) {
            NodeEventArgs args = new NodeEventArgs(e) {
                Node = ServerObject.ParseNode(e.ResponseObject),
            };
            if(NodeCreated != null) NodeCreated(this, args);
        }
        #endregion

        #region NodeUpdated
        public event NodeEventHandler NodeUpdated;
        protected virtual void OnNodeUpdated(ServiceEventArgs e) {
            NodeEventArgs args = new NodeEventArgs(e) {
                Node = ServerObject.ParseNode(e.ResponseObject),
            };
            if(NodeUpdated != null) NodeUpdated(this, args);
        }
        #endregion

        #region NodeDeleted
        public event IdentificationEventHandler NodeDeleted;
        protected virtual void OnNodeDeleted(ServiceEventArgs e) {
            int id = (int) e.Request.UserMetaData["NodeId"];
            IdentificationEventArgs args = new IdentificationEventArgs(e) {
                Id = id,
            };
            if(NodeDeleted != null) NodeDeleted(this, args);
        }
        #endregion
        #endregion

        #region Path Events
        #region PathCreated
        public event PathEventHandler PathCreated;
        protected virtual void OnPathCreated(ServiceEventArgs e) {
            PathEventArgs args = new PathEventArgs(e) {
                Path = ServerObject.ParsePath(e.ResponseObject),
            };
            if(PathCreated != null) PathCreated(this, args);
        }
        #endregion

        #region PathUpdated
        public event PathEventHandler PathUpdated;
        protected virtual void OnPathUpdated(ServiceEventArgs e) {
            PathEventArgs args = new PathEventArgs(e) {
                Path = ServerObject.ParsePath(e.ResponseObject),
            };
            if(PathUpdated != null) PathUpdated(this, args);
        }
        #endregion

        #region PathDeleted
        public event IdentificationEventHandler PathDeleted;
        protected virtual void OnPathDeleted(ServiceEventArgs e) {
            int id = (int) e.Request.UserMetaData["PathId"];
            IdentificationEventArgs args = new IdentificationEventArgs(e) {
                Id = id,
            };
            if(PathDeleted != null) PathDeleted(this, args);
        }
        #endregion
        #endregion

        #region Direction Events
        #region DirectionCreated
        public event DirectionEventHandler DirectionCreated;
        protected virtual void OnDirectionCreated(ServiceEventArgs e) {
            DirectionEventArgs args = new DirectionEventArgs(e) {
                Direction = ServerObject.ParseDirection(e.ResponseObject),
            };
            if(DirectionCreated != null) DirectionCreated(this, args);
        }
        #endregion

        #region DirectionUpdated
        public event DirectionEventHandler DirectionUpdated;
        protected virtual void OnDirectionUpdated(ServiceEventArgs e) {
            DirectionEventArgs args = new DirectionEventArgs(e) {
                Direction = ServerObject.ParseDirection(e.ResponseObject),
            };
            if(DirectionUpdated != null) DirectionUpdated(this, args);
        }
        #endregion

        #region DirectionDeleted
        public event IdentificationEventHandler DirectionDeleted;
        protected virtual void OnDirectionDeleted(ServiceEventArgs e) {
            int id = (int) e.Request.UserMetaData["DirectionId"];
            IdentificationEventArgs args = new IdentificationEventArgs(e) {
                Id = id,
            };
            if(DirectionDeleted != null) DirectionDeleted(this, args);
        }
        #endregion
        #endregion

        #region DirectionMessage Events
        #region DirectionMessageCreated
        public event DirectionMessageEventHandler DirectionMessageCreated;
        protected virtual void OnDirectionMessageCreated(ServiceEventArgs e) {
            DirectionMessageEventArgs args = new DirectionMessageEventArgs(e) {
                DirectionMessage = ServerObject.ParseDirectionMessage(e.ResponseObject),
            };
            if(DirectionMessageCreated != null) DirectionMessageCreated(this, args);
        }
        #endregion

        #region DirectionMessageUpdated
        public event DirectionMessageEventHandler DirectionMessageUpdated;
        protected virtual void OnDirectionMessageUpdated(ServiceEventArgs e) {
            DirectionMessageEventArgs args = new DirectionMessageEventArgs(e) {
                DirectionMessage = ServerObject.ParseDirectionMessage(e.ResponseObject),
            };
            if(DirectionMessageUpdated != null) DirectionMessageUpdated(this, args);
        }
        #endregion

        #region DirectionMessageDeleted
        public event IdentificationEventHandler DirectionMessageDeleted;
        protected virtual void OnDirectionMessageDeleted(ServiceEventArgs e) {
            int id = (int) e.Request.UserMetaData["DirectionMessageId"];
            IdentificationEventArgs args = new IdentificationEventArgs(e) {
                Id = id,
            };
            if(DirectionMessageDeleted != null) DirectionMessageDeleted(this, args);
        }
        #endregion
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

        public static double ServicesVersion { get; private set; }

        private void SetVersion(double version, ServiceEventArgs args) {
            if(version == Version) return;
            Version = version;
            OnVersionUpdate(args, version, 0);
        }

        private void SetServicesVersion(double version, ServiceEventArgs args)
        {
            if (version == ServicesVersion) return;
            ServicesVersion = version;
            OnVersionUpdate(args, 0, version);
        }

        private IList<int> PathIds { get; set; }

        private void ResponseReceived(object sender, ServiceEventArgs eventArgs) {

            switch(eventArgs.Type) {
                case ResponseType.AllLocations:
                case ResponseType.TopLocations:
                case ResponseType.InternalLocations:
                    HandleLocationsResponse(eventArgs);
                    break;

                case ResponseType.LocationsSearch:
                    HandleSearchResponse(eventArgs);
                    break;

                case ResponseType.Tours:
                    HandleToursResponse(eventArgs);
                    break;

                case ResponseType.Tags:
                    HandleTagsResponse(eventArgs);
                    break;

                case ResponseType.Directions:
                    HandleDirectionsResponse(eventArgs);
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
                    SetVersion(eventArgs.ResponseObject.LocationsVersion, eventArgs);
                    SetServicesVersion(eventArgs.ResponseObject.ServicesVersion, eventArgs);
                    break;

                case ResponseType.Login:
                    HandleLoginResponse(eventArgs);
                    break;

                case ResponseType.PathData:
                    HandlePathDataResponse(eventArgs);
                    break;

                case ResponseType.CampusServices:
                    HandleCampusServicesResponse(eventArgs);
                    break;

                case ResponseType.CampusServicesUpdate:
                    HandleCampusServicesUpdateResponse(eventArgs);
                    break;

                case ResponseType.ServerError:
                case ResponseType.ConnectionError:
                    OnServerErrorReturned(eventArgs);
                    break;

                case ResponseType.NodeCreation:
                    OnNodeCreated(eventArgs);
                    break;
                case ResponseType.NodeUpdate:
                    OnNodeUpdated(eventArgs);
                    break;
                case ResponseType.NodeDeletion:
                    OnNodeDeleted(eventArgs);
                    break;

                case ResponseType.PathCreation:
                    OnPathCreated(eventArgs);
                    break;
                case ResponseType.PathUpdate:
                    OnPathUpdated(eventArgs);
                    break;
                case ResponseType.PathDeletion:
                    OnPathDeleted(eventArgs);
                    break;

                case ResponseType.DirectionMessageCreation:
                    OnDirectionMessageCreated(eventArgs);
                    break;
                case ResponseType.DirectionMessageUpdate:
                    OnDirectionMessageUpdated(eventArgs);
                    break;
                case ResponseType.DirectionMessageDeletion:
                    OnDirectionMessageDeleted(eventArgs);
                    break;


                case ResponseType.DirectionUpdate:
                    OnDirectionUpdated(eventArgs);
                    break;
                case ResponseType.DirectionDeletion:
                    OnDirectionDeleted(eventArgs);
                    break;


                case ResponseType.DirectionCreation:
                    if(PathIds == null || PathIds.Count <= 0) {
                        OnDirectionCreated(eventArgs);
                        break;
                    }
                    AddDirectionPaths(int.Parse(eventArgs.ResponseObject.Table[0][0]), PathIds);
                    PathIds.Clear();
                    break;

                case ResponseType.DirectionPathAddition:
                    OnDirectionCreated(eventArgs);
                    break;
            }
        }

        #region Response Handlers
        private void HandleToursResponse(ServiceEventArgs eventArgs) {
            ServerObject response = eventArgs.ResponseObject;
            if(response.Result.Paths == null || response.Result.Paths.Count <= 0) return;
            OnToursReturned(eventArgs, response.Result.Paths);
        }
        
        private void HandleTagsResponse(ServiceEventArgs eventArgs) {
            ServerObject response = eventArgs.ResponseObject;
            if(response.TagsRoot == null) return;
            OnTagsReturned(eventArgs, response.TagsRoot);
        }

        private void HandleDirectionsResponse(ServiceEventArgs eventArgs) {
            ServerObject response = eventArgs.ResponseObject;
            if(response.Result.Paths == null || response.Result.Paths.Count <= 0) return;
            OnDirectionsReturned(eventArgs, response.Result.Paths);
        }

        private void HandleCampusServicesResponse(ServiceEventArgs eventArgs) {
            ServerObject response = eventArgs.ResponseObject;
            if(response.CampusServicesRoot == null) return;
            SetServicesVersion(eventArgs.ResponseObject.Version, eventArgs);
            OnCampusServicesReturned(eventArgs, response.CampusServicesRoot);
        }

        private void HandleCampusServicesUpdateResponse(ServiceEventArgs eventArgs)
        {
            OnCampusServicesUpdateReturned(eventArgs);
        }

        private void HandleSearchResponse(ServiceEventArgs eventArgs) {
            ServerObject response = eventArgs.ResponseObject;
            if(response.Locations == null || response.Locations.Count <= 0) return;
            List<LocationData> locations = ServerObject.GetLocations(response.Locations);
            OnSearchResultsReturned(eventArgs, locations);
        }

        private void HandleLocationsResponse(ServiceEventArgs eventArgs) {
            ServerObject response = eventArgs.ResponseObject;
            if(response.Locations == null || response.Locations.Count <= 0) return;
            List<LocationData> locations = ServerObject.GetLocations(response.Locations);
            SetVersion(response.Version, eventArgs);
            OnLocationsUpdate(eventArgs, locations);
        }

        private void HandleChangeLocationResponse(ServiceEventArgs eventArgs) {
            var dict = eventArgs.Request.UserMetaData;

            //TODO: Why am i doing this?
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
                OnLocationDeletion(eventArgs, new LocationData() { Id = (int) eventArgs.Request.UserMetaData["LocationId"], });
                return;
            }
            ServerObject response = eventArgs.ResponseObject;
            if(response.Locations == null || response.Locations.Count <= 0) return;
            List<LocationData> locations = ServerObject.GetLocations(response.Locations);
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

            PathDataEventArgs args = new PathDataEventArgs(eventArgs) {
                Directions = response.Directions,
                Messages = response.Messages,
                Nodes = response.Nodes,
                Partitions = response.Partitions,
                Paths = response.Paths,
            };
            OnPathDataReturned(args);
        }
        #endregion

        #region Request Methods

        public void Login(string username, string password) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.Authenticate(username, password);
            Connection.MakeRequest(request, RequestType.Login);
        }

        #region Version Requests
        public void GetVersion() {
            RequestPart request = new RequestBuilder(BaseAddress).Version;
            Connection.MakeRequest(request, RequestType.Version);
        }

        public void IncreaseServerVersion() {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.UpdateVersion(Connection.ServiceTokenGuid, Version + 0.001);
            Connection.MakeRequest(request, RequestType.IncrementVersion);
        }

        public void IncreaseServicesVersion() {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.UpdateServicesVersion(Connection.ServiceTokenGuid, ServicesVersion + 0.001);
            Connection.MakeRequest(request, RequestType.IncrementVersion);
        }
        #endregion

        #region Tour Related Requests
        public void GetTags() {
            RequestPart request = new RequestBuilder(BaseAddress).Tours.Tags;
            Connection.MakeRequest(request, RequestType.Tags);
        }

        public void GetTour(int fromId, IList<int> tagIds) {
            RequestPart request = new RequestBuilder(BaseAddress).Tours.OnCampus.FromLoc(fromId).Tag(tagIds);
            Connection.MakeRequest(request, RequestType.Tours);
        }

        public void GetTestTour() {
            RequestPart request = new RequestBuilder(BaseAddress).Directions.ToursTest;
            Connection.MakeRequest(request, RequestType.Tours);
        }
        #endregion

        #region Direction Requests
        public void GetDirections(LocationData from, LocationData to) {
            GetDirections(from.Id, to.Id);
        }

        public void GetDirections(int fromId, int toId) {
            RequestPart request = new RequestBuilder(BaseAddress).Directions.FromLoc(fromId).ToLoc(toId);
            Connection.MakeRequest(request, RequestType.Directions);
        }

        public void GetTestDirections() {
            RequestPart request = new RequestBuilder(BaseAddress).Directions.Test;
            Connection.MakeRequest(request, RequestType.Directions);
        }
        #endregion

        #region Location Requests
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

        public void GetChildLocations(LocationData parent) { GetChildLocations(parent.Id); }

        public void GetChildLocations(int parentId) {
            RequestPart request = new RequestBuilder(BaseAddress).Locations.Data.Within(parentId);
            Connection.MakeRequest(request, RequestType.InternalLocations);
        }

        public void CreateLocation(int id, int parentId, string name, double latitude, double longitude, int floor) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spCreateLocation");
            request = request.AddQueryParameter("id", id);
            request = request.AddQueryParameter("name", name);
            request = request.AddQueryParameter("lat", latitude);
            request = request.AddQueryParameter("lon", longitude);
            request = request.AddQueryParameter("floor", floor);
            request = request.AddQueryParameter("parent", parentId);

            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata["LocationId"] = id;
            Connection.MakeMetaDataRequest(request, RequestType.LocationCreation, metadata);
        }

        public void DeleteLocation(int id) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeleteLocation");
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            request = request.AddQueryParameter("id", id);
            metadata["LocationId"] = id;
            Connection.MakeMetaDataRequest(request, RequestType.DeleteLocation, metadata);
        }

        public void MoveLocation(int id, double latitude, double longitude) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spMoveLocationCenter");
            request = request.AddQueryParameter("location", id);
            request = request.AddQueryParameter("lat", latitude);
            request = request.AddQueryParameter("lon", longitude);

            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata["LocationId"] = id;
            Connection.MakeMetaDataRequest(request, RequestType.MoveLocation, metadata);
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
        #endregion

        #region Campus Services Requests
        public void GetCampusServices() {
            RequestPart request = new RequestBuilder(BaseAddress).Services;
            Connection.MakeRequest(request, RequestType.CampusServices);
        }

        public void SaveCampusServiceCategory(CampusServicesCategory_DC category, String newCategoryName, String parentCategoryName)
        {
            String name = Uri.EscapeDataString(category.Name);
            String newName = Uri.EscapeDataString(newCategoryName);
            String newParent = Uri.EscapeDataString(parentCategoryName == null ? "\0" : parentCategoryName);

            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spUpdateCampusServiceCategory")
                .AddQueryParameter("name", name)
                .AddQueryParameter("newname", newName)
                .AddQueryParameter("newparent", newParent);

            Connection.MakeRequest(request, RequestType.CampusServicesUpdate);
        }

        public void SaveCampusServiceLink(Link_DC link, String newLinkName, String newLinkURL, String categoryName)
        {
            String category = Uri.EscapeDataString(categoryName == null ? "\0" : categoryName);
            String name = Uri.EscapeDataString(link.Name);
            String newCategory = category;
            String newName = Uri.EscapeDataString(newLinkName);
            String newURL = Uri.EscapeDataString(newLinkURL);

            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spUpdateCampusServiceLink")
                .AddQueryParameter("category", category)
                .AddQueryParameter("name", name)
                .AddQueryParameter("newcategory", newCategory)
                .AddQueryParameter("newname", newName)
                .AddQueryParameter("newurl", newURL);

            Connection.MakeRequest(request, RequestType.CampusServicesUpdate);
        }


        public void DeleteCampusServiceCategory(CampusServicesCategory_DC category, String parentCategoryName) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeleteCampusServiceCategory");

            String name = Uri.EscapeDataString(category.Name);

            request = request.AddQueryParameter("name", name);

            Connection.MakeRequest(request, RequestType.CampusServicesUpdate);
        }

        public void DeleteCampuServiceLink(Link_DC link, String categoryName) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeleteCampusServiceLink");

            String name = Uri.EscapeDataString(link.Name);
            String category = Uri.EscapeDataString(categoryName == null ? "\0" : categoryName);

            request = request.AddQueryParameter("name", name);
            request = request.AddQueryParameter("category", category);

            Connection.MakeRequest(request, RequestType.CampusServicesUpdate);
        }

        public void AddCampusServiceCategory(String name, CampusServicesCategory_DC parent) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spAddCampusServiceCategory");

            String escapedName = Uri.EscapeDataString(name);
            String parentName = Uri.EscapeDataString(parent == null || parent.Name == null ? "\0" : parent.Name);

            request = request.AddQueryParameter("name", escapedName);
            request = request.AddQueryParameter("parent", parentName);

            Connection.MakeRequest(request, RequestType.CampusServicesUpdate);
        }

        public void AddCampusServiceLink(String name, CampusServicesCategory_DC parent) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spAddCampusServiceLink");

            String url = Uri.EscapeDataString("http://www.rose-hulman.edu/");
            String escapedName = Uri.EscapeDataString(name);
            String parentName = Uri.EscapeDataString(parent == null || parent.Name == null ? "\0" : parent.Name);

            request = request.AddQueryParameter("name", escapedName);
            request = request.AddQueryParameter("url", url);
            request = request.AddQueryParameter("category", parentName);

            Connection.MakeRequest(request, RequestType.CampusServicesUpdate);
        }
        #endregion

        public void GetPathData() {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.PathData(Connection.ServiceTokenGuid);
            Connection.MakeRequest(request, RequestType.PathData);
        }
        
        #region Node Requests
        public void CreateNode(double latitude, double longitude, double altitude, bool outside, int? locationId) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spAddNode");
            request = request.AddQueryParameter("lat", latitude);
            request = request.AddQueryParameter("lon", longitude);
            request = request.AddQueryParameter("altitude", altitude);
            request = request.AddQueryParameter("outside", outside);
            request = request.AddQueryParameter("location", (object) locationId ?? "%00");
            Connection.MakeRequest(request, RequestType.NodeCreation);
        }

        public void UpdateNode(int id, double latitude, double longitude, double altitude, bool outside, int? locationId) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spUpdateNode");
            request = request.AddQueryParameter("id", id);
            request = request.AddQueryParameter("lat", latitude);
            request = request.AddQueryParameter("lon", longitude);
            request = request.AddQueryParameter("altitude", altitude);
            request = request.AddQueryParameter("outside", outside);
            request = request.AddQueryParameter("location", (object) locationId ?? "%00");
            Connection.MakeRequest(request, RequestType.NodeUpdate);
        }

        public void DeleteNode(int id) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeleteNode");
            request = request.AddQueryParameter("id", id);

            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata["NodeId"] = id;
            Connection.MakeMetaDataRequest(request, RequestType.NodeDeletion, metadata);
        }
        #endregion

        #region Path Requests
        public void CreatePath(int node1, int node2, bool elevator, int stairs, int partition) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spAddPath");

            request = request.AddQueryParameter("elevator", elevator);
            request = request.AddQueryParameter("node1", node1);
            request = request.AddQueryParameter("node2", node2);
            request = request.AddQueryParameter("partition", partition);
            request = request.AddQueryParameter("stairs", stairs);

            Connection.MakeRequest(request, RequestType.PathCreation);
        }


        public void UpdatePath(int id, int node1, int node2, bool elevator, int stairs, int partition) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spUpdatePath");

            request = request.AddQueryParameter("id", id);
            request = request.AddQueryParameter("elevator", elevator);
            request = request.AddQueryParameter("node1", node1);
            request = request.AddQueryParameter("node2", node2);
            request = request.AddQueryParameter("partition", partition);
            request = request.AddQueryParameter("stairs", stairs);

            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata["PathId"] = id;
            Connection.MakeRequest(request, RequestType.PathUpdate);
        }

        public void DeletePath(int id) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeletePath");
            request = request.AddQueryParameter("id", id);

            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata["PathId"] = id;
            Connection.MakeMetaDataRequest(request, RequestType.PathDeletion, metadata);
        }
        #endregion

        #region Direction Requests
        public void CreateDirection(int message, IList<int> paths, int? within) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spAddDirection");
            request = request.AddQueryParameter("startpath", paths[0]);
            request = request.AddQueryParameter("message", message);
            request = request.AddQueryParameter("within", (object) within ?? "%00");

            paths.RemoveAt(0);
            PathIds = paths;

            Connection.MakeRequest(request, RequestType.DirectionCreation);
        }

        public void AddDirectionPaths(int id, IList<int> paths) {
            List<RequestPart> requests = new List<RequestPart>();
            RequestPart request;
            foreach(int path in paths) {
                request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spAddDirectionPath");
                request = request.AddQueryParameter("direction", id);
                request = request.AddQueryParameter("path", path);
                requests.Add(request);
            }

            Connection.MakeRequests(requests, RequestType.DirectionPathAddition);
        }

        public void UpdateDirection(int id, int message, int? within) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spUpdateDirection");
            request = request.AddQueryParameter("id", id);
            request = request.AddQueryParameter("message", message);
            request = request.AddQueryParameter("within", (object) within ?? "%00");

            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata["DirectionId"] = id;
            Connection.MakeMetaDataRequest(request, RequestType.DirectionUpdate, metadata);
        }

        public void DeleteDirection(int id) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeleteDirection");
            request = request.AddQueryParameter("id", id);

            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata["DirectionId"] = id;
            Connection.MakeMetaDataRequest(request, RequestType.DirectionDeletion, metadata);
        }

        public void GetDirection(int id) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spGetDirectionRow");
            request = request.AddQueryParameter("id", id);

        }

        public void GetDirectionPaths(int id) {

        }
        #endregion

        #region DirectionMessage Requests
        public void AddBlankDirectionMessage() {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spAddDirectionMessage");
            request = request.AddQueryParameter("nodeoffset", 1);
            request = request.AddQueryParameter("message1", "New Direction Message");
            request = request.AddQueryParameter("message2", "New Direction Message");
            request = request.AddQueryParameter("action1", "%00");
            request = request.AddQueryParameter("action2", "%00");
            Connection.MakeRequest(request, RequestType.DirectionMessageCreation);
        }

        public void UpdateDirectionMessage(int id, int offset, string message1, string message2, string action1, string action2) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spUpdateDirectionMessage");
            request = request.AddQueryParameter("id", id);
            request = request.AddQueryParameter("nodeoffset", offset);
            request = request.AddQueryParameter("message1", message1);
            request = request.AddQueryParameter("message2", message2);
            request = request.AddQueryParameter("action1", action1);
            request = request.AddQueryParameter("action2", action2);
            Connection.MakeRequest(request, RequestType.DirectionMessageUpdate);
        }

        public void DeleteDirectionMessage(int id) {
            RequestPart request = new RequestBuilder(BaseAddress).Admin.StoredProcedure(Connection.ServiceTokenGuid, "spDeleteDirectionMessage");
            request = request.AddQueryParameter("id", id);

            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata["DirectionMessageId"] = id;
            Connection.MakeMetaDataRequest(request, RequestType.DirectionMessageDeletion, metadata);
        }
        #endregion
        #endregion
    }
}
