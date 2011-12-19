using System.Collections.Generic;
using System.Windows;
using Microsoft.Maps.MapControl;
using System;

namespace Rhit.Applications.ViewModel.Providers {
    public interface IBuildingMappingProvider {
        void QueryMapping();

        event FloorMappingEventHandler MappingFinalized;
    }

    public delegate void FloorMappingEventHandler(Object sender, FloorMappingEventArgs e);
    public class FloorMappingEventArgs : EventArgs {
        public int FloorNumber { get; set; }
        public Dictionary<Location, Point> Mapping { get; set; }
    }
}
