#if WINDOWS_PHONE
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
#else
using Microsoft.Maps.MapControl;
#endif

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Rhit.Applications.Model {
    [DataContract]
    public class ServerObject {
        #region CampusServicesResponse
        [DataMember(Name = "Root")]
        public List<CampusServicesCategory_DC> CampusServicesRoot { get; set; }
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

        public static List<RhitLocation> GetLocations(List<Location_DC> locations) {
            if(locations == null) return null;
            List<RhitLocation> _locations = new List<RhitLocation>();
            foreach(Location_DC location in locations)
                _locations.Add(location.ToRhitLocation());
            return _locations;
        }
    }

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

        public RhitLocation ToRhitLocation() {
            LocationCollection locations = new LocationCollection();
            if(LocationData != null && LocationData.Locations != null) {
                foreach(GeoCoordinate_DC coordinate in LocationData.Locations)
                    locations.Add(coordinate.ToGeoCoordinate());
            }

            RhitLocation location = new RhitLocation() {
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
        public static Dictionary<string, string> ActionCodeDict = new Dictionary<string, string>() {
            {"GS", "Go Straight"}, {"CS", "Cross the Street"}, {"FP", "Follow the Path"},
            {"L1", "Slight Left"}, {"R1", "Slight Right"}, {"L2", "Trun Left"},
            {"R2", "Turn Right"}, {"L3", "Sharp Left"}, {"R3", "Sharp Right"},
            {"EN", "Enter the Building"}, {"EX", "Exit the Building"},
            {"US", "Go Up the Stairs"}, {"DS", "Go Down the Stairs"}, {"", ""},
        };

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

        public string ConvertAction() {
            return ActionCodeDict[Action];
        }
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
        public int Message { get; set; }

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