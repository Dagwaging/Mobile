using System;

namespace Rhit.Applications.Extentions.Maps.Sources {
    public class RoseOverlay : BaseTileSource {
        /// <summary>
        /// Constructor; Defines the base uri format for the tile source
        /// </summary>
        public RoseOverlay() {
            Label = "Floor Plans";
            UriFormat = "http://mobilewin.csse.rose-hulman.edu:5600/download/White Chapel/{0}.png";
        }

        /// <summary>
        /// Generate the uri for a specific tile.
        /// </summary>
        /// <param name="x">Longitude coordinate in request area</param>
        /// <param name="y">Latitude coordinate in request area</param>
        /// <param name="zoomLevel">Zoom level of map</param>
        /// <returns>The uri for the tile for the region specified</returns>
        public override Uri GetUri(int x, int y, int zoomLevel) {
            if(zoomLevel <= 0) return null;
            return new Uri(string.Format(UriFormat, TileToQuadKey(x, y, zoomLevel)));
        }
    }
}
