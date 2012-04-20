using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using Rhit.Applications.Models.Events;

namespace Rhit.Applications.Models.Services {
    public static class ResponseHandler {

        public static event ServiceEventHandler ResponseReceived;

        private static void OnResponse(ServiceEventArgs e) {
            if(ResponseReceived != null) ResponseReceived(null, e);
        }

        public static void RequestCallback(AsyncServiceResult asyncResult) {
            ParseResponse(asyncResult);
        }

        private static ResponseType ConvertType(RequestType request) {
            switch(request) {
                case RequestType.NodeUpdate:
                    return ResponseType.NodeUpdate;
                case RequestType.PathDeletion:
                    return ResponseType.PathDeletion;
                case RequestType.PathCreation:
                    return ResponseType.PathCreation;
                case RequestType.NodeDeletion:
                    return ResponseType.NodeDeletion;
                case RequestType.NodeCreation:
                    return ResponseType.NodeCreation;
                case RequestType.AllLocations:
                    return ResponseType.AllLocations;
                case RequestType.CampusServices:
                    return ResponseType.CampusServices;
                case RequestType.CampusServicesUpdate:
                    return ResponseType.CampusServicesUpdate;
                case RequestType.CampusServicesVersionUpdate:
                    return ResponseType.CampusServicesVersionUpdate;
                case RequestType.ChangeCorners:
                    return ResponseType.ChangeCorners;
                case RequestType.DeleteLocation:
                    return ResponseType.DeleteLocation;
                case RequestType.Directions:
                    return ResponseType.Directions;
                case RequestType.IncrementVersion:
                    return ResponseType.IncrementVersion;
                case RequestType.InternalLocations:
                    return ResponseType.InternalLocations;
                case RequestType.Location:
                    return ResponseType.Location;
                case RequestType.LocationCreation:
                    return ResponseType.LocationCreation;
                case RequestType.LocationSearch:
                    return ResponseType.LocationsSearch;
                case RequestType.LocationUpdate:
                    return ResponseType.LocationUpdate;
                case RequestType.Login:
                    return ResponseType.Login;
                case RequestType.MoveLocation:
                    return ResponseType.MoveLocation;
                case RequestType.TopLocations:
                    return ResponseType.TopLocations;
                case RequestType.Version:
                    return ResponseType.Version;
                case RequestType.PathData:
                    return ResponseType.PathData;
                case RequestType.Tours:
                    return ResponseType.Tours;
                case RequestType.Tags:
                    return ResponseType.Tags;
            }
            return ResponseType.ServerError;
        }

        private static void ParseResponse(AsyncServiceResult result) {
            HttpWebRequest request = result.GetWebRequest();
            HttpWebResponse response;
            Exception exception;
            try {
                response = (HttpWebResponse) request.EndGetResponse(result.BaseResult);
                exception = null;
            } catch(WebException e) {
                response = (HttpWebResponse) e.Response;
                exception = e;
            } catch(SecurityException e) {
                response = null;
                exception = e;
            }
            ServerObject obj = null;
            string responseString = "";
            HttpStatusCode status;
            ResponseType type;

            if(response == null) {
                status = HttpStatusCode.NotFound;
                type = ResponseType.ConnectionError;
            } else if(response.StatusCode == HttpStatusCode.OK) {
                status = response.StatusCode;
                type = ConvertType(result.Request.Type);
                using(StreamReader reader = new StreamReader(response.GetResponseStream())) {
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    using(var ms = new MemoryStream(Encoding.Unicode.GetBytes(responseString))) {
                        var serializer = new DataContractJsonSerializer(typeof(ServerObject));
                        obj = (ServerObject) serializer.ReadObject(ms);
                    }
                }
            } else {
                type = ResponseType.ServerError;
                status = response.StatusCode;
            }

            if(obj == null) type = ResponseType.ServerError;

            ServiceEventArgs args = new ServiceEventArgs() {
                ResponseObject = obj,
                StatusCode = status,
                RawResponse = responseString,
                Request = result.Request,
                Type = type,
                Error = exception,
            };

            response.Close();

            Connection.Dispatcher.BeginInvoke(new Action(() => OnResponse(args)));
        }
    }
}

