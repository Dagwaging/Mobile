using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace RHITMobileTest {
    [TestClass]
    public class AdminTests {
        public Path AdminPath;

        public AdminTests() {
            AdminPath = new Branch("admin", BadRequestTest, false, false, false, false) {
                new Branch("authenticate", BadRequestTest, false, false, false, false) {
                    new Branch("admin", BadRequestTest, false, true, false, false),
                }
            };
        }

        public void BadRequestTest(string path) {
            HttpStatusCode code;
            string message;
            var response = PathTester.MakeRequest<RHITMobile.LocationsResponse>(path, out code, out message);

            Assert.AreEqual(HttpStatusCode.BadRequest, code, "Status code was not BadRequest for path: {0}", path);
        }

        [TestMethod]
        public void AdminTest() {
            PathTester.RunTests(AdminPath);
        }
    }
}
