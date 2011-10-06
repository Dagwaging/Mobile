using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Data;
using System.Data.SqlClient;

namespace RHITMobile
{
    public class LocationsHandler : PathHandler
    {
        public LocationsHandler()
        {
            Redirects.Add("data", new LocationsDataHandler());
            Redirects.Add("names", new LocationsNamesHandler());
            Redirects.Add("desc", new LocationsDescHandler());
        }
    }

    public class LocationsDataHandler : PathHandler
    {
        public LocationsDataHandler()
        {
            Redirects.Add("id", new LocationsDataIdHandler());
            Redirects.Add("within", new LocationsDataWithinHandler());
            Redirects.Add("top", new LocationsDataTopHandler());
            Redirects.Add("all", new LocationsDataAllHandler());
        }

        public static IEnumerable<ThreadInfo> GetLocations(ThreadManager TM, string procedure, bool hideDescs, Dictionary<string, string> query, params SqlParameter[] parameters)
        {
            var currentThread = TM.CurrentThread;

            if (query.ContainsKey("version"))
            {
                double version;
                if (Double.TryParse(query["version"], out version))
                {
                    if (version >= Program.ServerVersion)
                    {
                        yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.NoContent));
                    }
                }
            }

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, procedure, parameters);
            DataTable table =  TM.GetResult<DataTable>(currentThread);
            var response = new LocationsResponse(Program.ServerVersion);
            foreach (DataRow row in table.Rows)
            {
                var location = new Location(row, hideDescs);
                if (location.IsMapArea())
                {
                    yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetMapAreaCorners",
                        new SqlParameter("@maparea", location.Id));
                    DataTable cornerTable = null;
                    cornerTable = TM.GetResult<DataTable>(currentThread);
                    foreach (DataRow cornerRow in cornerTable.Rows)
                    {
                        location.MapArea.Corners.Add(new LatLong(cornerRow));
                    }
                }
                response.Locations.Add(location);
            }
            yield return TM.Return(currentThread, new JsonResponse(response));
        }
    }

    public class LocationsDataIdHandler : PathHandler
    {
        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, IEnumerable<string> path, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;

            int location;
            if (!Int32.TryParse(path.First(), out location) || path.Count() > 1)
            {
                yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
            }
            else
            {
                yield return TM.Await(currentThread, LocationsDataHandler.GetLocations(TM, "spGetLocation", false, query, new SqlParameter("@location", location)));
                yield return TM.Return(currentThread);
            }
        }
    }

    public class LocationsDataWithinHandler : PathHandler
    {
        private PathHandler _noTop = new LocationsDataWithinNoTopHandler();

        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, IEnumerable<string> path, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;

            int location;
            if (!Int32.TryParse(path.First(), out location))
            {
                yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
            }
            else
            {
                var newpath = path.Skip(1);
                if (newpath.Any() && newpath.First() == "notop")
                {
                    query.Add("?id?", location.ToString());
                    yield return TM.Await(currentThread, _noTop.HandlePath(TM, newpath.Skip(1), query));
                }
                else
                {
                    yield return TM.Await(currentThread, LocationsDataHandler.GetLocations(TM, "spGetLocationsWithin", false, query, new SqlParameter("@location", location)));
                }
                yield return TM.Return(currentThread);
            }
        }
    }

    public class LocationsDataWithinNoTopHandler : PathHandler
    {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query)
        {
            return LocationsDataHandler.GetLocations(TM, "spGetLocationsWithinNoTop", false, query, new SqlParameter("@location", Int32.Parse(query["?id?"])));
        }
    }

    public class LocationsDataTopHandler : PathHandler
    {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query)
        {
            return LocationsDataHandler.GetLocations(TM, "spGetTopLocations", false, query);
        }
    }

    public class LocationsDataAllHandler : PathHandler
    {
        public LocationsDataAllHandler()
        {
            Redirects.Add("nodesc", new LocationsDataAllNoDescHandler());
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query)
        {
            return LocationsDataHandler.GetLocations(TM, "spGetAllLocations", false, query);
        }
    }

    public class LocationsDataAllNoDescHandler : PathHandler
    {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query)
        {
            return LocationsDataHandler.GetLocations(TM, "spGetAllLocationsNoDesc", true, query);
        }
    }

    public class LocationsNamesHandler : PathHandler
    {
        public LocationsNamesHandler()
        {
            Redirects.Add("alts", new LocationsNamesAltsHandler());
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query)
        {
            return GetNames(TM, "spGetNames", query);
        }

        public static IEnumerable<ThreadInfo> GetNames(ThreadManager TM, string procedure, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;

            if (query.ContainsKey("version"))
            {
                double version;
                if (Double.TryParse(query["version"], out version))
                {
                    if (version >= Program.ServerVersion)
                    {
                        yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.NoContent));
                    }
                }
            }

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, procedure);
            var table = TM.GetResult<DataTable>(currentThread);
            var response = new LocationNamesResponse(Program.ServerVersion);
            foreach (DataRow row in table.Rows)
            {
                response.Names.Add(new LocationName(row));
            }
            yield return TM.Return(currentThread, new JsonResponse(response));
        }
    }

    public class LocationsNamesAltsHandler : PathHandler
    {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query)
        {
            return LocationsNamesHandler.GetNames(TM, "spGetNamesOnlyAlts", query);
        }
    }

    public class LocationsDescHandler : PathHandler
    {
        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, IEnumerable<string> path, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;

            int location;
            if (!Int32.TryParse(path.First(), out location) || path.Count() > 1)
            {
                yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
            }
            else
            {
                yield return TM.Await(currentThread, GetDescription(TM, location));
                yield return TM.Return(currentThread);
            }
        }

        public static IEnumerable<ThreadInfo> GetDescription(ThreadManager TM, int location)
        {
            var currentThread = TM.CurrentThread;

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetLocationDesc", new SqlParameter("@location", location));
            var table = TM.GetResult<DataTable>(currentThread);
            foreach (DataRow row in table.Rows)
            {
                yield return TM.Return(currentThread, new JsonResponse(new LocationDescResponse(row)));
            }
            yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
        }
    }
}
