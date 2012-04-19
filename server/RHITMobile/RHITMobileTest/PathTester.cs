using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Xml;

namespace RHITMobileTest {
    public static class PathTester {
        public static void RunTests(Path path) {
            RunTests(path, "http://localhost:5600" + path.GetPath());
        }

        private static void RunTests(Path path, string pathSoFar) {
            path.RunTests(pathSoFar);
            foreach (var nextPath in path.GetPaths()) {
                RunTests(nextPath, pathSoFar + nextPath.GetPath());
            }
        }

        public static T MakeRequest<T>(string path, out HttpStatusCode code, out string message)
            where T : class {
                var request = HttpWebRequest.Create(path);
                try {
                    var response = (HttpWebResponse)request.GetResponse();
                    code = response.StatusCode;
                    message = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (response.StatusCode != HttpStatusCode.OK) {
                        return null;
                    } else {
                        var result = Deserialize<T>(message);
                        if (result == null) {
                            code = HttpStatusCode.BadRequest;
                            return null;
                        }
                        return result;
                    }
                } catch (WebException e) {
                    code = ((HttpWebResponse)e.Response).StatusCode;
                    message = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                    return null;
                }
        }

        private static T Deserialize<T>(string json)
            where T : class {
                MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                try {
                    return (T)serializer.ReadObject(stream);
                } catch (SerializationException) {
                    return null;
                }
        }
    }
}
