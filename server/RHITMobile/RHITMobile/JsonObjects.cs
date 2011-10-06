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

    #region MessageResponse
    [DataContract]
    public class MessageResponse : JsonObject
    {
        public MessageResponse(string message, params object[] objects)
        {
            Message = String.Format(message, objects);
        }

        [DataMember]
        public string Message { get; set; }
    }
    #endregion

    #region MapAreasResponse
    [DataContract]
    public class MapAreasResponse : JsonObject
    {
        public MapAreasResponse(double version)
        {
            Version = version;
            Areas = new List<MapArea>();
        }

        [DataMember]
        public List<MapArea> Areas { get; set; }
        [DataMember]
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
            LabelOnHybrid = (bool)row["labelonhybrid"];
            MinZoomLevel = (int)row["minzoomlevel"];
            Center = new LatLong(row);
            Corners = new List<LatLong>();
        }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public LatLong Center { get; set; }
        [DataMember]
        public List<LatLong> Corners { get; set; }
        [DataMember]
        public bool LabelOnHybrid { get; set; }
        [DataMember]
        public int MinZoomLevel { get; set; }
    }
    #endregion

    #region LocationsResponse
    [DataContract]
    public class LocationsResponse : JsonObject
    {
        public LocationsResponse(double version)
        {
            Version = version;
            Locations = new List<Location>();
        }

        [DataMember]
        public List<Location> Locations { get; set; }
        [DataMember]
        public double Version { get; set; }
    }

    [DataContract]
    public class Location : JsonObject
    {
        public Location(DataRow row, bool hideDescs)
        {
            Center = new LatLong(row);
            Description = hideDescs ? null : (string)row["description"];
            Id = (int)row["id"];
            IsPOI = (bool)row["ispoi"];
            MapArea = row.IsNull("labelonhybrid") ? null : new MapAreaData(row);
            Name = (string)row["name"];
            OnQuickList = (bool)row["onquicklist"];
            Parent = row.IsNull("parent") ? null : (int?)row["parent"];
        }

        [DataMember]
        public LatLong Center { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public bool IsPOI { get; set; }
        [DataMember]
        public MapAreaData MapArea { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool OnQuickList { get; set; }
        [DataMember]
        public int? Parent { get; set; }

        public bool IsMapArea()
        {
            return MapArea != null;
        }
    }

    [DataContract]
    public class MapAreaData : JsonObject
    {
        public MapAreaData(DataRow row)
        {
            Corners = new List<LatLong>();
            LabelOnHybrid = (bool)row["labelonhybrid"];
            MinZoomLevel = (int)row["minzoomlevel"];
        }

        [DataMember]
        public List<LatLong> Corners { get; set; }
        [DataMember]
        public bool LabelOnHybrid { get; set; }
        [DataMember]
        public int MinZoomLevel { get; set; }
    }
    #endregion

    #region LocationNamesResponse
    [DataContract]
    public class LocationNamesResponse : JsonObject
    {
        public LocationNamesResponse(double version)
        {
            Names = new List<LocationName>();
            Version = version;
        }

        [DataMember]
        public List<LocationName> Names { get; set; }
        [DataMember]
        public double Version { get; set; }
    }

    [DataContract]
    public class LocationName : JsonObject
    {
        public LocationName(DataRow row)
        {
            Id = (int)row["id"];
            Name = (string)row["name"];
        }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
    #endregion

    #region LocationDescResponse
    [DataContract]
    public class LocationDescResponse : JsonObject
    {
        public LocationDescResponse(DataRow row)
        {
            Description = (string)row["description"];
        }

        [DataMember]
        public string Description { get; set; }
    }
    #endregion

    #region DirectionsResponse
    [DataContract]
    public class DirectionsResponse : JsonObject
    {
        [DataMember]
        public int Done { get; set; }
        [DataMember]
        public int RequestId { get; set; }
        [DataMember]
        public string Message { get; set; }

        public DirectionsResponse(int done, int requestId, string message)
        {
            Done = done;
            RequestId = requestId;
            Message = message;
        }
    }
    #endregion

    #region Miscellaneous
    [DataContract]
    public class LatLong : JsonObject
    {
        public LatLong(DataRow row)
        {
            Lat = (double)row["lat"];
            Long = (double)row["lon"];
        }

        [DataMember]
        public double Lat { get; set; }
        [DataMember]
        public double Long { get; set; }
    }
    #endregion

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
