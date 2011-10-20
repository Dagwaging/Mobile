using System;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Core;

namespace RhitMobile.MapSource {
    /// <summary>
    /// Generic map tile source for various Bing map tile sources.
    /// Specify the MapType to change the map source.
    /// </summary>
    public class BaseBingSource : BaseTileSource {
        #region Private Fields
        private BingType _type;
        #endregion

        /// <summary>
        /// Constructor; Defines the default MapType (Bing Core Aerial).
        /// </summary>
        public BaseBingSource() : base() {
            MapType = BingType.CoreAerial;
        }

        #region Public Properties
        /// <summary>
        /// The type of the Bing map.; Also handles changing the Mode of the source.
        /// </summary>
        public BingType MapType {
            get { return _type; }
            set {
                _type = value;
                switch(value) {
                    case BingType.Aerial:
                        Mode = new MercatorMode();
                        UriFormat = "http://h{0}.ortho.tiles.virtualearth.net/tiles/h{1}.jpeg?g=203";
                        break;
                    case BingType.CoreAerial:
                        Mode = new AerialMode();
                        break;
                    case BingType.CoreRoad:
                        Mode = new RoadMode();
                        break;
                    case BingType.Road:
                        Mode = new MercatorMode();
                        UriFormat = "http://requestString{0}.ortho.tiles.virtualearth.net/tiles/requestString{1}.png?g=203";
                        break;
                }
            }
        }
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
            if(zoomLevel <= 0) return null;
            string quadKey = TileToQuadKey(x, y, zoomLevel);
            string veLink = string.Format(UriFormat, new object[] { quadKey[quadKey.Length - 1], quadKey });
            return new Uri(veLink);
        }
        #endregion
    }
}