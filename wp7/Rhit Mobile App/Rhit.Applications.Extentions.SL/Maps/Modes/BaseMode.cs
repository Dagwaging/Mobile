using System.Collections.Generic;
using Rhit.Applications.Extentions.Maps.Sources;

namespace Rhit.Applications.Extentions.Maps.Modes {
    public class BaseMode {
        public string Label { get; protected set; }

        public List<BaseTileSource> Sources { get; protected set; }
    }
}
