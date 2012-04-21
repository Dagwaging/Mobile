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
            Redirects.Add("oncampus", new ToursOnCampusHandler());
        }

        public static IEnumerable<ThreadInfo> GetTaggedLocations(ThreadManager TM, List<int> tags) {
            var currentThread = TM.CurrentThread;

            var locations = new Dictionary<int, LocationRank>();
            int order = 0;

            foreach (int tag in tags) {
                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetTaggedLocations",
                    new SqlParameter("tag", tag));
                foreach (DataRow row in TM.GetResult<DataTable>(currentThread).Rows) {
                    var locationRank = new LocationRank(row, order++);

                    if (!locations.ContainsKey(locationRank.LocId))
                        locations[locationRank.LocId] = locationRank;
                    locations[locationRank.LocId].AddHit(locationRank.Priority);
                }
            }

            yield return TM.Return(currentThread, locations.Values.OrderBy(x => x, new LocationRankComparer()));
        }
    }

    public class ToursTagsHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;

            if (query.ContainsKey("version")) {
                double version;
                if (Double.TryParse(query["version"], out version)) {
                    if (version >= Program.TagsVersion) {
                        throw new UpToDateException("Version is up to date.");
                    }
                }
            }

            var response = new TagsResponse(Program.TagsVersion);
            var categories = new Dictionary<string, TagCategory>();
            categories.Add("", response.TagsRoot);

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetTagCategories");
            using (var table = TM.GetResult<DataTable>(currentThread)) {
                foreach (DataRow row in table.Rows) {
                    var category = new TagCategory(row);
                    categories.Add(category.Name, category);
                }
                foreach (DataRow row in table.Rows) {
                    var category = new TagCategory(row);
                    categories[category.Parent ?? ""].Children.Add(categories[category.Name]);
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

            if (!(state is List<int>))
                throw new BadRequestException("Need to specify at least one tag.");

            yield return TM.Await(currentThread, ToursHandler.GetTaggedLocations(TM, (List<int>)state));

            var response = new LocationIdsResponse(TM.GetResult<IOrderedEnumerable<LocationRank>>(currentThread).Select(lr => lr.LocId).ToList());

            yield return TM.Return(currentThread, new JsonResponse(response));
        }
    }

    public class ToursOnCampusHandler : PathHandler {
        public ToursOnCampusHandler() {
            Redirects.Add("fromloc", new ToursFromLocHandler());
            Redirects.Add("fromgps", new ToursFromGpsHandler());
            Redirects.Add("status", new ToursStatusHandler());
        }
    }

    public class ToursStatusHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleIntPath(ThreadManager TM, int value, object state) {
            var currentThread = TM.CurrentThread;
            IntRedirect = TourFinder.GetFinder(value);
            if (IntRedirect == null)
                throw new BadRequestException("Invalid Tours ID.");
            yield return TM.Return(currentThread, false);
        }
    }

    public class ToursFromLocHandler : PathHandler {
        public ToursFromLocHandler() {
            IntRedirect = new ToursOnCampusTagsHandler();
        }

        protected override IEnumerable<ThreadInfo> HandleIntPath(ThreadManager TM, int value, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationDepartNode",
                new SqlParameter("@location", value));
            var table = TM.GetResult<DataTable>(currentThread);
            if (table.Rows.Count == 0)
                throw new BadRequestException("Cannot depart from this location.");
            yield return TM.Return(currentThread, new TourFinder(table.Rows[0]));
        }
    }

    public class ToursFromGpsHandler : PathHandler {
    }

    public class ToursOnCampusTagsHandler : PathHandler {
        public ToursOnCampusTagsHandler() {
            IntRedirect = this;
        }

        protected override IEnumerable<ThreadInfo> HandleIntPath(ThreadManager TM, int value, object state) {
            var currentThread = TM.CurrentThread;

            ((TourFinder)state).Tags.Add(value);

            yield return TM.Return(currentThread, state);
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;

            var tourFinder = (TourFinder)state;
            if (!tourFinder.Tags.Any())
                throw new BadRequestException("Need to specify at least one tag.");

            yield return TM.Await(currentThread, ToursHandler.GetTaggedLocations(TM, tourFinder.Tags));
            tourFinder.Locations = TM.GetResult<IOrderedEnumerable<LocationRank>>(currentThread).ToList();

            yield return TM.Await(currentThread, tourFinder.HandlePath(TM, false, new List<string>(), query, null, true));
            yield return TM.Return(currentThread);
        }
    }

    public class LocationRank {
        public LocationRank(DataRow row, int order) {
            LocId = (int)row["id"];
            Center = new LatLong(row);
            Priority = (int)row["priority"];
            Order = order;
            Hits = new Dictionary<int, int>();
            LargestPriority = -1;
        }

        public int LocId { get; set; }
        public LatLong Center { get; set; }
        public int Priority { get; set; }

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
