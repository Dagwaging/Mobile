
namespace Rhit.Applications.Model.Services.Requests {
    public class VersionRequestPart : RequestPart {
        public VersionRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "{0}";
        }
    }
}
