using System;
using System.Collections.Generic;

namespace Rhit.Applications.Model.Services.Requests {
    public class ToursRequestPart : RequestPart {
        public ToursRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/tours{0}";
        }

        public TagsRequestPart Tags {
            get { return new TagsRequestPart(FullUrl); }
        }

        public OffCampusRequestPart OffCampus {
            get { return new OffCampusRequestPart(FullUrl); }
        }

        public OnCampusRequestPart OnCampus {
            get { return new OnCampusRequestPart(FullUrl); }
        }

        public ToursTestRequestPart Test {
            get { return new ToursTestRequestPart(FullUrl); }
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class ToursTestRequestPart : RequestPart {
        public ToursTestRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/testing/tours{0}";
        }
    }

    public class TagsRequestPart : RequestPart {
        public TagsRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/tags{0}";
        }
    }

    public class OffCampusRequestPart : RequestPart {
        public OffCampusRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/offcampus{0}";
        }

        public TagRequestPart Tag(int id) {
            return new TagRequestPart(FullUrl, id);
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class OnCampusRequestPart : RequestPart {
        public OnCampusRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/oncampus{0}";
        }

        public StatusRequestPart Status(int id) {
            return new StatusRequestPart(FullUrl, id);
        }

        public ToursFromLocRequestPart FromLoc(int id) {
            return new ToursFromLocRequestPart(FullUrl, id);
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class ToursFromLocRequestPart : IdRequestPart {
        public ToursFromLocRequestPart(string baseUrl, int id) : base(baseUrl, id) {
            PartUrl = "/fromloc/{1}{0}";
        }

        public TagRequestPart Tag(int id) {
            return new TagRequestPart(FullUrl, id);
        }

        public TagRequestPart Tag(IList<int> ids) {
            int first = ids[0];
            ids.Remove(first);
            if(ids.Count <= 0) new TagRequestPart(FullUrl, first);
            return (new TagRequestPart(FullUrl, first)).AddTags(ids);
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class TagRequestPart : IdRequestPart {
        public TagRequestPart(string baseUrl, int id) : base(baseUrl, id) {
            PartUrl = "/{1}{0}";
        }

        public TagRequestPart Tag(int id) {
            return new TagRequestPart(FullUrl, id);
        }

        public TagRequestPart AddTags(IList<int> ids) {
            int first = ids[0];
            ids.Remove(first);
            if(ids.Count <= 0) new TagRequestPart(FullUrl, first);
            return (new TagRequestPart(FullUrl, first)).AddTags(ids);
        }
    }

}
