using System;
using System.Collections.Generic;

namespace Rhit.Applications.Model.Events {

    public delegate void LocationChangesEventHandler(Object sender, LocationChangesEventArgs e);

    /// <summary>
    /// Event argument object for MapPolgon object (Outline) events.
    /// </summary>
    public class LocationChangesEventArgs : EventArgs {
        public RhitLocation OldLocation { get; set; }
        public RhitLocation NewLocation { get; set; }
        public ICollection<RhitLocation> OldLocations { get; set; }
        public ICollection<RhitLocation> NewLocations { get; set; }
    }
}
