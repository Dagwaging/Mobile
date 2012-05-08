using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#if WINDOWS_PHONE
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
#else
using Microsoft.Maps.MapControl;
#endif


namespace Rhit.Applications.Models {
    [DataContract]
    public class ServerObject {
        #region OffCampusResponse
        [DataMember(Name = "LocationIds")]
        public List<int> LocationIds { get; set; }
        #endregion

        #region TagsResponse
        [DataMember(Name = "TagsRoot")]
        public TagsCategory_DC TagsRoot { get; set; }
        #endregion

        #region CampusServicesResponse
        [DataMember(Name = "ServicesRoot")]
        public CampusServicesCategory_DC CampusServicesRoot { get; set; }
        #endregion

        #region VersionResponse
        [DataMember(Name = "LocationsVersion")]
        public double LocationsVersion { get; set; }

        [DataMember(Name = "ServicesVersion")]
        public double ServicesVersion { get; set; }

        [DataMember(Name = "TagsVersion")]
        public double TagsVersion { get; set; }
        #endregion

        #region MessageResponse
        [DataMember(Name = "Message")]
        public string Message { get; set; }
        #endregion

        #region LocationsResponse
        [DataMember(Name = "Locations")]
        public List<Location_DC> Locations { get; set; }

        [DataMember(Name = "Version")]
        public double Version { get; set; }
        #endregion

        #region LocationNamesResponse
        [DataMember(Name = "Names")]
        public List<Location_DC> Names { get; set; }

        //Also includes Version (from LocationsResponse)
        #endregion

