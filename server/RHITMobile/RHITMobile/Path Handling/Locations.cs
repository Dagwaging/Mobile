using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Data;
using System.Data.SqlClient;

namespace RHITMobile {
    public class LocationsHandler : PathHandler {
        public const string StartHighlight = "<b>";
        public const string EndHighlight = "</b>";

        private static Dictionary<string, string> _searchCorrections = null;
        private static Dictionary<string, string> SearchCorrections {
            get {
                if (_searchCorrections == null) {
                    _searchCorrections = new Dictionary<string, string>();
                    using (var connection = new SqlConnection(Program.ConnectionString)) {
                        connection.Open();
                        var table = new DataTable();
                        using (var command = connection.CreateCommand()) {
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "spGetLocationSearchCorrections";
                            using (var reader = command.ExecuteReader()) {
                                while (reader.Read()) {
                                    SearchCorrections.Add(reader.GetString(0), reader.GetString(1));
                                }
                            }
                        }
                    }
                }
                return _searchCorrections;
            }
        }

        public LocationsHandler() {
            Redirects.Add("data", new LocationsDataHandler());
            Redirects.Add("names", new LocationsNamesHandler());
            Redirects.Add("desc", new LocationsDescHandler());
        }

        public static List<T> ApplySearchFilter<T>(Dictionary<string, string> query, Func<T, string> getName, Action<T, string> setName, List<T> items) {
            if (query.ContainsKey("s") || query.ContainsKey("sh")) {
                var sSearches = query.ContainsKey("s") ? query["s"].ToLower().Split('+') : new string[0];
                for (int i = 0; i < sSearches.Length; i++) {
                    if (SearchCorrections.ContainsKey(sSearches[i]))
                        sSearches[i] = SearchCorrections[sSearches[i]];
                }
                var shSearches = query.ContainsKey("sh") ? query["sh"].ToLower().Split('+') : new string[0];
                for (int i = 0; i < shSearches.Length; i++) {
                    if (SearchCorrections.ContainsKey(shSearches[i]))
                        shSearches[i] = SearchCorrections[shSearches[i]];
                }

                SortedList<double, T> newItems = new SortedList<double, T>();
                Dictionary<double, int> numRanks = new Dictionary<double, int>();
                foreach (T item in items) {
                    string name = getName(item).ToLower();
                    SearchHighlight[] highlighted = new SearchHighlight[name.Length];
                    double rank = 0;
                    foreach (string search in shSearches) {
                        int start = 0;
                        int i;
                        bool found = false;
                        while ((i = name.IndexOf(search, start)) >= 0) {
                            found = true;
                            start = i + 1;
                            for (int j = 0; j < search.Length; j++) {
                                highlighted[i + j] = SearchHighlight.SH;
                            }
                        }
                        if (found)
                            rank += 1000.0;
                    }
                    foreach (string search in sSearches) {
                        int start = 0;
                        int i;
                        bool found = false;
                        while ((i = name.IndexOf(search, start)) >= 0) {
                            found = true;
                            start = i + 1;
                            for (int j = 0; j < search.Length; j++) {
                                if (highlighted[i + j] == SearchHighlight.NA)
                                    highlighted[i + j] = SearchHighlight.S;
                            }
                        }
                        if (found)
                            rank += 1000.0;
                    }
                    rank += (highlighted.Count(x => x != SearchHighlight.NA) * 1000.0 / highlighted.Count()) - highlighted.Count();
                    if (rank > 0) {
                        if (query.ContainsKey("sh")) {
                            name = getName(item);
                            bool highlighting = false;
                            for (int i = highlighted.Length - 1; i >= 0; i--) {
                                if (highlighting) {
                                    if (highlighted[i] != SearchHighlight.SH) {
                                        name = name.Insert(i + 1, StartHighlight);
                                        highlighting = false;
                                    }
                                } else {
                                    if (highlighted[i] == SearchHighlight.SH) {
                                        name = name.Insert(i + 1, EndHighlight);
                                        highlighting = true;
                                    }
                                }
                            }
                            if (highlighting) {
                                name = name.Insert(0, StartHighlight);
                            }
                            setName(item, name);
                        }
                        if (numRanks.ContainsKey(rank)) {
                            newItems.Add(rank - numRanks[rank] / 10000.0, item);
                            numRanks[rank]++;
                        } else {
                            newItems.Add(rank, item);
                            numRanks[rank] = 1;
                        }
                    }
                }
                return newItems.Values.Reverse().ToList();
            } else {
                return items;
            }
        }
    }

    public enum SearchHighlight {
        NA = 0,
        S = 1,
        SH = 2,
    }

    public class LocationsDataHandler : PathHandler {
        public LocationsDataHandler() {
            Redirects.Add("id", new LocationsDataIdHandler());
            Redirects.Add("within", new LocationsDataWithinHandler());
            Redirects.Add("top", new LocationsDataTopHandler());
            Redirects.Add("all", new LocationsDataAllHandler());
        }

