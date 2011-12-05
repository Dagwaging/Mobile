using System;

namespace Rhit.Admin.Model.Services.Requests {
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

        public override string ToString() {
            throw new InvalidOperationException("Not a valid request string.");
        }
    }
}
