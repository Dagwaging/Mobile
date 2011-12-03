
namespace Rhit.Admin.Model.Services.Requests {
    public class StatusRequestPart : IdRequestPart {
        public StatusRequestPart(string baseUrl, int id) : base(baseUrl, id) {
            PartUrl = "/status/{1}{0}";
        }
    }
}
