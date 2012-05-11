using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rhit.Applications.Extentions.Maps.Sources;

namespace Rhit.Applications.Extentions.Maps.Overlays {
    public class Overlay : BaseTileSource {

        static Overlay() {
            EmptyOverlay = new Overlay();
        }

        public static Overlay EmptyOverlay { get; protected set; }

        private Overlay() {
            Label = "None";
            UriFormat = "";
        }

        public Overlay(string uriFormat) {
            Label = "Custom";
            UriFormat = uriFormat;
        }

        public bool IsEmpty() { return UriFormat == ""; }

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
