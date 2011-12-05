using System;

namespace Rhit.Admin.Model.Services.Requests {
    public class LocationRequestPart : RequestPart {
        public LocationRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/locations{0}";
        }

        public DescRequestPart Desc(int id) {
            return new DescRequestPart(FullUrl, id);
        }

        public NamesRequestPart Names {
            get { return new NamesRequestPart(FullUrl); }
        }

        public DataRequestPart Data {
            get { return new DataRequestPart(FullUrl); }
        }

        public override string ToString() {
            throw new InvalidOperationException("Not a valid request string.");
        }
    }
}
