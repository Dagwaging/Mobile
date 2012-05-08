using System;
using System.Collections.Generic;

namespace Rhit.Applications.Models.Events {
    public delegate void FolderNamesEventHandler(Object sender, FolderNamesEventArgs e);

    public class FolderNamesEventArgs : ServiceEventArgs {
        public FolderNamesEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public IEnumerable<string> FolderNames { get; set; }
    }
}
