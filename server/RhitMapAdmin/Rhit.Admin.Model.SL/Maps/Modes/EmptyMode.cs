using System.Collections.Generic;
using Rhit.Admin.Model.Maps.Sources;

namespace Rhit.Admin.Model.Maps.Modes {
    public class EmptyMode : RhitMode {
        public EmptyMode() {
            Label = "None";
            Sources = new List<BaseTileSource>();
            CurrentSource = null;
        }
    }
}
