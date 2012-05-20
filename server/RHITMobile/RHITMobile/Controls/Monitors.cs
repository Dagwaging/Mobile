using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace RHITMobile {
    public static class Monitors {
        public static IEnumerable<ThreadInfo> SetPartitionAndLocationBoundaries(ThreadManager TM) {
            var currentThread = TM.CurrentThread;

            while (true) {
                yield return TM.Sleep(currentThread, 1000*60*60*23);

                yield return TM.Await(currentThread, AdminPathDataHandler.GetPathData(TM));
                var pathData = TM.GetResult<PathDataResponse>(currentThread);

                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetAllLocationsNoDesc");
                Dictionary<int, int> locationParent = new Dictionary<int, int>();
                foreach (DataRow row in TM.GetResult<DataTable>(currentThread).Rows) {
                    if (!row.IsNull("parent"))
                        locationParent[(int)row["id"]] = (int)row["parent"];
                }

                using (var connection = new SqlConnection(Program.ConnectionString)) {
                    connection.Open();
                    var transaction = connection.BeginTransaction("Updating Boundaries");
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction;

                    command.CommandText = "spDeleteBoundaries";
                    command.ExecuteNonQuery();

                    foreach (var n in pathData.Nodes) {
                        var locs = GetLocationsAbove(n.Location, locationParent);
                        var parts = new SortedSet<int>();
                        var locBoundaries = new SortedSet<int>();
                        foreach (var p in pathData.Paths.Where(p => p.Node1 == n.Id || p.Node2 == n.Id)) {
                            parts.Add(p.Partition);
                            int n2id = p.Node1 == n.Id ? p.Node2 : p.Node1;
                            var n2 = pathData.Nodes.Where(node => node.Id == n2id).First();
                            locBoundaries.UnionWith(locs.Except(GetLocationsAbove(n2.Location, locationParent)));
                        }
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("node", n.Id);
                        var partParam = new SqlParameter("partition", 0);
                        var locParam = new SqlParameter("location", 0);
                        command.Parameters.Add(partParam);
                        command.Parameters.Add(locParam);
                        foreach (int part in parts) {
                            partParam.Value = part;
                            if (parts.Count > 1) {
                                command.Parameters.Remove(locParam);
                                command.CommandText = "spAddPartitionBoundary";
                                command.ExecuteNonQuery();
                                command.Parameters.Add(locParam);
                            }

                            command.CommandText = "spAddLocationBoundary";
                            foreach (int loc in locBoundaries) {
                                locParam.Value = loc;
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        public static SortedSet<int> GetLocationsAbove(int? loc, Dictionary<int, int> locationParent) {
            if (!loc.HasValue) return new SortedSet<int>();
            var result = new SortedSet<int>();
            int c = loc.Value;
            result.Add(c);
            while (locationParent.ContainsKey(c)) {
                c = locationParent[c];
                result.Add(c);
            }
            return result;
        }
    }
}
