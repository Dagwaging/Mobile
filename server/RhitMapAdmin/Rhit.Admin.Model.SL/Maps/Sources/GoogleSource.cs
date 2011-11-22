using System;

namespace Rhit.Admin.Model.Maps.Sources {
    public class GoogleSource : BaseTileSource {
        /// <summary>
        /// Constructor; Defines the base uri format and Google type for the tile source.
        /// </summary>
        public GoogleSource(GoogleType type) {
            Label = type.ToString();
            MapType = type;
            UriFormat = @"http://mt{0}.google.com/vt/lyrs={1}&z={2}&x={3}&y={4}";
        }

        #region Public Properties
        /// <summary>
        /// The type of google tile source.
        /// </summary>
        public GoogleType MapType { get; set; }
        #endregion

        #region Public Method
        /// <summary>
        /// Generate the uri for a specific tile.
        /// </summary>
        /// <param name="x">Longitude coordinate in request area</param>
        /// <param name="y">Latitude coordinate in request area</param>
        /// <param name="zoomLevel">Zoom level of map</param>
        /// <returns>The uri for the tile for the region specified</returns>
        public override Uri GetUri(int x, int y, int zoomLevel) {
            if(zoomLevel <= 0 || zoomLevel > 19) return null;
            string url = string.Format(UriFormat, (x % 2) + (2 * (y % 2)), (char) MapType, zoomLevel, x, y);
            return new Uri(url);
        }
        #endregion
    }
}