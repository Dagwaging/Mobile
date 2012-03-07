using System;
using System.Collections.Generic;
using Rhit.Applications.Models.Services;

namespace Rhit.Applications.Models.Events {
    public delegate void CampusServicesEventHandler(Object sender, CampusServicesEventArgs e);

    public class CampusServicesEventArgs : ServiceEventArgs {
        public CampusServicesEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }
        public CampusServicesCategory_DC Root { get; set; }
    }
}
