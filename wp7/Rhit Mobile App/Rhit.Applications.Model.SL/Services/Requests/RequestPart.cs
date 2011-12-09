using System;

namespace Rhit.Applications.Model.Services.Requests {
    public abstract class RequestPart {
        public RequestPart(string baseUrl) {
            BaseUrl = baseUrl;
        }

        protected virtual string FullUrl {
            get { return String.Format(BaseUrl, PartUrl); }
        }

        public string BaseUrl { get; set; }

        public string PartUrl { get; set; }

        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }
}
