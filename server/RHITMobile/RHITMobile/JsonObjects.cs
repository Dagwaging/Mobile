using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Data;

namespace RHITMobile
{
    [DataContract]
    public abstract class JsonObject { }

    [DataContract]
    public class ClientRequest : JsonObject
    {
        [DataMember(IsRequired = true)]
        public int Request { get; set; }
        [DataMember(IsRequired = false)]
        public double MyVersion { get; set; }
        [DataMember(IsRequired = false)]
        public double Latitude { get; set; }
        [DataMember(IsRequired = false)]
        public double Longitude { get; set; }
        [DataMember(IsRequired = false)]
        public int NearbyLocation { get; set; }
        [DataMember(IsRequired = false)]
        public int Destination { get; set; }
        [DataMember(IsRequired = false)]
        public double StairsMultiplier { get; set; }
        [DataMember(IsRequired = false)]
        public double OutdoorsMultiplier { get; set; }
        [DataMember(IsRequired = false)]
        public bool CanUseElevator { get; set; }
    }

    [DataContract]
    public class MessageResponse : JsonObject
    {
        public MessageResponse(string message, params object[] objects)
        {
            Message = String.Format(message, objects);
        }

        [DataMember(IsRequired = true)]
        public string Message { get; set; }
    }

    [DataContract]
    public class MapAreasResponse : JsonObject
    {
        public MapAreasResponse(double version)
        {
            Version = version;
            Areas = new List<MapArea>();
        }

        [DataMember(IsRequired = true)]
        public List<MapArea> Areas { get; set; }
        [DataMember(IsRequired = true)]
        public double Version { get; set; }
    }

    [DataContract]
    public class MapArea : JsonObject
    {
        public MapArea(DataRow row)
        {
            Id = (int)row["id"];
            Name = (string)row["name"];
            Description = (string)row["description"];
            Corners = new List<LatLong>();
        }

        [DataMember(IsRequired = true)]
        public int Id { get; set; }
        [DataMember(IsRequired = true)]
        public string Name { get; set; }
        [DataMember(IsRequired = true)]
        public string Description { get; set; }
        [DataMember(IsRequired = true)]
        public List<LatLong> Corners { get; set; }
    }

    [DataContract]
    public class LatLong : JsonObject
    {
        public LatLong(DataRow row)
        {
            Lat = (double)row["lat"];
            Long = (double)row["lon"];
        }

        [DataMember(IsRequired = true)]
        public double Lat { get; set; }
        [DataMember(IsRequired = true)]
        public double Long { get; set; }
    }

    public static class JsonUtility
    {
        public static T Deserialize<T>(this string json)
            where T : JsonObject
        {
            MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }

        public static string Serialize<T>(this T obj)
            where T : JsonObject
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            serializer.WriteObject(stream, obj);
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
