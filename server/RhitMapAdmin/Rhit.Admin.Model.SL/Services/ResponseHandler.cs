using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Rhit.Admin.Model.Events;
using System.Windows;

namespace Rhit.Admin.Model.Services {
    public static class ResponseHandler {
        public static event ServerEventHandler ResponseReceived;

        private static void OnResponse(ServerEventArgs e) {
            if(ResponseReceived != null) ResponseReceived(null, e);
        }

        public static void RequestCallback(IAsyncResult asyncResult) {
            SendResults(ParseResponse(asyncResult));
        }

        private static ServerEventArgs ParseResponse(IAsyncResult asyncResult) {
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

            //ServerEventArgs args = new ServerEventArgs() {
            //    ResponseObject = new ServerObject(),
            //    ServerResponse = HttpStatusCode.OK,
            //};

            ServerEventArgs args = new ServerEventArgs() {
                ResponseObject = obj,
                ServerResponse = response.StatusCode,
            };
            response.Close();
            return args;
        }

        private static void SendResults(ServerEventArgs args) {
            Connection.Dispatcher.BeginInvoke(new Action(() => OnResponse(args)));
        }
    }
}
