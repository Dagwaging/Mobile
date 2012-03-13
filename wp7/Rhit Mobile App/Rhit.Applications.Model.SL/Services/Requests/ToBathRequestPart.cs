
namespace Rhit.Applications.Model.Services.Requests {
    public class ToBathRequestPart : RequestPart {
        public ToBathRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/tobath{0}";
        }

        public MensRequestPart Mens {
            get { return new MensRequestPart(FullUrl); }
        }

        public WomensRequestPart Womens {
            get { return new WomensRequestPart(FullUrl); }
        }
    }
}