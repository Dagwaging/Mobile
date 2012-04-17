using System;

namespace Rhit.Applications.Models.Events {
    public delegate void IdentificationEventHandler(Object sender, IdentificationEventArgs e);

    public class IdentificationEventArgs : ServiceEventArgs {
        public IdentificationEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public int Id { get; set; }
    }
}
