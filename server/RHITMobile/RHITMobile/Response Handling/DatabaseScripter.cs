using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using System.Collections.Specialized;

namespace RHITMobile {
    public class DatabaseScripter : SecurePathHandler {
        public override IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            return AdminHandler.VerifyToken(TM, headers);
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var now = DateTime.Now;
            return ScriptDatabase(TM, Program.ConnectionString, "MapData", String.Format("C:/DBBackup/Dropbox/MapData_Backup_{0:yyyy_MM_dd_HH_mm_ss}.sql", now), now, ((SqlLoginData)state).Username);
        }

        public IEnumerable<ThreadInfo> ScriptDatabase(ThreadManager TM, string connectionString, string dbName, string destination, DateTime start, string user) {
            var currentThread = TM.CurrentThread;

            yield return TM.StartNewThread(currentThread, () => {
                using (var connection = new SqlConnection(connectionString))
                using (var output = new StreamWriter(destination)) {
                    connection.Open();
                    var server = new Server(new ServerConnection(connection));
                    var database = server.Databases[dbName];
                    var scripter = new Scripter(server);
                    scripter.Options.ScriptSchema = true;
                    scripter.Options.ScriptData = true;
                    scripter.Options.ScriptDrops = false;
                    scripter.Options.WithDependencies = true;
                    scripter.Options.Indexes = true;
                    scripter.Options.DriAllConstraints = true;

                    // Not scripting: DdlTrigger, UnresolvedEntity
                    var urns = new List<Urn>();
                    urns.AddRange(database.UserDefinedFunctions.Cast<UserDefinedFunction>().Where(f => !f.IsSystemObject).Select(f => f.Urn));
                    foreach (UserDefinedFunction function in database.UserDefinedFunctions) {
                        if (!function.IsSystemObject)
                            urns.Add(function.Urn);
                        else break;
                    }
                    foreach (View view in database.Views) {
                        if (!view.IsSystemObject)
                            urns.Add(view.Urn);
                        else break;
                    }
                    foreach (Table table in database.Tables) {
                        if (!table.IsSystemObject)
                            urns.Add(table.Urn);
                        else break;
                    }
                    foreach (StoredProcedure proc in database.StoredProcedures) {
                        if (!proc.IsSystemObject)
                            urns.Add(proc.Urn);
                        else break;
                    }
                    foreach (Default def in database.Defaults) {
                        urns.Add(def.Urn);
                    }
                    foreach (Microsoft.SqlServer.Management.Smo.Rule rule in database.Rules) {
                        urns.Add(rule.Urn);
                    }
                    foreach (Trigger trigger in database.Triggers) {
                        if (!trigger.IsSystemObject)
                            urns.Add(trigger.Urn);
                        else break;
                    }
                    foreach (UserDefinedAggregate aggregate in database.UserDefinedAggregates) {
                        urns.Add(aggregate.Urn);
                    }
                    foreach (Synonym synonym in database.Synonyms) {
                        urns.Add(synonym.Urn);
                    }
                    foreach (UserDefinedDataType type in database.UserDefinedDataTypes) {
                        urns.Add(type.Urn);
                    }
                    foreach (XmlSchemaCollection xsc in database.XmlSchemaCollections) {
                        urns.Add(xsc.Urn);
                    }
                    foreach (UserDefinedType type in database.UserDefinedTypes) {
                        urns.Add(type.Urn);
                    }
                    foreach (UserDefinedTableType type in database.UserDefinedTableTypes) {
                        urns.Add(type.Urn);
                    }
                    foreach (PartitionScheme scheme in database.PartitionSchemes) {
                        urns.Add(scheme.Urn);
                    }
                    foreach (PartitionFunction function in database.PartitionFunctions) {
                        urns.Add(function.Urn);
                    }
                    foreach (PlanGuide guide in database.PlanGuides) {
                        urns.Add(guide.Urn);
                    }
                    foreach (SqlAssembly assembly in database.Assemblies) {
                        if (!assembly.IsSystemObject)
                            urns.Add(assembly.Urn);
                        else break;
                    }

                    output.WriteLine("-- Script generated at {0:MM/dd/yyyy HH:mm:ss} by {1} --", start, user);
                    var strings = scripter.EnumScript(urns.ToArray());
                    foreach (string s in strings) {
                        if (s.Contains("CREATE") || s.StartsWith("SET ANSI_NULLS"))
                            output.WriteLine("GO");
                        output.WriteLine(s);
                    }
                    output.WriteLine("-- Scripting complete --");

                    return "Successfully scripted database.";
                }});

            yield return TM.Return(currentThread, new JsonResponse(new MessageResponse(TM.GetResult<string>(currentThread))));
        }
    }
}
