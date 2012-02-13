using System.Collections.Generic;
using Rhit.Applications.Extentions.Maps.Sources;

namespace Rhit.Applications.Extentions.Maps.Modes {
    public class BingMode : BaseMode {
        public BingMode() {
            Sources = new List<BaseTileSource>() {
                new BingSource(BingType.Aerial),
                new BingSource(BingType.Hybrid),
                new BingSource(BingType.Road),
            };
            Label = "Bing";
        }
    }
}
