using System;
using System.Collections.Generic;
using RhitMobile.ObjectModel;

namespace RhitMobile.Events {
    public delegate void SearchEventHandler(Object sender, SearchEventArgs e);

    public class SearchEventArgs : ServiceEventArgs {
        public List<RhitLocation> Places { get; set; }
    }
}
