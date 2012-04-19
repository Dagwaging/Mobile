using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace RHITMobileTest {
    [TestClass]
    public class ServicesTests {
        public Path ServicesBranch;

        public ServicesTests() {
            ServicesBranch = new Branch("services", BadRequestTest, false, true, true, true) {
                new EndOfPath(CampusServicesResponseTest),
            };
        }

        public void BadRequestTest(string path) {
            HttpStatusCode code;
            string message;
            var response = PathTester.MakeRequest<RHITMobile.CampusServicesResponse>(path, out code, out message);

            Assert.AreEqual(HttpStatusCode.BadRequest, code, "Status code was not BadRequest for path: {0}", path);
        }

        public void CampusServicesResponseTest(string path) {
            HttpStatusCode code;
            string message;

            // Normal path
            var response = PathTester.MakeRequest<RHITMobile.CampusServicesResponse>(path, out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsNotNull(response.ServicesRoot, "Root is null: {0}", path);
            Assert.IsNotNull(response.ServicesRoot.Children, "Root's children is null: {0}", path);
            Assert.IsTrue(response.ServicesRoot.Children.Any(), "No categories under Root: {0}", path);
            double version = response.Version;

            // With current version
            response = PathTester.MakeRequest<RHITMobile.CampusServicesResponse>(path + "?version=" + version, out code, out message);

            Assert.AreEqual(HttpStatusCode.BadRequest, code, "Status code was not NoContent for the current version of path: {0}", path);

            // With old version
            response = PathTester.MakeRequest<RHITMobile.CampusServicesResponse>(path + "?version=" + (version / 2), out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsNotNull(response.ServicesRoot, "Root is null: {0}", path);
            Assert.IsNotNull(response.ServicesRoot.Children, "Root's children is null: {0}", path);
            Assert.IsTrue(response.ServicesRoot.Children.Any(), "No categories under Root: {0}", path);
        }

        [TestMethod]
        public void ServicesTest() {
            PathTester.RunTests(ServicesBranch);
        }
    }
}
