using System;

namespace RhitMobile.MapSource {
    /// \ingroup tile_sources
    /// <summary>
    /// A tile source generated as a branch from the open source OpenStreetMap dataset.
    /// The tile source comes from http://openstreet.org/.
    /// </summary>
    public class OsmaRender : BaseTileSource {
        #region Private Fields
        /// <summary>
        /// Used to define which region the tile is in.
        /// Osma splits tile locations into a function of latitude and longitude.
        /// </summary>
        private readonly static string[] TilePathPrefixes = new[] { "a", "b", "c", "dispatcher", "e", "f" };
        #endregion

        /// <summary>
        /// Constructor; Defines the base uri format for the tile source.
        /// </summary>
        public OsmaRender() : base() {
            UriFormat = "http://{0}.tah.openstreetmap.org/Tiles/tile/{1}/{2}/{3}.png";
        }

        #region Public Methods
        /// <summary>
        /// Generate the uri for a specific tile.
        /// </summary>
        /// <param name="x">Longitude coordinate in request area</param>
        /// <param name="y">Latitude coordinate in request area</param>
        /// <param name="zoomLevel">Zoom level of map</param>
        /// <returns>The uri for the tile for the region specified</returns>
        public override Uri GetUri(int x, int y, int zoomLevel) {
            if(zoomLevel <= 0) return null;
            string url = string.Format(UriFormat, TilePathPrefixes[(y % 3) + (3 * (x % 2))], zoomLevel, x, y);
            return new Uri(url);
        }
        #endregion
    }
}