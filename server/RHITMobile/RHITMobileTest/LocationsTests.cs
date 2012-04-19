using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace RHITMobileTest {
    [TestClass]
    public class LocationsTests {
        public Path LocationsBranch;

        public LocationsTests() {
            LocationsBranch = new Branch("locations", BadRequestTest, true, true, true, true) {
                new Branch("data", BadRequestTest, true, true, true, true) {
                    new Branch("all", BadRequestTest, false, true, true, true) {
                        new EndOfPath(LocationsResponseTest),
                        new Branch("nodesc", BadRequestTest, false, true, true, true) {
                            new EndOfPath(LocationsResponseTest),
                        },
                    },
                    new Branch("top", BadRequestTest, false, true, true, true) {
                        new EndOfPath(LocationsResponseTest),
                    },
                    new Branch("id", BadRequestTest, true, true, false, true) {
                        new Branch("1100000", BadRequestTest, false, true, true, true) {
                            new EndOfPath(LocationsResponseTest),
                        },
                    },
                    new Branch("within", BadRequestTest, true, true, false, true) {
                        new Branch("1100000", BadRequestTest, false, true, true, true) {
                            new EndOfPath(LocationsResponseTest),
                            new Branch("notop", BadRequestTest, false, true, true, true) {
                                new EndOfPath(LocationsResponseTest),
                            },
                        },
                    },
                },
                new Branch("desc", BadRequestTest, true, true, false, true) {
                    new Branch("1100000", BadRequestTest, false, true, true, true) {
                        new EndOfPath(DescriptionResponseTest),
                    },
                },
                new Branch("names", BadRequestTest, false, true, true, true) {
                    new EndOfPath(LocationNamesResponse),
                    new Branch("departable", BadRequestTest, false, true, true, true) {
                        new EndOfPath(LocationNamesResponse),
                    },
                },
            };
        }

        public void BadRequestTest(string path) {
            HttpStatusCode code;
            string message;
            var response = PathTester.MakeRequest<RHITMobile.LocationsResponse>(path, out code, out message);

            Assert.AreEqual(HttpStatusCode.BadRequest, code, "Status code was not BadRequest for path: {0}", path);
        }

        public void LocationsResponseTest(string path) {
            HttpStatusCode code;
            string message;

            // Normal path
            var response = PathTester.MakeRequest<RHITMobile.LocationsResponse>(path, out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsTrue(response.Locations.Any(), "No locations returned for path: {0}", path);
            int numLocations = response.Locations.Count;
            string firstLocName = response.Locations.First().Name;
            int firstLocId = response.Locations.First().Id;
            double version = response.Version;

            // With current version
            response = PathTester.MakeRequest<RHITMobile.LocationsResponse>(path + "?version=" + version, out code, out message);

            Assert.AreEqual(HttpStatusCode.BadRequest, code, "Status code was not NoContent for the current version of path: {0}", path);

            // With old version
            response = PathTester.MakeRequest<RHITMobile.LocationsResponse>(path + "?version=" + (version / 2), out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsTrue(response.Locations.Any(), "No locations returned for path: {0}", path);

            // With search
            response = PathTester.MakeRequest<RHITMobile.LocationsResponse>(path + "?s=" + firstLocName[0], out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsTrue(response.Locations.Count <= numLocations, "Search resulted in more results than the original: {0}", path);
            Assert.IsTrue(response.Locations.Any(loc => loc.Name == firstLocName), "Search eliminated a necessary result: {0}", path);

            // With highlighted search
            response = PathTester.MakeRequest<RHITMobile.LocationsResponse>(path + "?sh=" + firstLocName[0], out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsTrue(response.Locations.Count <= numLocations, "Search resulted in more results than the original: {0}", path);
            Assert.IsTrue(response.Locations.Any(loc => loc.Id == firstLocId), "Search eliminated a necessary result: {0}", path);
            Assert.IsFalse(response.Locations.Any(loc => loc.Name == firstLocName), "Search did not highlight the name: {0}", path);
        }

        public void DescriptionResponseTest(string path) {
            HttpStatusCode code;
            string message;

            // Normal path
            var response = PathTester.MakeRequest<RHITMobile.LocationDescResponse>(path, out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsFalse(String.IsNullOrWhiteSpace(response.Description), "Description is empty: {0}", path);
            Assert.IsNotNull(response.Links, "Links is null: {0}", path);
        }

        public void LocationNamesResponse(string path) {
            HttpStatusCode code;
            string message;

            // Normal path
            var response = PathTester.MakeRequest<RHITMobile.LocationNamesResponse>(path, out code, out message);
            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsTrue(response.Names.Any(), "No names returned for path: {0}", path);
            int numLocations = response.Names.Count;
            string firstLocName = response.Names.First().Name;
            int firstLocId = response.Names.First().Id;
            double version = response.Version;

            // With current version
            response = PathTester.MakeRequest<RHITMobile.LocationNamesResponse>(path + "?version=" + version, out code, out message);

            Assert.AreEqual(HttpStatusCode.BadRequest, code, "Status code was not NoContent for the current version of path: {0}", path);

            // With old version
            response = PathTester.MakeRequest<RHITMobile.LocationNamesResponse>(path + "?version=" + (version / 2), out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsTrue(response.Names.Any(), "No names returned for path: {0}", path);

            // With search
            response = PathTester.MakeRequest<RHITMobile.LocationNamesResponse>(path + "?s=" + firstLocName[0], out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsTrue(response.Names.Count <= numLocations, "Search resulted in more results than the original: {0}", path);
            Assert.IsTrue(response.Names.Any(loc => loc.Name == firstLocName), "Search eliminated a necessary result: {0}", path);

            // With highlighted search
            response = PathTester.MakeRequest<RHITMobile.LocationNamesResponse>(path + "?sh=" + firstLocName[0], out code, out message);

            Assert.AreEqual(HttpStatusCode.OK, code, "Status code was not OK for path: {0}", path);
            Assert.IsTrue(response.Names.Count <= numLocations, "Search resulted in more results than the original: {0}", path);
            Assert.IsTrue(response.Names.Any(loc => loc.Id == firstLocId), "Search eliminated a necessary result: {0}", path);
            Assert.IsFalse(response.Names.Any(loc => loc.Name == firstLocName), "Search did not highlight the name: {0}", path);
        }

        [TestMethod]
        public void LocationsTest() {
            PathTester.RunTests(LocationsBranch);
        }
    }
}
