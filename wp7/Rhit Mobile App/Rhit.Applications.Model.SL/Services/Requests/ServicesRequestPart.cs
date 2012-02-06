
namespace Rhit.Applications.Model.Services.Requests {
    public class ServicesRequestPart  : RequestPart {
        public ServicesRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/services{0}";
        }
    }
}