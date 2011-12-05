
namespace Rhit.Admin.Model.Services.Requests {
    public class NamesRequestPart : RequestPart {
        public NamesRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/names{0}";
        }

        public DepartableRequestPart Departable {
            get { return new DepartableRequestPart(FullUrl); }
        }
    }
}
