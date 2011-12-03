using System;

namespace Rhit.Admin.Model.Services.Requests {
    public class FromLocRequestPart : IdRequestPart {
        public FromLocRequestPart(string baseUrl, int id): base(baseUrl, id) {
            PartUrl = "/fromloc/{1}{0}";
        }

        public ToLocRequestPart ToLoc(int id) {
            return new ToLocRequestPart(FullUrl, id);
        }

        public override string ToString() {
            throw new InvalidOperationException("Not a valid request string.");
        }
    }
}
