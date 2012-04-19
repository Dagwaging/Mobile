using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace RHITMobileTest {
    [TestClass]
    public class DirectionsTests {
        public Path DirectionsBranch;

        public DirectionsTests() {
            DirectionsBranch = new Branch("directions", BadRequestTest, true, true, true, true) {
                new Branch("fromloc", BadRequestTest, true, true, false, true) {
                    //new Branch("
                },
                new Branch("status", BadRequestTest, true, true, false, true) {

                },
            };
        }

        public void BadRequestTest(string path) {
            HttpStatusCode code;
            string message;
            var response = PathTester.MakeRequest<RHITMobile.DirectionsResponse>(path, out code, out message);

            Assert.AreEqual(HttpStatusCode.BadRequest, code, "Status code was not BadRequest for path: {0}", path);
        }

        public void PartialDirectionsResponseTest(string path) {

        }

        public void FullDirectionsResponseTest(string path) {

        }

        [TestMethod]
        public void TestMethod1() {
        }
    }
}
