using System;

namespace RhitMobile.Events {
    public delegate void ServiceEventHandler(Object sender, ServiceEventArgs e);

    public class ServiceEventArgs : EventArgs { }
}
