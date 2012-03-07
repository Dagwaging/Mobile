using System;

namespace Rhit.Applications.Models.Services.Requests {
    public class AllRequestPart : RequestPart {
        public AllRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/all{0}";
        }

        public NoDescRequestPart NoDesc {
            get { return new NoDescRequestPart(FullUrl); }
        }
    }

    public class DataRequestPart : RequestPart {
        public DataRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/data{0}";
        }

        public WithinRequestPart Within(int id) {
            return new WithinRequestPart(FullUrl, id);
        }

        public IdRequestPart Id(int id) {
            return new IdRequestPart(FullUrl, id);
        }

        public TopRequestPart Top {
            get { return new TopRequestPart(FullUrl); }
        }

        public AllRequestPart All {
            get { return new AllRequestPart(FullUrl); }
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class DepartableRequestPart : RequestPart {
        public DepartableRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/departable{0}";
        }
    }

    public class DescRequestPart : IdRequestPart {
        public DescRequestPart(string baseUrl, int id)
            : base(baseUrl, id) {
            PartUrl = "/desc/{1}{0}";
        }
    }

    public class IdRequestPart : RequestPart {
        public IdRequestPart(string baseUrl, int id) : base(baseUrl) {
            Id = id;
            PartUrl = "/id/{1}{0}";
        }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", Id); }
        }

        public int Id { get; set; }

        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }

    public class LocationsRequestPart : RequestPart {
        public LocationsRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/locations{0}";
        }

        public DescRequestPart Desc(int id) {
            return new DescRequestPart(FullUrl, id);
        }

        public NamesRequestPart Names {
            get { return new NamesRequestPart(FullUrl); }
        }

        public DataRequestPart Data {
            get { return new DataRequestPart(FullUrl); }
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class NamesRequestPart : RequestPart {
        public NamesRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/names{0}";
        }

        public DepartableRequestPart Departable {
            get { return new DepartableRequestPart(FullUrl); }
        }
    }

    public class NoDescRequestPart : RequestPart {
        public NoDescRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/nodesc{0}";
        }
    }

    public class NoTopRequestPart : RequestPart {
        public NoTopRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/notop{0}";
        }
    }

    public class TopRequestPart : RequestPart {
        public TopRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/top{0}";
        }
    }

    public class WithinRequestPart : IdRequestPart {
        public WithinRequestPart(string baseUrl, int id)
            : base(baseUrl, id) {
            PartUrl = "/within/{1}{0}";
        }

        public NoTopRequestPart NoTop {
            get { return new NoTopRequestPart(FullUrl); }
        }
    }
}
