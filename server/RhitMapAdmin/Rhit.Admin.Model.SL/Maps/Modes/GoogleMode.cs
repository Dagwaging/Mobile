using System.Collections.Generic;
using Rhit.Admin.Model.Maps.Sources;

namespace Rhit.Admin.Model.Maps.Modes {
    public class GoogleMode : RhitMode {
        public GoogleMode() {
            Label = "Google";
            Sources = new List<BaseTileSource>() {
                new GoogleSource(GoogleType.Hybrid),
                new GoogleSource(GoogleType.Satellite),
                new GoogleSource(GoogleType.Street),
                //new GoogleSource(GoogleType.Physical),
                //new GoogleSource(GoogleType.PhysicalHybrid),
            };
            CurrentSource = Sources[1];
        }
    }
}
