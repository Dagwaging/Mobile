using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace RHITMobileTest {
    [TestClass]
    public class RootTests {
        public Path Root;

        public RootTests() {
            Root = new EndOfPath(VersionResponseTest);
        }

        public void VersionResponseTest(string path) {
            HttpStatusCode code;
            string message;

            // Normal path
            var response = PathTester.MakeRequest<RHITMobile.VersionResponse>(path, out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsTrue(response.LocationsVersion > 0, "LocationsVersion is not positive: {0}", path);
            Assert.IsTrue(response.ServicesVersion > 0, "ServicesVersion is not positive: {0}", path);
            Assert.IsTrue(response.TagsVersion > 0, "TagsVersion is not positive: {0}", path);
        }

        [TestMethod]
        public void RootTest() {
            PathTester.RunTests(Root);
        }
    }
}
