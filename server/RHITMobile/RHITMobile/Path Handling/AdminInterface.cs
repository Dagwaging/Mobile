using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.Data.SqlClient;
using System.Collections.Specialized;

namespace RHITMobile {
    public class AdminHandler : PathHandler {
        public static Dictionary<Guid, SqlLoginData> Logins = new Dictionary<Guid, SqlLoginData>();

        public AdminHandler() {
            Redirects.Add("authenticate", new AdminAuthenticateHandler());
            Redirects.Add("storedproc", new AdminStoredProcHandler());
            Redirects.Add("updateversion", new AdminUpdateVersionHandler());
            Redirects.Add("scriptdb", new DatabaseScripter());
            Redirects.Add("pathdata", new AdminPathDataHandler());
            Redirects.Add("filehost", new FileHostHandler());
            UnknownRedirect = new AdminTokenHandler(); //TODO: Remove
        }

        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, string path, object state) { //TODO: Remove
            var currentThread = TM.CurrentThread;

            yield return TM.Return(currentThread, path);
        }

        public static IEnumerable<ThreadInfo> VerifyToken(ThreadManager TM, NameValueCollection headers) {
            var currentThread = TM.CurrentThread;

            string token = headers["Auth-Token"];
            if (token == null)
                throw new UnauthorizedException(currentThread, "An authentication token is required for this request.");

            Guid id = new Guid();
            try {
                id = new Guid(token);
            } catch (Exception e) {
                throw new UnauthorizedException(currentThread, "Authentication token is invalid.");
            }
            if (!AdminHandler.Logins.ContainsKey(id))
                throw new UnauthorizedException(currentThread, "Authentication token is invalid.");
            if (AdminHandler.Logins[id].Expiration < DateTime.Now)
                throw new UnauthorizedException(currentThread, "Authentication token has expired.");

            yield return TM.Return(currentThread, AdminHandler.Logins[id]);
        }

