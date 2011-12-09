using System;

namespace Rhit.Applications.Model.Events {
    public delegate void AuthenticationEventHandler(Object sender, AuthenticationEventArgs e);

    public class AuthenticationEventArgs : ServiceEventArgs {
        public bool Authorized { get; set; }

        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
