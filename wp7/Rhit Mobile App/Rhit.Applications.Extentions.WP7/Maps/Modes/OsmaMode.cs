using System.Collections.Generic;
using Rhit.Applications.Extentions.Maps.Sources;

namespace Rhit.Applications.Extentions.Maps.Modes {
    public class OsmaMode : BaseMode {
        public OsmaMode() {
            Label = "OsmaRender";
            Sources = new List<BaseTileSource>() {
                new OsmaSource(),
            };
        }
    }
}