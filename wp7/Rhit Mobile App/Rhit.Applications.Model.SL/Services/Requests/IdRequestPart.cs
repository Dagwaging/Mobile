using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class IdRequestPart : RequestPart {
        public IdRequestPart(string baseUrl, int id) : base(baseUrl) {
            Id = id;
            PartUrl = "/id/{1}{0}";
        }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", Id); }
        }

        public int Id { get; set; }

        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }
}
