using System.Collections.Generic;
using Rhit.Applications.Extentions.Maps.Sources;

namespace Rhit.Applications.Extentions.Maps.Modes {
    public class MapnikMode : BaseMode {
        public MapnikMode() {
            Label = "Mapnik";
            Sources = new List<BaseTileSource>() {
                new MapnikSource(),
            };
        }
    }
}
