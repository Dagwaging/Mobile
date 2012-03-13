
namespace Rhit.Applications.Model.Services.Requests {
    public class DescRequestPart : IdRequestPart {
        public DescRequestPart(string baseUrl, int id) : base(baseUrl, id) {
            PartUrl = "/desc/{1}{0}";
        }
    }
}
