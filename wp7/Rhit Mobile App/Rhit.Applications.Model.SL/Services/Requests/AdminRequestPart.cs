using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class AdminRequestPart : RequestPart {
        public AdminRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/admin{0}";
        }

        public PathDataRequestPart PathData(Guid token) {
            return new PathDataRequestPart(FullUrl, token);
        }

        public UpdateVersionRequestPart UpdateVersion(Guid token, double version) {
            return new UpdateVersionRequestPart(FullUrl, token, version);
        }

        public AuthenticateRequestPart Authenticate(string username, string password) {
            return new AuthenticateRequestPart(FullUrl, username, password);
        }

        public StoredProcedureRequestPart StoredProcedure(Guid token, string spName) {
            return new StoredProcedureRequestPart(FullUrl, token, spName);
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }
}
