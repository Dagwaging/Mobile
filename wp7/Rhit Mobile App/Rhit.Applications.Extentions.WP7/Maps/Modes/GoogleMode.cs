using System.Collections.Generic;
using Rhit.Applications.Extentions.Maps.Sources;

namespace Rhit.Applications.Extentions.Maps.Modes {
    public class GoogleMode : BaseMode {
        public GoogleMode() {
            Label = "Google";
            Sources = new List<BaseTileSource>() {
                new GoogleSource(GoogleType.Hybrid),
                new GoogleSource(GoogleType.Satellite),
                new GoogleSource(GoogleType.Street),
                //new GoogleSource(GoogleType.Physical),
                //new GoogleSource(GoogleType.PhysicalHybrid),
            };
        }
    }
}
