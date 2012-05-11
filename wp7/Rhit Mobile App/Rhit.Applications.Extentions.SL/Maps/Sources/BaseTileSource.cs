﻿using System;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
#else
using Microsoft.Maps.MapControl.Core;
using Microsoft.Maps.MapControl;
#endif

namespace Rhit.Applications.Extentions.Maps.Sources {
    public abstract class BaseTileSource : TileSource, IEquatable<BaseTileSource> {
        /// <summary>
        /// Name of the tile source.
        /// </summary>
        public string Label { get; set; }

        public override bool Equals(object obj) {
            return Equals(obj as BaseTileSource);
        }

        public bool Equals(BaseTileSource other) {
            if(this is GoogleSource && !(other is GoogleSource)) return false;
            if(other is GoogleSource && !(this is GoogleSource)) return false;
            if(this is BingSource && !(other is BingSource)) return false;
            if(other is BingSource && !(this is BingSource)) return false;
            return other != null && other.Label.Equals(Label);
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

    }
}
