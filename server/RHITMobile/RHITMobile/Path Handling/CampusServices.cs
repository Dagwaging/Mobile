﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;

namespace RHITMobile {
    public class ServicesHandler : PathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;

            if (query.ContainsKey("version")) {
                double version;
                if (Double.TryParse(query["version"], out version)) {
                    if (version >= Program.ServicesVersion) {
                        yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.NoContent));
                    }
                }
            }

            var response = new CampusServicesResponse(Program.ServicesVersion);
            var categories = new Dictionary<string, CampusServicesCategory>();
            categories.Add("", response.Root);

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetCampusServiceCategories");
            using (var table = TM.GetResult<DataTable>(currentThread)) {
                foreach (DataRow row in table.Rows) {
                    var category = new CampusServicesCategory(row);
                    categories[category.Parent ?? ""].Children.Add(category);
                    categories.Add(category.Name, category);
                }
            }

            yield return TM.MakeDbCall(currentThread, Program.ConnectionString, "spGetCampusServiceLinks");
            using (var table = TM.GetResult<DataTable>(currentThread)) {
                foreach (DataRow row in table.Rows) {
                    categories[(string)row["category"]].Links.Add(new HyperLink(row));
                }
            }

            yield return TM.Return(currentThread, new JsonResponse(response));
        }
    }
}
