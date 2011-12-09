using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Rhit.Applications.ViewModel.Models;

namespace Rhit.Applications.View.Views {
    /// \ingroup pages
    public partial class MapPage : PhoneApplicationPage {
        //#region Private Fields
        //private GeoCoordinateWatcher _userLocationWatcher;
        //private ApplicationBar _appBar;
        //private TextBlock _debugText;
        //#endregion

        public MapPage() {
            InitializeComponent();

            ViewModel = new MainViewModel(MyMap);
            DataContext = ViewModel;

            Loaded += Page_Loaded;

            //LoadMap();

            //InitGeoCordinateWatcher();
        }

        public MainViewModel ViewModel { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e) {

        }

        private void Settings_Click(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
        }

        //NOTE: Using System.Windows.Input absolute path because Microsoft.Phone.Controls also has a GestureEventArgs
        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e) {
            ViewModel.IgnoreEvent(e.GetPosition(ViewModel.Map.MapControl));
            NavigationService.Navigate(new Uri("/Views/InfoPage.xaml", UriKind.Relative));
        }


//        #region Initializers
//        private void LoadMap() {
//            Map map = RhitMap.Instance.Map;
//            map.Margin = new System.Windows.Thickness(0, 0, 0, 0);
//            map.Mode = new MercatorMode();
//            map.CacheMode = new BitmapCache();
//            map.LogoVisibility = System.Windows.Visibility.Collapsed;
//            map.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
//            map.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
//            map.MapZoom += new EventHandler<MapZoomEventArgs>(Map_MapZoom);
//        }


//        void InitGeoCordinateWatcher() {
//            _userLocationWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);

//            _userLocationWatcher.MovementThreshold = 10.0f;

//            _userLocationWatcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(Watcher_StatusChanged);
//            _userLocationWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(Watcher_PositionChanged);

//            new Thread(bgWatcherUpdate).Start();
//        }
//        #endregion


//        void Map_MapZoom(object sender, MapZoomEventArgs e) {
//            RhitMap.Instance.DebugText = "Zoom: " + RhitMap.Instance.Map.ZoomLevel.ToString();
//        }


//        private void SelectedPushpin_MouseLeftButtonUp(object sender, PushpinEventArgs e) {
//            NavigationService.Navigate(new Uri("/InfoPage.xaml", UriKind.Relative));
//        }


//        #region AppBar Button Event Handlers
//        void MeButton_Click(object sender, EventArgs e) {
//            RhitMap.Instance.GoToUserLocation();
//        }

//        void RhitButton_Click(object sender, EventArgs e) {
//            RhitMap.Instance.GoToRhit();
//        }


//        #region AppBar MenuItem Event Handlers
//        void Settings_Click(object sender, EventArgs e) {
//            GoToSettings();
//        }


//        #region GeoCoordinateWatcher Event Handlers
//        void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e) {
//            //switch(e.Status) {
//            //    case GeoPositionStatus.Disabled:
//            //        if(watcher.Permission == GeoPositionPermission.Denied)
//            //            statusTextBlock.Text = "You have disabled Location Service.";
//            //        else
//            //            statusTextBlock.Text = "Location Service is not functioning on this device.";
//            //        break;
//            //    case GeoPositionStatus.Initializing:
//            //        statusTextBlock.Text = "Location Service is retrieving data...";
//            //        break;
//            //    case GeoPositionStatus.NoData:
//            //        statusTextBlock.Text = "Location data is not available.";
//            //        break;
//            //    case GeoPositionStatus.Ready:
//            //        statusTextBlock.Text = "Location data is available.";
//            //        break;
//            //}
//        }

//        void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e) {
//            GeoCoordinate coordinate = e.Position.Location;
//            //string accuracy = "0.0000000000000";
//            //statusTextBlock.Text = "Updated: " + coordinate.Latitude.ToString(accuracy) + ", " + coordinate.Longitude.ToString(accuracy);
//            RhitMap.Instance.User.Location = e.Position.Location;
//        }

//        void bgWatcherUpdate() {
//            _userLocationWatcher.TryStart(true, TimeSpan.FromMilliseconds(60000));
//        }
//        #endregion


//        public void GoToSettings() {
//            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
//        }

//        public void GoToQuikList() {
//            NavigationService.Navigate(new Uri("/QuickListPage.xaml", UriKind.Relative));
//        }

//        protected override void OnNavigatedTo(NavigationEventArgs e) {
//            LoadData();
//        }

//        protected override void OnNavigatedFrom(NavigationEventArgs e) {
//            RhitMap.Instance.StoreData();
//            ContentPanel.Children.Remove(RhitMap.Instance.Map);
//        }

        

//        public void LoadData() {

//            List<RhitLocation> locations = DataCollector.Instance.GetMapAreas(Dispatcher);

//            if(!ContentPanel.Children.Contains(RhitMap.Instance.Map)) LoadMap();

//            if(locations != null) RhitMap.Instance.Outlines = locations;
//            //TODO: Display message if locations == null; "Map Data is Empty. Trying to download..."

//            fillGrid();

//            RhitMap.Instance.Select(RhitMap.Instance.SelectedLocation);
//        }
    }
}