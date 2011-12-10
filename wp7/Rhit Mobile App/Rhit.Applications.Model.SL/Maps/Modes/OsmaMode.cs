using System.Collections.Generic;
using Rhit.Applications.Model.Maps.Sources;

namespace Rhit.Applications.Model.Maps.Modes {
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