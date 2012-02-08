using System;
using System.Collections.Generic;

namespace Rhit.Applications.Model.Events {
    public delegate void ServiceEventHandler(Object sender, ServiceEventArgs e);

    public class ServiceEventArgs : EventArgs { }
}
