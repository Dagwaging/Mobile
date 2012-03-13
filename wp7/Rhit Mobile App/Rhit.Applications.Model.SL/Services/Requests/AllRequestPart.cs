
namespace Rhit.Applications.Model.Services.Requests {
    public class AllRequestPart : RequestPart {
        public AllRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/all{0}";
        }

        public NoDescRequestPart NoDesc {
            get { return new NoDescRequestPart(FullUrl); }
        }
    }
}
