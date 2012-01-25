using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class StoredProcedureRequestPart : RequestPart {
        public StoredProcedureRequestPart(string baseUrl, Guid token, string spName) : base(baseUrl) {
            Token = token;
            Name = spName;
            PartUrl = "/{1}/storedproc/{2}{0}";
        }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", Token, Name); }
        }

        public Guid Token { get; set; }

        public string Name { get; set; }

        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }
}