        #region DescriptionResponse
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "Links")]
        public List<Link_DC> Links { get; set; }
        #endregion

        #region DirectionsResponse
        [DataMember(Name = "Done")]
        public int Done { get; set; }

        [DataMember(Name = "RequestId")]
        public int RequestId { get; set; }

        [DataMember(Name = "Result")]
        public Directions_DC Result { get; set; }
        #endregion

        #region PrinterResponse
        //Also includes DirectionsResponse

        [DataMember(Name = "Printer")]
        public string Printer { get; set; }
        #endregion

        #region AuthenticationResponse
        [DataMember(Name = "Expiration")]
        public DateTime Expiration { get; set; }

        [DataMember(Name = "Token")]
        public string Token { get; set; }
        #endregion

        #region StoredProcedureResponse
        [DataMember(Name = "Columns")]
        public List<string> Columns { get; set; }

        [DataMember(Name = "Table")]
        public List<List<string>> Table { get; set; }
        #endregion

        #region PathDataResponse
        [DataMember(Name = "Directions")]
        public List<Direction_DC> Directions { get; set; }

        [DataMember(Name = "Messages")]
        public List<DirectionMessage_DC> Messages { get; set; }


        [DataMember(Name = "Nodes")]
        public List<Node_DC> Nodes { get; set; }


        [DataMember(Name = "Partitions")]
        public List<Partition_DC> Partitions { get; set; }


        [DataMember(Name = "Paths")]
        public List<Path_DC> Paths { get; set; }
        #endregion

        [DataMember(Name = "Folders")]
        public List<string> Folders { get; set; }



        public static Node_DC ParseNode(ServerObject instance) {
            //""Columns\":[\"id\",\"lat\",\"lon\",\"altitude\",\"outside\",\"location\"],\
            //"Table\":[[\"1083\",\"39.4840212214018\",\"-87.3248627936401\",\"0\",\"True\",null]]}"
            return new Node_DC() {
                Id=int.Parse(instance.Table[0][0]),
                Latitude = double.Parse(instance.Table[0][1]),
                Longitude = double.Parse(instance.Table[0][2]),
                Altitude = double.Parse(instance.Table[0][3]),
                Outside = bool.Parse(instance.Table[0][4]),
                //Location = int.Parse(instance.Table[0][5]),
            };
        }

        public static List<LocationData> GetLocations(List<Location_DC> locations) {
            if(locations == null) return null;
            List<LocationData> _locations = new List<LocationData>();
            foreach(Location_DC location in locations)
                _locations.Add(location.ToRhitLocation());
            return _locations;
        }

        internal static Path_DC ParsePath(ServerObject instance) {
            return new Path_DC() {
                Id = int.Parse(instance.Table[0][0]),
                Node1 = int.Parse(instance.Table[0][1]),
                Node2 = int.Parse(instance.Table[0][2]),
                Stairs = int.Parse(instance.Table[0][3]),
                Elevator = bool.Parse(instance.Table[0][4]),
                Partition = int.Parse(instance.Table[0][5]),
            };
        }

        internal static DirectionMessage_DC ParseDirectionMessage(ServerObject instance) {
            return new DirectionMessage_DC() {
                Id = int.Parse(instance.Table[0][0]),
                Message1 = instance.Table[0][1],
                Message2 = instance.Table[0][2],
                Action1 = instance.Table[0][3],
                Action2 = instance.Table[0][4],
                Offset = int.Parse(instance.Table[0][5]),
            };
        }

        internal static Direction_DC ParseDirection(ServerObject instance) {
            return new Direction_DC() {
                Id = int.Parse(instance.Table[0][0]),
                MessageId = int.Parse(instance.Table[0][1]),
                Within = int.Parse(instance.Table[0][3]),
            };
        }
    }

    #region TagsCategory - Data Contract
    [DataContract]
    public class TagsCategory_DC {
        public TagsCategory_DC() : base() { }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Children")]
        public List<TagsCategory_DC> Children { get; set; }

        [DataMember(Name = "Tags")]
        public List<Tag_DC> Tags { get; set; }
    }
    #endregion

    #region Tag - Data Contract
    [DataContract]
    public class Tag_DC {
        public Tag_DC() : base() { }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "IsDefault")]
        public bool IsDefault { get; set; }
    }
    #endregion

    #region CampusServicesCategory - Data Contract
    [DataContract]
    public class CampusServicesCategory_DC {
        public CampusServicesCategory_DC() : base() { }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Children")]
        public List<CampusServicesCategory_DC> Children { get; set; }

        [DataMember(Name = "Links")]
        public List<Link_DC> Links { get; set; }
    }
    #endregion

    #region Location - Data Contract
    [DataContract]
    public class Location_DC {
        [DataMember(Name = "AltNames")]
        public List<string> AltNames { get; set; }

        [DataMember(Name = "Center")]
        public GeoCoordinate_DC Center { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "Floor")]
        public int Floor { get; set; }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "Links")]
        public List<Link_DC> Links { get; set; }

        [DataMember(Name = "MapArea")]
        public LocationData_DC LocationData { get; set; }

        [DataMember(Name = "Name")]
        public string Label { get; set; }

        [DataMember(Name = "Parent")]
        public int ParentId { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }

        public LocationData ToRhitLocation() {
            LocationCollection locations = new LocationCollection();
            if(LocationData != null && LocationData.Locations != null) {
                foreach(GeoCoordinate_DC coordinate in LocationData.Locations)
                    locations.Add(coordinate.ToGeoCoordinate());
            }

            LocationData location = new LocationData() {
                Center = Center.ToGeoCoordinate(),
                Corners = locations,
                Id = Id,
                Description = Description,
                Label = Label,
                Type = ConvertTypeKeyToType(Type),
                AltNames = AltNames,
                ParentId = ParentId,
                Floor = Floor,
            };

            foreach(ILink link in Links)
                location.Links.Add(link);

            if(LocationData != null) {
                location.MinZoomLevel = LocationData.MinZoomLevel;
                location.LabelOnHybrid = LocationData.LabelOnHybrid;
            }
            return location;
        }

        private static Dictionary<string, LocationType> _locationTypes = new Dictionary<string, LocationType>() {
            { "NL", LocationType.NormalLocation },
            { "PI", LocationType.PointOfInterest },
            { "QL", LocationType.OnQuickList },
            { "MB", LocationType.MenRestroom },
            { "WB", LocationType.WomenRestroom },
            { "UB", LocationType.UnisexRestroom },
            { "PR", LocationType.Printer },
        };
        private static LocationType ConvertTypeKeyToType(string key) {
            return _locationTypes[key];
        }

        public static string ConvertTypeToTypeKey(LocationType type) {
            foreach (var kvp in _locationTypes)
                if (kvp.Value == type) return kvp.Key;
            return null;
        }
    }
    #endregion

    #region Location Data - Data Contract
    [DataContract]
    public class LocationData_DC {
        public LocationData_DC() : base() { }

        [DataMember(Name = "Corners")]
        public List<GeoCoordinate_DC> Locations { get; set; }

        [DataMember(Name = "MinZoomLevel")]
        public int MinZoomLevel { get; set; }

        [DataMember(Name = "LabelOnHybrid")]
        public bool LabelOnHybrid { get; set; }
    }
    #endregion

    #region Link - Data Contract
    [DataContract]
    public class Link_DC : ILink {
        public Link_DC() : base() { }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Url")]
        public string Address { get; set; }
    }
    #endregion

    #region Directions - Data Contract
    [DataContract]
    public class Directions_DC {
        public Directions_DC() : base() { }

        [DataMember(Name = "Dist")]
        public double Distance { get; set; }

        [DataMember(Name = "Paths")]
        public List<DirectionPath_DC> Paths { get; set; }

        [DataMember(Name = "StairsDown")]
        public int StairsDown { get; set; }

        [DataMember(Name = "StairsUp")]
        public int StairsUp { get; set; }
    }
    #endregion

    #region DirectionPath - Data Contract
    [DataContract]
    public class DirectionPath_DC {
        public DirectionPath_DC() : base() {
            Action = "";
        }

        [DataMember(Name = "Action")]
        public string Action { get; set; }

        [DataMember(Name = "Dir")]
        public string Direction { get; set; }

        [DataMember(Name = "Flag")]
        public bool Flag { get; set; }

        [DataMember(Name = "Outside")]
        public bool Outside { get; set; }

        [DataMember(Name = "Lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "Lon")]
        public double Longitude { get; set; }

        [DataMember(Name = "Location")]
        public int Location { get; set; }
    }
    #endregion

    #region GeoCoordinate - Data Contract
    [DataContract]
    public class GeoCoordinate_DC {
        public GeoCoordinate_DC() : base() { }

        [DataMember(Name = "Lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "Lon")]
        public double Longitude { get; set; }

        public GeoCoordinate ToGeoCoordinate() {
            return new GeoCoordinate() {
                Latitude = Latitude,
                Longitude = Longitude,
            };
        }
    }
    #endregion

    #region Direction - Data Contract
    [DataContract]
    public class Direction_DC {
        public Direction_DC() : base() { }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "Message")]
        public int MessageId { get; set; }

        [DataMember(Name = "Paths")]
        public List<int> Paths { get; set; }

        [DataMember(Name = "Within")]
        public int Within { get; set; }
    }
    #endregion

    #region DirectionMessage - Data Contract
    [DataContract]
    public class DirectionMessage_DC {
        public DirectionMessage_DC() : base() { }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "NodeOffset")]
        public int Offset { get; set; }

        [DataMember(Name = "Message1")]
        public string Message1 { get; set; }

        [DataMember(Name = "Message2")]
        public string Message2 { get; set; }

        [DataMember(Name = "Action1")]
        public string Action1 { get; set; }

        [DataMember(Name = "Action2")]
        public string Action2 { get; set; }
    }
    #endregion

    #region Node - Data Contract
    [DataContract]
    public class Node_DC {
        public Node_DC() : base() { }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "Lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "Lon")]
        public double Longitude { get; set; }

        [DataMember(Name = "Altitude")]
        public double Altitude { get; set; }

        [DataMember(Name = "Location")]
        public int Location { get; set; }

        [DataMember(Name = "Outside")]
        public bool Outside { get; set; }
    }
    #endregion

    #region Partition - Data Contract
    [DataContract]
    public class Partition_DC {
        public Partition_DC() : base() { }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }
    }
    #endregion

    #region Path - Data Contract
    [DataContract]
    public class Path_DC {
        public Path_DC() : base() { }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "Node1")]
        public int Node1 { get; set; }

        [DataMember(Name = "Node2")]
        public int Node2 { get; set; }

        [DataMember(Name = "Partition")]
        public int Partition { get; set; }

        [DataMember(Name = "Stairs")]
        public int Stairs { get; set; }

        [DataMember(Name = "Elevator")]
        public bool Elevator { get; set; }
    }
    #endregion
}