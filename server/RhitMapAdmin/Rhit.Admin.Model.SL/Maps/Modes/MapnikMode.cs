using System.Collections.Generic;
using Rhit.Admin.Model.Maps.Sources;

namespace Rhit.Admin.Model.Maps.Modes {
    public class MapnikMode : RhitMode {
        public MapnikMode() {
            Label = "Mapnik";
            Sources = new List<BaseTileSource>() {
                new MapnikSource(),
            };
            CurrentSource = Sources[0];
        }
    }
}
