using System.Collections.Generic;
using Rhit.Applications.Extentions.Maps.Sources;

namespace Rhit.Applications.Extentions.Maps.Modes {
    public class RoseMode : BaseMode {
        public RoseMode() {
            Sources = new List<BaseTileSource>() {
                new RoseOverlay(),
            };
            Label = "Rose";
        }
    }
}
