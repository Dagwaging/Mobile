
namespace Rhit.Admin.Model.Services.Requests {
    public class WithinRequestPart : IdRequestPart {
        public WithinRequestPart(string baseUrl, int id) : base(baseUrl, id) {
            PartUrl = "/within/{1}{0}";
        }

        public NoTopRequestPart NoTop {
            get { return new NoTopRequestPart(FullUrl); }
        }
    }
}
