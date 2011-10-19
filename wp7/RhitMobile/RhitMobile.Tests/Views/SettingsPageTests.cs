using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhitMobile.ObjectModel;

namespace RhitMobile.Tests.Views {
    [TestClass]
    public class SettingsPageTests {

        private SettingsPage Page { get; set; }

        [TestInitialize]
        public void SetUp() {
            Page = new SettingsPage();
            Assert.IsNotNull(Page);
            Page.LoadSettings();
        }

        [TestCleanup]
        public void CleanUp() {

        }

        [TestMethod]
        [Description("")]
        public void TileSourceTest() {
            RhitMapView.Instance.ChangeTileSource("Bing", "Aerial");
            Page.LoadSettings();
            Assert.IsNotNull(Page.mapSourcePicker.SelectedItem);
            Assert.AreEqual("Bing", ((ListPickerObject) Page.mapSourcePicker.SelectedItem).Name);
        }
    }
}
