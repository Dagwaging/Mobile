using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class PathDataRequestPart : RequestPart {
        public PathDataRequestPart(string baseUrl, Guid token) : base(baseUrl) {
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
}
