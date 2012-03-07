using System;

namespace Rhit.Applications.Models.Services.Requests {
    public class AdminRequestPart : RequestPart {
        public AdminRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/admin{0}";
        }

        public PathDataRequestPart PathData(Guid token) {
            return new PathDataRequestPart(FullUrl, token);
        }

        public UpdateVersionRequestPart UpdateVersion(Guid token, double version) {
            return new UpdateVersionRequestPart(FullUrl, token, version);
        }

        public AuthenticateRequestPart Authenticate(string username, string password) {
            return new AuthenticateRequestPart(FullUrl, username, password);
        }

        public StoredProcedureRequestPart StoredProcedure(Guid token, string spName) {
            return new StoredProcedureRequestPart(FullUrl, token, spName);
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class AuthenticateRequestPart : RequestPart {
        public AuthenticateRequestPart(string baseUrl, string username, string password)
            : base(baseUrl) {
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

    public class PathDataRequestPart : RequestPart {
        public PathDataRequestPart(string baseUrl, Guid token)
            : base(baseUrl) {
            Token = token;
            PartUrl = "/{1}/pathdata{0}";
        }
        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", Token); }
        }

        public Guid Token { get; set; }


        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }

    public class StoredProcedureRequestPart : RequestPart {
        public StoredProcedureRequestPart(string baseUrl, Guid token, string spName)
            : base(baseUrl) {
            Token = token;
            Name = spName;
            PartUrl = "/{1}/storedproc/{2}{0}";
        }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", Token, Name); }
        }

        public Guid Token { get; set; }

        public string Name { get; set; }

        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }

    public class UpdateVersionRequestPart : RequestPart {
        public UpdateVersionRequestPart(string baseUrl, Guid token, double version)
            : base(baseUrl) {
            Token = token;
            Version = version;
            PartUrl = "/{1}/updateversion/{2}{0}";
        }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", Token, Version); }
        }

        public Guid Token { get; set; }

        public double Version { get; set; }

        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }
}
