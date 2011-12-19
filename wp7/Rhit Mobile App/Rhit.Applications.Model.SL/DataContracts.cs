#if WINDOWS_PHONE
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
#else
using Microsoft.Maps.MapControl.Core;
using Microsoft.Maps.MapControl;
#endif

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Rhit.Applications.Model {
    [DataContract]
    public class ServerObject {
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

        public static List<RhitLocation> GetLocations(List<Location_DC> locations) {
            if(locations == null) return null;
            List<RhitLocation> _locations = new List<RhitLocation>();
            foreach(Location_DC location in locations)
                _locations.Add(location.ToRhitLocation());
            return _locations;
        }
    }

    #region Location - Data Contract
    [DataContract]
    public class Location_DC {
        [DataMember(Name = "AltNames")]
        public List<string> AltNames { get; set; }

        [DataMember(Name = "Center")]
        public GeoCoordinate_DC Center { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "IsDepartable")]
        public bool IsDepartable { get; set; }

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
            Dictionary<string, string> links = new Dictionary<string, string>();
            if (Links != null)
                foreach(Link_DC link in Links)
                    links[link.Name] = link.Url;

            RhitLocation location = new RhitLocation() {
                Center = Center.ToGeoCoordinate(),
                Locations = locations,
                Id = Id,
                Description = Description,
                Label = Label,
                Type = ConvertTypeKeyToType(Type),
                Links = links,
                IsDepartable = IsDepartable,
                AltNames = AltNames,
                ParentId = ParentId,
            };
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

        public static string ConvertTypeTypeToKey(LocationType type) {
            foreach (var kvp in _locationTypes) {
                if (kvp.Value == type) {
                    return kvp.Key;
                }
            }
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
    public class Link_DC {
        public Link_DC() : base() { }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Url")]
        public string Url { get; set; }
    }
    #endregion

    #region Directions - Data Contract
    [DataContract]
    public class Directions_DC {
        public Directions_DC() : base() { }

        [DataMember(Name = "Dist")]
        public double Distance { get; set; }

        [DataMember(Name = "Paths")]
        public List<Path_DC> Paths { get; set; }

        [DataMember(Name = "StairsDown")]
        public int StairsDown { get; set; }

        [DataMember(Name = "StairsUp")]
        public int StairsUp { get; set; }

        [DataMember(Name = "Start")]
        public GeoCoordinate_DC Start { get; set; }
    }
    #endregion

    #region Path - Data Contract
    [DataContract]
    public class Path_DC {
        public Path_DC() : base() { }

        [DataMember(Name = "Dir")]
        public string Direction { get; set; }

        [DataMember(Name = "To")]
        public GeoCoordinate_DC To { get; set; }

        [DataMember(Name = "Flag")]
        public bool Flag { get; set; }
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
}