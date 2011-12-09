
namespace Rhit.Applications.Model.Services.Requests {
    public class ToLocRequestPart : IdRequestPart {
        public ToLocRequestPart(string baseUrl, int id) : base(baseUrl, id) {
            PartUrl = "/toloc/{1}{0}";
        }
    }
}
