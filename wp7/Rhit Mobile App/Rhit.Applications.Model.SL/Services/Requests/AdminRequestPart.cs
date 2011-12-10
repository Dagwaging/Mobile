using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class AdminRequestPart : RequestPart {
        public AdminRequestPart(string baseUrl, Guid token, string storedProcedure) : base(baseUrl) {
            PartUrl = "/admin/" + token.ToString() + "/storedproc" + storedProcedure + "{0}";
        }

        public AdminRequestPart(string baseUrl, string username, string password) : base(baseUrl) {
            PartUrl = "/admin/authenticate/" + username + "/" + password + "{0}";
        }
    }
}
