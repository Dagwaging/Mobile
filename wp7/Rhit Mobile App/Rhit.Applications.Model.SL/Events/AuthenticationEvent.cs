using System;

namespace Rhit.Applications.Models.Events {
    public delegate void AuthenticationEventHandler(Object sender, AuthenticationEventArgs e);

    public class AuthenticationEventArgs : ServiceEventArgs {
        public AuthenticationEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public DateTime Expiration { get; set; }

        public string Token { get; set; }
    }
}
