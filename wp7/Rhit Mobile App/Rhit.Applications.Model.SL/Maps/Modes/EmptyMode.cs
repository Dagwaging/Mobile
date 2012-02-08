using System.Collections.Generic;
using Rhit.Applications.Model.Maps.Sources;

namespace Rhit.Applications.Model.Maps.Modes {
    public class EmptyMode : RhitMode {
        public EmptyMode() {
            Label = "None";
            Sources = new List<BaseTileSource>();
            CurrentSource = null;
        }
    }
}
