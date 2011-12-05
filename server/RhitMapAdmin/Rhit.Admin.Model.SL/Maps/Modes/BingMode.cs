using System.Collections.Generic;
using Rhit.Admin.Model.Maps.Sources;

namespace Rhit.Admin.Model.Maps.Modes {
    public class BingMode : RhitMode {
        public BingMode() {
            Label = "Bing";
            Sources = new List<BaseTileSource>() {
                new BingSource(BingType.Aerial),
                new BingSource(BingType.Hybrid),
                new BingSource(BingType.Road),
            };
            CurrentSource = Sources[0];
        }
    }
}
