using System.Collections.Generic;
using Rhit.Applications.Model.Maps.Sources;

namespace Rhit.Applications.Model.Maps.Modes {
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
