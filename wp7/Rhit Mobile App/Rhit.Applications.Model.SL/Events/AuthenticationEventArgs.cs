using System;

namespace Rhit.Applications.Model.Events {
    public delegate void AuthenticationEventHandler(Object sender, AuthenticationEventArgs e);

    public class AuthenticationEventArgs : ServiceEventArgs {
        public AuthenticationEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
