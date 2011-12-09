
namespace Rhit.Applications.Model.Services.Requests {
    public class MensRequestPart : RequestPart {
        public MensRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/mens{0}";
        }
    }
}