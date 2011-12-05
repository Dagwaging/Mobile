
namespace Rhit.Admin.Model.Services.Requests {
    public class DepartableRequestPart : RequestPart {
        public DepartableRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/departable{0}";
        }
    }
}
