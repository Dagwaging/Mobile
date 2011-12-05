using System.Collections.Generic;
using Rhit.Admin.Model.Maps.Sources;

namespace Rhit.Admin.Model.Maps.Modes {
    public class OsmaMode : RhitMode {
        public OsmaMode() {
            Label = "OsmaRender";
            Sources = new List<BaseTileSource>() {
                new OsmaSource(),
            };
            CurrentSource = Sources[0];
        }
    }
}