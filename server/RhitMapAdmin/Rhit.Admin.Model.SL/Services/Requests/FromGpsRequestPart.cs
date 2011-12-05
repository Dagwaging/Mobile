using System;

namespace Rhit.Admin.Model.Services.Requests {
    public class FromGpsRequestPart : RequestPart {
        public FromGpsRequestPart(string baseUrl, double latitude, double longitude)
            : base(baseUrl) {            
            PartUrl = "/fromgps/" + latitude.ToString() + "/" + longitude.ToString() + "{0}";
        }

        public ToLocRequestPart ToLoc(int id) {
            return new ToLocRequestPart(FullUrl, id);
        }

        public ToBathRequestPart ToBath {
            get { return new ToBathRequestPart(FullUrl); }
        }

        public ToPrinterRequestPart ToPrinter {
            get { return new ToPrinterRequestPart(FullUrl); }
        }

        public override string ToString() {
            throw new InvalidOperationException("Not a valid request string.");
        }
    }
}
