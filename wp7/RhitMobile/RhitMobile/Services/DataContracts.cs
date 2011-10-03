using System.Collections.Generic;
using System.Device.Location;
using System.Runtime.Serialization;
using Microsoft.Phone.Controls.Maps;
using RhitMobile.ObjectModel;

namespace RhitMobile.Services {
    [DataContract]
    public class ServerObject {
        [DataMember(Name = "Version")]
        public double Version { get; set; }

        [DataMember(Name = "Areas")]
        public List<RhitLocation_DC> Areas { get; set; }

        public List<RhitLocation> GetLocations() {
            List<RhitLocation> locations = new List<RhitLocation>();
            foreach(RhitLocation_DC location in Areas)
                locations.Add(location.ToRhitLocation());
            return locations;
        }
    }

    [DataContract]
    public class RhitLocation_DC {

        [DataMember(Name = "Name")]
        public string Label { get; set; }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "Center")]
        public GeoCoordinate_DC Center { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "Corners")]
        public List<GeoCoordinate_DC> Locations { get; set; }

        [DataMember(Name = "MinZoomLevel")]
        public int MinZoomLevel { get; set; }

        [DataMember(Name = "LabelOnHybrid")]
        public bool LabelOnHybrid { get; set; }

        public RhitLocation ToRhitLocation() {
            LocationCollection locations = new LocationCollection();
            foreach(GeoCoordinate_DC coordinate in Locations)
                locations.Add(coordinate.ToGeoCoordinate());

            return new RhitLocation() {
                Center = Center.ToGeoCoordinate(),
                Locations = locations,
                Id = Id,
                Description = Description,
                Label = Label,
                MinZoomLevel = MinZoomLevel,
                LabelOnHybrid = LabelOnHybrid,
            };
        }
    }

    [DataContract]
    public class GeoCoordinate_DC {

        public GeoCoordinate_DC() : base() { }

        [DataMember(Name = "Lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "Long")]
        public double Longitude { get; set; }

        public GeoCoordinate ToGeoCoordinate() {
            return new GeoCoordinate() {
                Latitude = Latitude,
                Longitude = Longitude,
            };
        }
    }
}