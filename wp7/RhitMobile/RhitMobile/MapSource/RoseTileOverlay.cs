using System;

namespace RhitMobile.MapSource {
    /// <summary>
    /// Tile source for the floor plans of Rose-Hulman.
    /// </summary>
    public class RoseTileOverlay : BaseTileSource {
        /// \ingroup tile_sources
        /// <summary>
        /// Constructor; Defines the base uri format for the tile source
        /// </summary>
        public RoseTileOverlay() : base() {
            UriFormat = "http://dl.dropbox.com/u/26625851/{0}.png";
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
