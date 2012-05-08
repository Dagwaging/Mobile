using System.Collections.Generic;
using Rhit.Applications.Extentions.Maps.Sources;

#if WINDOWS_PHONE
using System.Device.Location;
using Microsoft.Phone.Controls.Maps.Core;
using Microsoft.Phone.Controls.Maps;
#else
using Microsoft.Maps.MapControl.Core;
using Microsoft.Maps.MapControl;
#endif


namespace Rhit.Applications.Extentions.Maps {
    public class RhitMapMode : MercatorMode {

        // The latitude value range (From = bottom most latitude, To = top most latitude)
        protected static Range<double> validLatitudeRange = new Range<double>(39.479665, 39.486985);
        // The longitude value range (From = left most longitude, To = right most longitude)
        protected static Range<double> validLongitudeRange = new Range<double>(-87.333154, -87.314100);
        // Restricts the map view.

#if WINDOWS_PHONE
        protected override Range<double> GetZoomRange(GeoCoordinate center) {
#else
        protected override Range<double> GetZoomRange(Location center) {
#endif
            // The allowable zoom levels - 14 to 25.
            return new Range<double>(14, 25);
        }

        // This method is called when the map view changes on Keyboard and Navigation Bar events.

#if WINDOWS_PHONE
        public override bool ConstrainView(GeoCoordinate center, ref double zoomLevel, ref double heading, ref double pitch) {
#else
        public override bool ConstrainView(Location center, ref double zoomLevel, ref double heading, ref double pitch) {
#endif
            bool isChanged = base.ConstrainView(center, ref zoomLevel, ref heading, ref pitch);

            double newLatitude = center.Latitude;
            double newLongitude = center.Longitude;

            // If the map view is outside the valid longitude range,
            // move the map back within range.
            if(center.Longitude > validLongitudeRange.To) {
                newLongitude = validLongitudeRange.To;
            } else if(center.Longitude < validLongitudeRange.From) {
                newLongitude = validLongitudeRange.From;
            }

            // If the map view is outside the valid latitude range,
            // move the map back within range.
            if(center.Latitude > validLatitudeRange.To) {
                newLatitude = validLatitudeRange.To;
            } else if(center.Latitude < validLatitudeRange.From) {
                newLatitude = validLatitudeRange.From;
            }

            // The new map view location.
            if(newLatitude != center.Latitude || newLongitude != center.Longitude) {
                center.Latitude = newLatitude;
                center.Longitude = newLongitude;
                isChanged = true;
            }

            // The new zoom level.
            Range<double> range = GetZoomRange(center);
            if(zoomLevel > range.To) {
                zoomLevel = range.To;
                isChanged = true;
            } else if(zoomLevel < range.From) {
                zoomLevel = range.From;
                isChanged = true;
            }

            return isChanged;
        }
    }
}
