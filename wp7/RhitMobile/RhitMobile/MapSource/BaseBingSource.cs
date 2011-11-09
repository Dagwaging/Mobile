using System;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Core;

namespace RhitMobile.MapSource {
    /// <summary>
    /// Generic map tile source for various Bing map tile sources.
    /// Specify the MapType to change the map source.
    /// </summary>
    public class BaseBingSource : BaseTileSource {
        /// <summary>
        /// Constructor; Defines the default MapType (Bing Core Aerial).
        /// </summary>
        public BaseBingSource() : base() {
            MapType = BingType.Aerial;
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
            if(Mode is AerialMode || Mode is RoadMode) return base.GetUri(x, y, zoomLevel);
            if(zoomLevel <= 0) return null;
            string quadKey = TileToQuadKey(x, y, zoomLevel);
            string veLink = string.Format(UriFormat, (char) MapType, quadKey[quadKey.Length - 1], quadKey);
            return new Uri(veLink);
        }
        #endregion
    }
}