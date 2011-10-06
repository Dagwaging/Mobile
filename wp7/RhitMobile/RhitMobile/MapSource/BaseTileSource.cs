using System;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Core;

namespace RhitMobile.MapSource {
    public class BaseTileSource : TileSource, IEquatable<BaseTileSource> {
        /// <summary>
        /// Constructor; Sets the default map mode to mercator.
        /// </summary>
        public BaseTileSource() {
            Mode = new MercatorMode();
        }

        #region Public Properties
        /// <summary>
        /// The MapMode that the map should be in to use this tile source.
        /// </summary>
        public MapMode Mode { get; set; }

        /// <summary>
        /// Name of the tile source.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Public Methods
        public override bool Equals(object obj) {
            return Equals(obj as BaseTileSource);
        }

        public bool Equals(BaseTileSource other) {
            return other != null && other.Name.Equals(Name);
        }

        /// <summary>
        /// Converts a tile area in to a quad key code.
        /// </summary>
        /// <param name="x">Longitude coordinate in request area</param>
        /// <param name="y">Latitude coordinate in request area</param>
        /// <param name="zoomLevel">Zoom level of map</param>
        /// <returns>Quad key</returns>
        public static string TileToQuadKey(int tileX, int tileY, int zoomLevel) {
            string quadKey = "";
            for(int i = zoomLevel; i > 0; i--) {
                int mask = 1 << (i - 1);
                int cell = 0;
                if((tileX & mask) != 0) cell++;
                if((tileY & mask) != 0) cell += 2;
                quadKey += cell;
            }
            return quadKey;
        }
        #endregion
    }
}
