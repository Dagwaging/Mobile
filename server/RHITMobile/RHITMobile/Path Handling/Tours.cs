using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Data;
using System.Data.SqlClient;

namespace RHITMobile {
    public class ToursHandler : PathHandler {
        public ToursHandler() {
            Redirects.Add("tags", new ToursTagsHandler());
            Redirects.Add("offcampus", new ToursOffCampusHandler());
        }
    }

    public class ToursTagsHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;

            if (query.ContainsKey("version")) {
                double version;
                if (Double.TryParse(query["version"], out version)) {
                    if (version >= Program.TagsVersion) {
                        yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.NoContent));
                    }
                }
            }

            var response = new TagsResponse(Program.TagsVersion);
            var categories = new Dictionary<string, TagCategory>();
            categories.Add("", response.Root);

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetTagCategories");
            using (var table = TM.GetResult<DataTable>(currentThread)) {
                foreach (DataRow row in table.Rows) {
                    var category = new TagCategory(row);
                    categories[category.Parent ?? ""].Children.Add(category);
                    categories.Add(category.Name, category);
                }
            }

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetTags");
            using (var table = TM.GetResult<DataTable>(currentThread)) {
                foreach (DataRow row in table.Rows) {
                    var tag = new Tag(row);
                    categories[(string)row["category"]].Tags.Add(tag);
                }
            }

            yield return TM.Return(currentThread, new JsonResponse(response));
        }
    }

    public class ToursOffCampusHandler : PathHandler {
        public ToursOffCampusHandler() {
            IntRedirect = this;
        }

        protected override IEnumerable<ThreadInfo> HandleIntPath(ThreadManager TM, int value, object state) {
            var currentThread = TM.CurrentThread;

            var tags = state is List<int> ? (List<int>)state : new List<int>();
            tags.Add(value);

            yield return TM.Return(currentThread, tags);
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;

            if (!(state is List<int>)) {
                yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
            }

            var tags = (List<int>)state;
            var locations = new Dictionary<int, LocationRank>();
            int order = 0;

            foreach (int tag in tags) {
                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetTaggedLocationIds",
                    new SqlParameter("tag", tag));
                foreach (DataRow row in TM.GetResult<DataTable>(currentThread).Rows) {
                    int id = (int)row["location"];

                    if (!locations.ContainsKey(id))
                        locations[id] = new LocationRank(id, order++);
                    locations[id].AddHit((int)row["priority"]);
                }
            }

            var response = new LocationIdsResponse(locations.Values.OrderBy(x => x, new LocationRankComparer()).Select(l => l.LocId).ToList());

            yield return TM.Return(currentThread, new JsonResponse(response));
        }
    }

    public class LocationRank {
        public LocationRank(int id, int order) {
            LocId = id;
            Order = order;
            Hits = new Dictionary<int, int>();
            LargestPriority = -1;
        }

        public int LocId { get; set; }
        public int Order { get; set; }
        private Dictionary<int, int> Hits { get; set; }
        private int LargestPriority { get; set; }

        public void AddHit(int priority) {
            if (!Hits.ContainsKey(priority)) Hits[priority] = 1;
            else Hits[priority]++;

            if (priority > LargestPriority) LargestPriority = priority;
        }

        public int GetHits(int priority) {
            if (priority > LargestPriority) return -1;
            if (!Hits.ContainsKey(priority)) return 0;
            return Hits[priority];
        }
    }

    public class LocationRankComparer : IComparer<LocationRank> {
        public int Compare(LocationRank x, LocationRank y) {
            int priority = 1;
            while (true) {
                int xHits = x.GetHits(priority);
                int yHits = y.GetHits(priority);

                if (xHits != yHits) return yHits - xHits;
                else if (xHits < 0) return x.Order - y.Order;

                priority++;
            }
        }
    }
}
