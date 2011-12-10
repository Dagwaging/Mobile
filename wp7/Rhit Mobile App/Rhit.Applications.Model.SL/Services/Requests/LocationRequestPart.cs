using System;

namespace Rhit.Applications.Model.Services.Requests {
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

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }
}
