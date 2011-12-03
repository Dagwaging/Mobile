using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Maps.MapControl;

namespace Rhit.Admin.Model {
    [DataContract]
    public class ServerObject {
        [DataMember(Name = "Version")]
        public double Version { get; set; }

        [DataMember(Name = "Locations")]
        public List<Location_DC> Locations { get; set; }

        [DataMember(Name = "Names")]
        public List<Location_DC> Names { get; set; }

        public static List<RhitLocation> GetLocations(List<Location_DC> locations) {
            List<RhitLocation> _locations = new List<RhitLocation>();
            foreach(Location_DC location in locations)
                _locations.Add(location.ToRhitLocation());
            return _locations;
        }
    }

    [DataContract]
    public class Location_DC {
        [DataMember(Name = "Name")]
        public string Label { get; set; }

        [DataMember(Name = "AltNames")]
        public List<string> AltNames { get; set; }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "IsDepartable")]
        public bool IsDepartable { get; set; }

        [DataMember(Name = "Center")]
        public Coordinate_DC Center { get; set; }

        [DataMember(Name = "IsPOI")]
        public bool IsPOI { get; set; }

        [DataMember(Name = "Links")]
        public List<Link_DC> Links { get; set; }

        [DataMember(Name = "Parent")]
        public int ParentId { get; set; }

        [DataMember(Name = "OnQuickList")]
        public bool OnQuickList { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "MapArea")]
        public LocationData_DC LocationData { get; set; }

        public RhitLocation ToRhitLocation() {
            LocationCollection locations = new LocationCollection();
            if(LocationData != null && LocationData.Locations != null) {
                foreach(Coordinate_DC coordinate in LocationData.Locations)
                    locations.Add(coordinate.ToLocation());
            }
            Dictionary<string, string> links = new Dictionary<string,string>();
            foreach(Link_DC link in Links)
                links[link.Name] = link.Url;

            RhitLocation location = new RhitLocation() {
                Center = Center.ToLocation(),
                Locations = locations,
                Id = Id,
                Description = Description,
                Label = Label,
                IsPOI = IsPOI,
                OnQuikList = OnQuickList,
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

        [OnDeserializing]
        public void SetDefaults(StreamingContext c) {
            ParentId = -1;
        }
    }

    [DataContract]
    public class LocationData_DC {
        public LocationData_DC() : base() { }

        [DataMember(Name = "Corners")]
        public List<Coordinate_DC> Locations { get; set; }

        [DataMember(Name = "MinZoomLevel")]
        public int MinZoomLevel { get; set; }

        [DataMember(Name = "LabelOnHybrid")]
        public bool LabelOnHybrid { get; set; }
    }

    [DataContract]
    public class Link_DC {
        public Link_DC() : base() { }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Url")]
        public string Url { get; set; }
    }

    [DataContract]
    public class Coordinate_DC {
        public Coordinate_DC() : base() { }

        [DataMember(Name = "Lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "Long")]
        public double Longitude { get; set; }

        public Location ToLocation() {
            return new Location() {
                Latitude = Latitude,
                Longitude = Longitude,
            };
        }
    }
}