using System;
using System.Collections.Generic;
using Rhit.Applications.Model.Services;

namespace Rhit.Applications.Model.Events {
    public delegate void CampusServicesEventHandler(Object sender, CampusServicesEventArgs e);

    public class CampusServicesEventArgs : ServiceEventArgs {
        public CampusServicesEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }
        public CampusServicesCategory_DC Root { get; set; }
    }
}