        public static IEnumerable<ThreadInfo> DeleteExpiredLogins(ThreadManager TM) {
            var currentThread = TM.CurrentThread;
            while (true) {
                // Sleep for an hour
                yield return TM.Sleep(currentThread, 3600000);
                var expired = Logins.Where(kvp => kvp.Value.Expiration < DateTime.Now);
                foreach (var kvp in expired) {
                    Logins.Remove(kvp.Key);
                }
            }
        }
    }

    public class AdminAuthenticateHandler : PathHandler {
        public AdminAuthenticateHandler() {
            UnknownRedirect = new AdminAuthenticateUsernameHandler(); //TODO: Remove
        }

        public override IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            var currentThread = TM.CurrentThread;

            string username = headers["Login-Username"];
            string password = headers["Login-Password"];

            if (username == null || password == null)
                yield return TM.Return(currentThread, "OK"); //TODO: Change to be a bad request

            yield return TM.MakeDbCall(currentThread, Program.GetConnectionString(username, password), "spTestConnection");
            bool success = true;
            try {
                var result = TM.GetResult<DataTable>(currentThread);
                if (result.Rows.Count != 1 || (int)result.Rows[0][0] != 56) {
                    success = false;
                }
            } catch {
                success = false;
            }

            if (success) {
                var alreadyLoggedIn = AdminHandler.Logins.Where(kvp => kvp.Value.Username == username);
                if (alreadyLoggedIn.Any())
                    AdminHandler.Logins.Remove(alreadyLoggedIn.First().Key);
                Guid id = Guid.NewGuid();
                while (AdminHandler.Logins.ContainsKey(id))
                    id = Guid.NewGuid();
                var loginData = new SqlLoginData(username, password);
                AdminHandler.Logins[id] = loginData;
                yield return TM.Return(currentThread, new SAdminAuthResponse(loginData.Expiration, id));
            } else {
                throw new BadRequestException(currentThread, "SQL authentication failed.");
            }
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new JsonResponse((SAdminAuthResponse)state));
        }
    }

    public class AdminAuthenticateUsernameHandler : PathHandler { //TODO: Remove
        public AdminAuthenticateUsernameHandler() {
            UnknownRedirect = new AdminAuthenticatePasswordHandler();
        }

        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, string path, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new SqlLoginData((string)state, path));
        }
    }

    public class AdminAuthenticatePasswordHandler : PathHandler { //TODO: Remove
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;
            var loginData = (SqlLoginData)state;
            yield return TM.MakeDbCall(currentThread, Program.GetConnectionString(loginData.Username, loginData.Password), "spTestConnection");
            bool success = true;
            try {
                var result = TM.GetResult<DataTable>(currentThread);
                if (result.Rows.Count != 1 || (int)result.Rows[0][0] != 56) {
                    success = false;
                }
            } catch {
                success = false;
            }

            if (success) {
                var alreadyLoggedIn = AdminHandler.Logins.Where(kvp => kvp.Value.Username == loginData.Username);
                if (alreadyLoggedIn.Any())
                    AdminHandler.Logins.Remove(alreadyLoggedIn.First().Key);
                Guid id = Guid.NewGuid();
                while (AdminHandler.Logins.ContainsKey(id))
                    id = Guid.NewGuid();
                AdminHandler.Logins[id] = loginData;
                yield return TM.Return(currentThread, new JsonResponse(new SAdminAuthResponse(loginData.Expiration, id)));
            } else {
                throw new BadRequestException(currentThread, "SQL authentication failed.");
            }
        }
    }

    public class AdminTokenHandler : PathHandler { //TODO: Remove
        public AdminTokenHandler() {
            Redirects.Add("storedproc", new AdminStoredProcHandler());
            Redirects.Add("updateversion", new AdminUpdateVersionHandler());
            Redirects.Add("scriptdb", new DatabaseScripter());
            Redirects.Add("pathdata", new AdminPathDataHandler());
            Redirects.Add("filehost", new FileHostHandler());
        }

        public override IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            var currentThread = TM.CurrentThread;
            headers.Add("Auth-Token", (string)state);
            yield return TM.Return(currentThread, null);
        }
    }

    public class AdminStoredProcHandler : PathHandler {
        public AdminStoredProcHandler() {
            UnknownRedirect = new AdminStoredProcNameHandler();
        }

        public override IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            return AdminHandler.VerifyToken(TM, headers);
        }

        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, string path, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new SqlStoredProcData((SqlLoginData)state, path));
        }
    }

    public class AdminStoredProcNameHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;
            var storedProcData = (SqlStoredProcData)state;
            var parameters = new SqlParameter[query.Count];
            int i = 0;
            foreach (var kvp in query) {
                if (kvp.Value == "\0")
                    parameters[i] = new SqlParameter(kvp.Key, DBNull.Value);
                else
                    parameters[i] = new SqlParameter(kvp.Key, kvp.Value);
                i++;
            }
            yield return TM.MakeDbCall(currentThread, Program.GetConnectionString(storedProcData.LoginData.Username, storedProcData.LoginData.Password), storedProcData.StoredProcName, parameters);
            var response = new StoredProcedureResponse();
            try {
                var table = TM.GetResult<DataTable>(currentThread);
                foreach (DataColumn column in table.Columns) {
                    response.Columns.Add(column.ColumnName);
                }
                int columns = response.Columns.Count;
                foreach (DataRow row in table.Rows) {
                    var rowList = new List<string>();
                    for (int j = 0; j < columns; j++) {
                        rowList.Add(row.IsNull(j) ? null : row[j].ToString());
                    }
                    response.Table.Add(rowList);
                }
            } catch (Exception e) {
                throw new BadRequestException(currentThread, e.Message);
            }

            yield return TM.Return(currentThread, new JsonResponse(response));
        }
    }

    public class AdminUpdateVersionHandler : PathHandler {
        public override IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            return AdminHandler.VerifyToken(TM, headers);
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;
            double newLocations = Program.LocationsVersion;
            double newServices = Program.ServicesVersion;
            double newTags = Program.TagsVersion;
            if (query.ContainsKey("locations"))
                Double.TryParse(query["locations"], out newLocations);
            if (query.ContainsKey("services"))
                Double.TryParse(query["services"], out newServices);
            if (query.ContainsKey("tags"))
                Double.TryParse(query["tags"], out newTags);
            Program.WriteVersions(newLocations, newServices, newTags);
            yield return TM.Return(currentThread, new JsonResponse(new VersionResponse()));
        }
    }

    public class SqlLoginData {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime Expiration { get; set; }

        public SqlLoginData(string username, string password) {
            Username = username;
            Password = password;
            Expiration = DateTime.Now.AddDays(1);
        }
    }

    public class SqlStoredProcData {
        public SqlLoginData LoginData { get; set; }
        public string StoredProcName { get; set; }

        public SqlStoredProcData(SqlLoginData loginData, string name) {
            LoginData = loginData;
            StoredProcName = name;
        }
    }

    public class AdminPathDataHandler : PathHandler {
        public override IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            return AdminHandler.VerifyToken(TM, headers);
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;
            var result = new PathDataResponse(Program.LocationsVersion);
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetPaths");
            foreach (DataRow row in TM.GetResult<DataTable>(currentThread).Rows) {
                result.Paths.Add(new Path(row));
            }
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetNodes");
            foreach (DataRow row in TM.GetResult<DataTable>(currentThread).Rows) {
                result.Nodes.Add(new Node(row));
            }
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetPartitions");
            foreach (DataRow row in TM.GetResult<DataTable>(currentThread).Rows) {
                result.Partitions.Add(new Partition(row));
            }
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetPartitions");
            foreach (DataRow row in TM.GetResult<DataTable>(currentThread).Rows) {
                result.Partitions.Add(new Partition(row));
            }
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetDirectionMessages");
            foreach (DataRow row in TM.GetResult<DataTable>(currentThread).Rows) {
                result.Messages.Add(new DirectionMessage(row));
            }
            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetDirections");
            foreach (DataRow row in TM.GetResult<DataTable>(currentThread).Rows) {
                var direction = new Direction(row);
                yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetDirectionPaths", new SqlParameter("direction", direction.Id));
                foreach (DataRow pathRow in TM.GetResult<DataTable>(currentThread).Rows) {
                    direction.Paths.Add((int)pathRow["path"]);
                }
                result.Directions.Add(direction);
            }

            yield return TM.Return(currentThread, new JsonResponse(result));
        }
    }
}
