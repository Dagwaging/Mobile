using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FakeClient
{
    public class Tracker {
        public Tracker() {
            Times = new List<TimeSpan>();
            BadResponses = new List<HttpWebResponse>();
        }

        public List<TimeSpan> Times { get; set; }

        public List<HttpWebResponse> BadResponses { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tracker = new Tracker();
            var numExec = 30;

            var tasks = new List<Task>();
            for (int i = 0; i < numExec; i++) {
                tasks.Add(Task.Factory.StartNew(new Action<object>((t) => {
                    var start = DateTime.Now;
                    var request = HttpWebRequest.Create("http://localhost:5600/tours/oncampus/fromloc/111/1/201");
                    var response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK) {
                        ((Tracker)t).BadResponses.Add(response);
                    }
                    ((Tracker)t).Times.Add(DateTime.Now - start);
                }), tracker));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine("Average Time: " + tracker.Times.Sum(ts => ts.TotalSeconds) / numExec + " seconds");

            Console.ReadLine();
        }
    }

    public static class JsonUtility
    {
        public static T Deserialize<T>(this string json)
            where T : class
        {
            MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }

        public static string Serialize<T>(this T obj)
            where T : class
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(stream, obj);
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
