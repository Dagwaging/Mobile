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
            AltNames = new List<string>();
            Center = new LatLong(row);
            Description = hideDescs ? null : (string)row["description"];
            Links = hideDescs ? null : new List<HyperLink>();
            Id = (int)row["id"];
            IsDepartable = !row.IsNull("departnode");
            MapArea = row.IsNull("labelonhybrid") ? null : new MapAreaData(row);
            Name = (string)row["name"];
            Parent = row.IsNull("parent") ? null : (int?)row["parent"];
            Type = (string)row["type"];

            IsPOI = (bool)row["ispoi"];
            OnQuickList = (bool)row["onquicklist"];

            HasAltNames = (bool)row["hasalts"];
            HasLinks = (bool)row["haslinks"];
        }

        [DataMember]
        public List<string> AltNames { get; set; }
        [DataMember]
        public LatLong Center { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public bool IsDepartable { get; set; }
        [DataMember]
        public List<HyperLink> Links { get; set; }
        [DataMember]
        public MapAreaData MapArea { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int? Parent { get; set; }
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public bool IsPOI { get; set; }
        [DataMember]
        public bool OnQuickList { get; set; }

        public bool HasAltNames { get; set; }
        public bool HasLinks { get; set; }

        public bool IsMapArea()
        {
            return MapArea != null;
        }

        public void AddAltNames(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                AltNames.Add((string)row["name"]);
            }
        }

        public void AddLinks(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                Links.Add(new HyperLink(row));
            }
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

    [DataContract]
    public class HyperLink : JsonObject
    {
        public HyperLink(DataRow row)
        {
            Name = (string)row["name"];
            Url = (string)row["url"];
        }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Url { get; set; }
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
            Links = new List<HyperLink>();
        }

        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public List<HyperLink> Links { get; set; }
    }
    #endregion

    #region DirectionsResponse & PrinterResponse
    [DataContract]
    public class DirectionsResponse : JsonObject
    {
        public DirectionsResponse(int done, int requestId)
        {
            Done = done;
            RequestId = requestId;
            Result = null;
        }

        [DataMember]
        public int Done { get; set; }
        [DataMember]
        public int RequestId { get; set; }
        [DataMember]
        public Directions Result { get; set; }
    }

    [DataContract]
    public class PrinterResponse : DirectionsResponse
    {
        public PrinterResponse(int done, int requestId, string printer)
            : base(done, requestId)
        {
            Printer = printer;
        }

        [DataMember]
        public string Printer { get; set; }
    }

    [DataContract]
    public class Directions : JsonObject
    {
        public Directions(LatLong start, List<Path> paths)
        {
            Dist = paths.Sum(path => path.HDist*path.HDist + path.VDist*path.VDist);
            Paths = paths;
            StairsDown = -paths.Sum(path => Math.Min(path.Stairs, 0));
            StairsUp = paths.Sum(path => Math.Max(path.Stairs, 0));
            Start = start;
        }

        [DataMember]
        public double Dist { get; set; }
        [DataMember]
        public List<Path> Paths { get; set; }
        [DataMember]
        public int StairsDown { get; set; }
        [DataMember]
        public int StairsUp { get; set; }
        [DataMember]
        public LatLong Start { get; set; }
    }

    [DataContract]
    public class Path : JsonObject
    {
        public Path(DataRow row, Node prevNode, DirectionsSettings settings)
        {
            Id = (int)row["pathid"];
            Forward = (bool)row["forward"];
            Stairs = (int)row["stairs"] * (Forward ? 1 : -1);
            Elev = (bool)row["elevator"];
            ToNode = new Node(row);
            FromNode = prevNode.Id;

            HDist = prevNode.HDistanceTo(ToNode);
            VDist = ToNode.Alt - prevNode.Alt;
            Outside = (prevNode.Outside ? 0.5 : 0) + (ToNode.Outside ? 0.5 : 0);

            WeightedDist = settings.WeightedDist(this);
        }

        [DataMember]
        public string Dir { get; set; }
        [DataMember]
        public LatLong To { get { return ToNode.Pos; } set { } }

        public int Id { get; set; }
        public int Stairs { get; set; }
        public bool Elev { get; set; }
        public bool Forward { get; set; }
        public Node ToNode { get; set; }
        public int FromNode { get; set; }

        public double HDist { get; set; }
        public double VDist { get; set; }
        public double Outside { get; set; }

        public double WeightedDist { get; set; }
    }

    public class Node
    {
        public Node(DataRow row)
        {
            Id = (int)row["id"];
            Pos = new LatLong(row);
            Alt = (double)row["altitude"];
            Outside = (bool)row["outside"];
            Location = row.IsNull("location") ? null : (int?)row["location"];
            Partition = row.Table.Columns.Contains("partition") ? (int?)row["partition"] : null;
        }

        public int Id { get; set; }
        public LatLong Pos { get; set; }
        public double Alt { get; set; }
        public bool Outside { get; set; }
        public int? Location { get; set; }
        public int? Partition { get; set; }

        public double HDistanceTo(Node node)
        {
            var x = (node.Pos.Lon - this.Pos.Lon) * Program.DegToRad * Math.Cos((node.Pos.Lat + this.Pos.Lat) * Program.DegToRad / 2);
            var y = (node.Pos.Lat - this.Pos.Lat) * Program.DegToRad;
            return Math.Sqrt(x * x + y * y) * Program.EarthRadius;
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
            Lon = (double)row["lon"];
        }

        [DataMember]
        public double Lat { get; set; }
        [DataMember]
        public double Lon { get; set; }
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
