
namespace Rhit.Applications.Model.Services.Requests {
    public class NoTopRequestPart : RequestPart {
        public NoTopRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/notop{0}";
        }
    }
}