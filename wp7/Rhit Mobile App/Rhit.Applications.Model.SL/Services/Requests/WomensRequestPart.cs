
namespace Rhit.Applications.Model.Services.Requests {
    public class WomensRequestPart : RequestPart {
        public WomensRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/womens{0}";
        }
    }
}