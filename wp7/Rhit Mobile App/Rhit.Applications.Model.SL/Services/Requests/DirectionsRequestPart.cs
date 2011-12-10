using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class DirectionsRequestPart : RequestPart {
        public DirectionsRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/directions{0}";
        }

        public StatusRequestPart Status(int id) {
            return new StatusRequestPart(FullUrl, id);
        }

        public FromLocRequestPart FromLoc(int id) {
            return new FromLocRequestPart(FullUrl, id);
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }
}
