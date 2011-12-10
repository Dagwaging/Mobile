using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class FromLocRequestPart : IdRequestPart {
        public FromLocRequestPart(string baseUrl, int id): base(baseUrl, id) {
            PartUrl = "/fromloc/{1}{0}";
        }

        public ToLocRequestPart ToLoc(int id) {
            return new ToLocRequestPart(FullUrl, id);
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }
}
