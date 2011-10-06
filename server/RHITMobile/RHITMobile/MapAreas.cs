using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.Data.SqlClient;

namespace RHITMobile
{
    public class MapAreasHandler : PathHandler
    {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;

            if (query.ContainsKey("version"))
            {
                double version;
                if (Double.TryParse(query["version"], out version))
                {
                    if (version < Program.ServerVersion)
                    {
                        yield return TM.Await(currentThread, GetMapAreas(TM));
                        yield return TM.Return(currentThread);
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
                yield return TM.Return(currentThread);
            }
        }

        private static IEnumerable<ThreadInfo> GetMapAreas(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetMapAreas");
            bool exception = false;
            DataTable table = null;
            try
            {
                table = TM.GetResult<DataTable>(currentThread);
            }
            catch (SqlException)
            {
                exception = true;
            }
            if (!exception)
            {
                var response = new MapAreasResponse(Program.ServerVersion);
                foreach (DataRow row in table.Rows)
                {
                    var mapArea = new MapArea(row);
                    yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetMapAreaCorners",
                        new SqlParameter("@maparea", mapArea.Id));
                    DataTable cornerTable = null;
                    try
                    {
                        cornerTable = TM.GetResult<DataTable>(currentThread);
                    }
                    catch (SqlException)
                    {
                        exception = true;
                        break;
                    }
                    foreach (DataRow cornerRow in cornerTable.Rows)
                    {
                        mapArea.Corners.Add(new LatLong(cornerRow));
                    }
                    response.Areas.Add(mapArea);
                }
                if (!exception)
                {
                    yield return TM.Return(currentThread, new JsonResponse(response));
                }
            }
            yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.ServiceUnavailable));
        }
    }
}
