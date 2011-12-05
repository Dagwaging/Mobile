
namespace Rhit.Admin.Model.Services.Requests {
    public class ToPrinterRequestPart : RequestPart {
        public ToPrinterRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/tobath{0}";
        }
    }
}