using System;

namespace Rhit.Admin.Model.Maps.Sources {
    /// <summary>
    /// A tile source from the open source mapping project Mapnik (http://mapnik.org/).
    /// The tile source comes from http://openstreet.org/.
    /// </summary>
    public class MapnikSource : BaseTileSource {
        #region Private Fields
        /// <summary>
        /// Used to define which region the tile is in.
        /// Mapnik splits tile locations into thirds by latitudes.
        /// </summary>
        private readonly static string[] TilePathPrefixes = new[] { "a", "b", "c" };
        #endregion

        /// <summary>
        /// Constructor; Defines the base uri format for the tile source.
        /// </summary>
        public MapnikSource() {
            Label = "Mapnik";
            UriFormat = "http://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png";
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
            if(zoomLevel <= 0 || zoomLevel > 19) return null;
            string url = string.Format(UriFormat, TilePathPrefixes[y % 3], zoomLevel, x, y);
            return new Uri(url);
        }
        #endregion
    }
}
