using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class UpdateVersionRequestPart : RequestPart {
        public UpdateVersionRequestPart(string baseUrl, Guid token, double version) : base(baseUrl) {
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
