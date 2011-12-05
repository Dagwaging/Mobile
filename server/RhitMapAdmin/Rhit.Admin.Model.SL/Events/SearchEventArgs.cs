using System;
using System.Collections.Generic;

namespace Rhit.Admin.Model.Events {
    public delegate void SearchEventHandler(Object sender, SearchEventArgs e);

    public class SearchEventArgs : ServiceEventArgs {
        public List<RhitLocation> Places { get; set; }
    }
}
