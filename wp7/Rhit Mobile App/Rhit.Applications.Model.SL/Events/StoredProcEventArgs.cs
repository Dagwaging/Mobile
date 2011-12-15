using System;
using System.Collections.Generic;

namespace Rhit.Applications.Model.Events {
    public delegate void StoredProcEventHandler(Object sender, StoredProcEventArgs e);

    public class StoredProcEventArgs : ServiceEventArgs {
        public List<string> Columns { get; set; }

        public List<List<string>> Table { get; set; }
    }
}