        public static IEnumerable<ThreadInfo> GetLocations(ThreadManager TM, string procedure, bool hideDescs, Dictionary<string, string> query, params SqlParameter[] parameters) {
            var currentThread = TM.CurrentThread;

            if (query.ContainsKey("version")) {
                double version;
                if (Double.TryParse(query["version"], out version)) {
                    if (version >= Program.LocationsVersion) {
                        throw new UpToDateException("Version is up to date.");
                    }
                }
            }

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, procedure, parameters);
            DataTable table = TM.GetResult<DataTable>(currentThread);
            var response = new LocationsResponse(Program.LocationsVersion);
            foreach (DataRow row in table.Rows) {
                var location = new Location(row, hideDescs);
                if (location.Id >= 0) {
                    if (location.IsMapArea()) {
                        yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetMapAreaCorners",
                            new SqlParameter("@maparea", location.Id));
                        DataTable cornerTable = TM.GetResult<DataTable>(currentThread);
                        foreach (DataRow cornerRow in cornerTable.Rows) {
                            location.MapArea.Corners.Add(new LatLong(cornerRow));
                        }
                    }
                    if (location.HasAltNames) {
                        yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationAltNames",
                            new SqlParameter("@location", location.Id));
                        location.AddAltNames(TM.GetResult<DataTable>(currentThread));
                    }
                    if (location.HasLinks && !hideDescs) {
                        yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationLinks",
                            new SqlParameter("@location", location.Id));
                        location.AddLinks(TM.GetResult<DataTable>(currentThread));
                    }
                    response.Locations.Add(location);
                }
            }

            response.Locations = LocationsHandler.ApplySearchFilter(query, location => location.Name, (location, name) => location.Name = name, response.Locations);

            yield return TM.Return(currentThread, new JsonResponse(response));
        }
    }

    public class LocationsDataIdHandler : PathHandler {
        public LocationsDataIdHandler() {
            IntRedirect = new LocationsDataIdIdHandler();
        }
    }

    public class LocationsDataIdIdHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            return LocationsDataHandler.GetLocations(TM, "spGetLocation", false, query, new SqlParameter("@location", (int)state));
        }
    }

    public class LocationsDataWithinHandler : PathHandler {
        public LocationsDataWithinHandler() {
            IntRedirect = new LocationsDataWithinIdHandler();
        }
    }

    public class LocationsDataWithinIdHandler : PathHandler {
        public LocationsDataWithinIdHandler() {
            Redirects.Add("notop", new LocationsDataWithinIdNoTopHandler());
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            return LocationsDataHandler.GetLocations(TM, "spGetLocationsWithin", false, query, new SqlParameter("@location", (int)state));
        }
    }

    public class LocationsDataWithinIdNoTopHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            return LocationsDataHandler.GetLocations(TM, "spGetLocationsWithinNoTop", false, query, new SqlParameter("@location", (int)state));
        }
    }

    public class LocationsDataTopHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            return LocationsDataHandler.GetLocations(TM, "spGetTopLocations", false, query);
        }
    }

    public class LocationsDataAllHandler : PathHandler {
        public LocationsDataAllHandler() {
            Redirects.Add("nodesc", new LocationsDataAllNoDescHandler());
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            return LocationsDataHandler.GetLocations(TM, "spGetAllLocations", false, query);
        }
    }

    public class LocationsDataAllNoDescHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            return LocationsDataHandler.GetLocations(TM, "spGetAllLocationsNoDesc", true, query);
        }
    }

    public class LocationsNamesHandler : PathHandler {
        public LocationsNamesHandler() {
            Redirects.Add("departable", new LocationsNamesDepartableHandler());
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            return GetNames(TM, "spGetLocationNames", query);
        }

        public static IEnumerable<ThreadInfo> GetNames(ThreadManager TM, string procedure, Dictionary<string, string> query) {
            var currentThread = TM.CurrentThread;

            if (query.ContainsKey("version")) {
                double version;
                if (Double.TryParse(query["version"], out version)) {
                    if (version >= Program.LocationsVersion) {
                        throw new UpToDateException("Version is up to date.");
                    }
                }
            }

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, procedure);
            var table = TM.GetResult<DataTable>(currentThread);
            var response = new LocationNamesResponse(Program.LocationsVersion);

            foreach (DataRow row in table.Rows) {
                var location = new LocationName(row);
                if (location.Id >= 0)
                    response.Names.Add(location);
            }

            response.Names = LocationsHandler.ApplySearchFilter(query, locName => locName.Name, (locName, name) => locName.Name = name, response.Names);

            yield return TM.Return(currentThread, new JsonResponse(response));
        }
    }

    public class LocationsNamesDepartableHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            return LocationsNamesHandler.GetNames(TM, "spGetLocationNamesDepartable", query);
        }
    }

    public class LocationsDescHandler : PathHandler {
        public LocationsDescHandler() {
            IntRedirect = new LocationsDescIdHandler();
        }
    }

    // Need to add links to response
    public class LocationsDescIdHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationDesc", new SqlParameter("@location", (int)state));
            var table = TM.GetResult<DataTable>(currentThread);
            foreach (DataRow row in table.Rows) {
                yield return TM.Return(currentThread, new JsonResponse(new LocationDescResponse(row)));
            }
            throw new BadRequestException("Could not find location with ID {0}.", state);
        }
    }
}
