using System;

namespace Rhit.Applications.Model.Services.Requests {
    public class DirectionsRequestPart : RequestPart {
        public DirectionsRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/directions{0}";
        }

        public StatusRequestPart Status(int id) {
            return new StatusRequestPart(FullUrl, id);
        }

        public FromLocRequestPart FromLoc(int id) {
            return new FromLocRequestPart(FullUrl, id);
        }

        public DirectionsTestRequestPart Test {
            get { return new DirectionsTestRequestPart(FullUrl); }
        }

        public ToursTestRequestPart ToursTest {
            get { return new ToursTestRequestPart(FullUrl); }
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class ToursTestRequestPart : RequestPart {
        public ToursTestRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/testing/tour{0}";
        }
    }

    public class DirectionsTestRequestPart : RequestPart {
        public DirectionsTestRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/testing/directions{0}";
        }
    }

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

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class FromLocRequestPart : IdRequestPart {
        public FromLocRequestPart(string baseUrl, int id)
            : base(baseUrl, id) {
            PartUrl = "/fromloc/{1}{0}";
        }

        public ToLocRequestPart ToLoc(int id) {
            return new ToLocRequestPart(FullUrl, id);
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class MensRequestPart : RequestPart {
        public MensRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/mens{0}";
        }
    }

    public class StatusRequestPart : IdRequestPart {
        public StatusRequestPart(string baseUrl, int id)
            : base(baseUrl, id) {
            PartUrl = "/status/{1}{0}";
        }
    }

    public class ToBathRequestPart : RequestPart {
        public ToBathRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/tobath{0}";
        }

        public MensRequestPart Mens {
            get { return new MensRequestPart(FullUrl); }
        }

        public WomensRequestPart Womens {
            get { return new WomensRequestPart(FullUrl); }
        }
    }

    public class ToLocRequestPart : IdRequestPart {
        public ToLocRequestPart(string baseUrl, int id)
            : base(baseUrl, id) {
            PartUrl = "/toloc/{1}{0}?wait=true";
        }
    }

    public class ToPrinterRequestPart : RequestPart {
        public ToPrinterRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/tobath{0}";
        }
    }

    public class WomensRequestPart : RequestPart {
        public WomensRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/womens{0}";
        }
    }
}
