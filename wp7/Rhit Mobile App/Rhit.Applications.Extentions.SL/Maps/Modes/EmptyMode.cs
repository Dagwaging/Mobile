using System.Collections.Generic;
using Rhit.Applications.Extentions.Maps.Sources;

namespace Rhit.Applications.Extentions.Maps.Modes {
    public class EmptyMode : BaseMode {
        public EmptyMode() {
            Label = "None";
            Sources = new List<BaseTileSource>();
        }
    }
}
