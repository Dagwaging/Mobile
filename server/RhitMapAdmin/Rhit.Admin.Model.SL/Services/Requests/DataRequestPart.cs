using System;

namespace Rhit.Admin.Model.Services.Requests {
    public class DataRequestPart : RequestPart {
        public DataRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/data{0}";
        }

        public WithinRequestPart Within(int id) {
            return new WithinRequestPart(FullUrl, id);
        }

        public IdRequestPart Id(int id) {
            return new IdRequestPart(FullUrl, id);
        }

        public TopRequestPart Top {
            get { return new TopRequestPart(FullUrl); }
        }

        public AllRequestPart All {
            get { return new AllRequestPart(FullUrl); }
        }

        public override string ToString() {
            throw new InvalidOperationException("Not a valid request string.");
        }
    }
}
