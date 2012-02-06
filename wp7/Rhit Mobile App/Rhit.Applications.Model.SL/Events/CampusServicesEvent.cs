using System;
using System.Collections.Generic;
using Rhit.Applications.Model.Services;

namespace Rhit.Applications.Model.Events {
    public delegate void CampusServicesEventHandler(Object sender, CampusServicesEventArgs e);

    public class CampusServicesEventArgs : ServiceEventArgs {
        public CampusServicesEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }
        public IList<CampusServicesCategory_DC> Categories { get; set; }
    }
}
