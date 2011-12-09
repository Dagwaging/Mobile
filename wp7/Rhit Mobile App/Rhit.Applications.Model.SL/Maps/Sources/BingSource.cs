using System;

namespace Rhit.Applications.Model.Maps.Sources {
    public class BingSource : BaseTileSource {
        public BingSource(BingType type) {
            Label = type.ToString();
            MapType = type;
            UriFormat = @"http://{0}{1}.ortho.tiles.virtualearth.net/tiles/{0}{2}.jpeg?g=826";
        }

        #region Public Properties
        /// <summary>
        /// The type of bing tile source.
        /// </summary>
        public BingType MapType { get; set; }
        #endregion

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
            string quadKey = TileToQuadKey(x, y, zoomLevel);
            string veLink = string.Format(UriFormat, (char) MapType, quadKey[quadKey.Length - 1], quadKey);
            return new Uri(veLink);
        }
        #endregion

    }
}
