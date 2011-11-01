using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections;
using Microsoft.Silverlight.Testing;
using Microsoft.Silverlight.Testing.Client;
using RhitMobile.ObjectModel;
using RhitMobile.Services;
using System.Collections.Generic;

namespace RhitMobile.Tests.Views {
    [TestClass]
    
    public class MapPageTests {
        private MapPage Page { get; set; }

        [TestInitialize]
        public void SetUp() {
            Page = new MapPage();
            Assert.IsNotNull(Page);
            DataCollector.Instance.BaseAddress = "http://mobilewin.csse.rose-hulman.edu:5600";
            Page.LoadData();
            //TODO: Utilize data from service
            // Location data should be loaded in above, but tests don't wait for data
            List<RhitLocation> locations = DataCollector.Instance.GetAllLocations(App.Current.RootVisual.Dispatcher);
            if(locations != null) RhitMapView.Instance.Outlines = locations;
            else RhitMapView.Instance.Outlines = Locations.ALL;
        }

        [TestCleanup]
        public void CleanUp() {
            Page.ContentPanel.Children.Clear();
        }

        [TestMethod]
        [Description("Checks that the app bar menu items have been added.")]
        public void AppBarMenuItemTest() {
            IList menuItems = Page.ApplicationBar.MenuItems;
            Assert.AreEqual(2, menuItems.Count);
        }

        [TestMethod]
        [Description("Tests/Simulates a user tapping an outline on the map.")]
        public void OutlineTappedTest() {
            Assert.IsNotNull(RhitMapView.Instance.Outlines);
            Assert.IsTrue(RhitMapView.Instance.Outlines.Count > 0, "There are no map locations loaded.");
            RhitLocation firstLocation = RhitMapView.Instance.Outlines[0];
            RhitLocation secondLocation = null;
            RhitMapView.Instance.Select(firstLocation);
            foreach(RhitLocation location in RhitMapView.Instance.Outlines)
                if (location.Label == firstLocation.Label) {
                    secondLocation = location;
                    break;
                }
            Assert.IsNotNull(secondLocation, "The tapped location cannot be found.");
            Assert.AreEqual(secondLocation.Center, RhitMapView.Instance.SelectedPushpin.Location,
                "The information pushpin was not positioned correctly.");
            Assert.AreNotEqual(secondLocation.OutLine.Fill, Colors.Transparent, "The location is still hidden.");
            Assert.AreNotEqual(secondLocation.OutLine.Stroke, Colors.Transparent, "The location's stroke is still hidden.");
        }
    }
}
