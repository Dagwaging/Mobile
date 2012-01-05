using System;
using System.Device.Location;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Rhit.Applications.ViewModel.Models;
using System.Windows.Media;

namespace Rhit.Applications.View.Views {
    /// \ingroup pages
    public partial class MapPage : PhoneApplicationPage {
        //private GeoCoordinateWatcher _userLocationWatcher;

        public MapPage() {
            InitializeComponent();

            ViewModel = new MainViewModel(MyMap);
            DataContext = ViewModel;
        }

        public MainViewModel ViewModel { get; set; }

        private GeoCoordinateWatcher UserWatcher { get; set; }

        #region Event Handlers
        //Note: Would use command binding for the appbar, but it was not available during development and wasn't worth it to implement it
        private void Settings_Click(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
        }

        //NOTE: Using System.Windows.Input absolute path because Microsoft.Phone.Controls also has a GestureEventArgs
        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e) {
            ViewModel.IgnoreEvent(e.GetPosition(ViewModel.Map.MapControl));
            NavigationService.Navigate(new Uri("/Views/InfoPage.xaml", UriKind.Relative));
        }

        private void QuickList_Click(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/Views/QuickListPage.xaml", UriKind.Relative));
        }

        private void Search_Click(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/Views/SearchPage.xaml", UriKind.Relative));
        }

        private void GotoRhit_Click(object sender, EventArgs e) {
            ViewModel.GotoRhit();
        }

        private void GotoUser_Click(object sender, EventArgs e) {
            ViewModel.GotoUser();
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            ViewModel.GotoCurrentLocation();
        }
    }
}