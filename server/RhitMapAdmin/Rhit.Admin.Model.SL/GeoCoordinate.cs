using Microsoft.Maps.MapControl;

namespace Rhit.Admin.Model {
    public class GeoCoordinate : Location {
        public GeoCoordinate() : base() { }
        public GeoCoordinate(double latitude, double longitude) : base(latitude, longitude) { }
        public GeoCoordinate(double latitude, double longitude, double altitude) : base(latitude, longitude, altitude) { }
    }
}
