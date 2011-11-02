
namespace RhitMobile.Services.Requests {
    public class DepartableRequestPart : RequestPart {
        public DepartableRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/alts{0}";
        }
    }
}
