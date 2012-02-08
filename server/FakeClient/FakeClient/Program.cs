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
        }

        public List<TimeSpan> Times { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tracker = new Tracker();
            var numExec = 1000;

            var tasks = new List<Task>();
            for (int i = 0; i < numExec; i++) {
                tasks.Add(Task.Factory.StartNew(new Action<object>((t) => {
                    var start = DateTime.Now;
                    var request = HttpWebRequest.Create("http://localhost:5600/locations/data/all");
                    request.GetResponse();
                    ((Tracker)t).Times.Add(DateTime.Now - start);
                }), tracker));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine("Average Time: " + tracker.Times.Sum(ts => ts.TotalSeconds) / numExec + " seconds");

            Console.ReadLine();

            /*var stream = client.GetStream();
            var encoding = new ASCIIEncoding();
            var message = new ClientRequest() { Request = 400, MyVersion = 0.0 };
            byte[] request = encoding.GetBytes(message.Serialize());
            stream.Write(request, 0, request.Length);
            stream.WriteByte(0);

            byte[] response = new byte[1000];
            while (true)
            {
                int bytesRead = stream.Read(response, 0, 1000);
                if (bytesRead == 0)
                    break;
                Console.WriteLine(encoding.GetString(response, 0, bytesRead));
            }

            client.Close();*/
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
