using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class AuthenticateRequestPart : RequestPart {
        public AuthenticateRequestPart(string baseUrl, string username, string password) : base(baseUrl) {
            UserName = username;
            Password = password;
            PartUrl = "/authenticate/{1}/{2}{0}";
        }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", UserName, Password); }
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }
}
