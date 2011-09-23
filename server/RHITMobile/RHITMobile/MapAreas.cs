using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.Data.SqlClient;

namespace RHITMobile
{
    public static class MapAreas
    {
        public static IEnumerable<ulong> HandleMapAreasRequest(ThreadManager TM, IEnumerable<string> path, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;
            if (!path.Any())
            {
                if (query.ContainsKey("version"))
                {
                    double version;
                    if (Double.TryParse(query["version"], out version))
                    {
                        if (version < Program.ServerVersion)
                        {
                            yield return TM.Await(currentThread, GetMapAreas(TM));
                            yield return TM.Return(currentThread, new JsonResponse(TM.GetResult<JsonObject>()));
                        }
                        else
                        {
                            yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.NoContent));
                        }
                    }
                    else
                    {
                        yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
                    }
                }
                else
                {
                    yield return TM.Await(currentThread, GetMapAreas(TM));
                    yield return TM.Return(currentThread, new JsonResponse(TM.GetResult<JsonObject>()));
                }
            }
            else
            {
                yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
            }
        }

        private static IEnumerable<ulong> GetMapAreas(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetMapAreas");
            var table = TM.GetResult<DataTable>();
            var response = new MapAreasResponse(Program.ServerVersion);
            foreach (DataRow row in table.Rows)
            {
                var mapArea = new MapArea(row);
                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetMapAreaCorners",
                    new SqlParameter("@maparea", mapArea.Id));
                var cornerTable = TM.GetResult<DataTable>();
                foreach (DataRow cornerRow in cornerTable.Rows)
                {
                    mapArea.Corners.Add(new LatLong(cornerRow));
                }
                response.Areas.Add(mapArea);
            }
            yield return TM.Return(currentThread, response);
        }
    }
}
