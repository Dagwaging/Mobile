using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Rhit.Applications.Model.Events;
using System.Windows;

namespace Rhit.Applications.Model.Services {
    public static class ResponseHandler {


        public static event ServerEventHandler ResponseReceived;


        private static void OnResponse(ServerEventArgs e) {
            if(ResponseReceived != null) ResponseReceived(null, e);
        }

        #region Callbacks
        public static void RequestCallback(IAsyncResult asyncResult) {
            SendResults(ParseResponse(asyncResult));
        }

        public static void AllRequestCallback(IAsyncResult asyncResult) {
            SendResults(ParseResponse(asyncResult), ResponseType.All);
        }

        public static void TopRequestCallback(IAsyncResult asyncResult) {
            SendResults(ParseResponse(asyncResult), ResponseType.Top);
        }

        public static void SearchRequestCallback(IAsyncResult asyncResult) {
            SendResults(ParseResponse(asyncResult), ResponseType.Search);
        }
        #endregion

        private static ServerEventArgs ParseResponse(IAsyncResult asyncResult) {
            HttpWebRequest request = (HttpWebRequest) asyncResult.AsyncState;
            HttpWebResponse response;
            try {
                response = (HttpWebResponse) request.EndGetResponse(asyncResult);
            } catch(WebException e) {
                response = (HttpWebResponse) e.Response;
            }
            ServerObject obj = null;
            string responseString = "";
            if(response.StatusCode == HttpStatusCode.OK) {
                using(StreamReader reader = new StreamReader(response.GetResponseStream())) {
                    responseString = reader.ReadToEnd();
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
                RawResponse = responseString,
            };
            response.Close();
            return args;
        }

        private static void SendResults(ServerEventArgs args) {
            ServerObject response = args.ResponseObject;

            //Error Response
            if(response == null || args.ServerResponse != HttpStatusCode.OK)
                args.Type = ResponseType.Error;

            //Locations Response
            else if(response.Locations != null && response.Locations.Count > 0)
                args.Type = ResponseType.Locations;

            //Location Names Response
            else if(response.Names != null && response.Names.Count > 0)
                args.Type = ResponseType.Names;

            //Desription Response
            //TODO: This doesn't work, description can be empty
            else if(response.Description != null && response.Description != string.Empty)
                args.Type = ResponseType.Description;

            //Directions Response
            //TODO: Does this work? Can 'Done' be 0?
            else if(response.Done != 0)
                args.Type = ResponseType.Directions;

            //Printer Response
            //TODO: Does this work? Can 'Printer' be empty?
            else if(response.Printer != null && response.Printer != string.Empty)
                args.Type = ResponseType.Printer;

            //Authentication Response
            else if(response.Token != null && response.Token != string.Empty)
                args.Type = ResponseType.Authentication;

            else if(response.Table != null && response.Table.Count > 0)
                args.Type = ResponseType.StoredProc;

            else args.Type = ResponseType.Error;

            Connection.Dispatcher.BeginInvoke(new Action(() => OnResponse(args)));
        }

        private static void SendResults(ServerEventArgs args, ResponseType type) {
            args.Type = type;
            Connection.Dispatcher.BeginInvoke(new Action(() => OnResponse(args)));
        }
    }
}

