using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class RequestQuery : RequestPart {
        public RequestQuery(string baseUrl, string name, object value)
            : base(baseUrl) {
            PartUrl = String.Format("?{0}={1}", name, value);
        }

        protected override string FullUrl {
            get { return BaseUrl + PartUrl; }
        }

        public override RequestQuery AddQueryParameter(string name, object value) {
            PartUrl += String.Format("&{0}={1}", name, value);
            return this;
        }
    }
}