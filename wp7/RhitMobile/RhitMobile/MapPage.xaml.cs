using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Core;
using Microsoft.Phone.Shell;
using RhitMobile.Events;
using RhitMobile.ObjectModel;
using RhitMobile.Services;
using Microsoft.Silverlight.Testing;

namespace RhitMobile {
    public partial class MapPage : PhoneApplicationPage {
        #region Private Fields
        private GeoCoordinateWatcher _userLocationWatcher;
        private ApplicationBar _appBar;
        private TextBlock _debugText;
        #endregion

        public MapPage() {
            InitializeComponent();
            Loaded += Page_Loaded;

            LoadMap();

            RhitMapView.Instance.OutlineTapped += new Events.OutlineEventHandler(Outline_Tapped);
            RhitMapView.Instance.PushpinTapped += new PushpinEventHandler(SelectedPushpin_MouseLeftButtonUp);
            RhitMapView.Instance.DebugTextChanged += new DebugEventHandler(DebugText_Changed);
            RhitMapView.Instance.DebugModeChanged += new DebugEventHandler(DebugMode_Changed);

            InitApplicationBar();
            InitGeoCordinateWatcher();
        }

        #region Initializers
        private void LoadMap() {
            Map map = RhitMapView.Instance.Map;
            map.Margin = new System.Windows.Thickness(0, 0, 0, 0);
            map.Mode = new MercatorMode();
            map.CacheMode = new BitmapCache();
            map.LogoVisibility = System.Windows.Visibility.Collapsed;
            map.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            map.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            map.MapZoom += new EventHandler<MapZoomEventArgs>(Map_MapZoom);
        }

        void InitApplicationBar() {
            _appBar = new ApplicationBar() {
                Opacity = 0.5,
                IsVisible = true,
                IsMenuEnabled = true
            };
            ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/Images/position.png", UriKind.Relative)) {
                Text = "Me",
            };
            ApplicationBarIconButton button2 = new ApplicationBarIconButton(new Uri("/Images/rose-hulman.png", UriKind.Relative)) {
                Text = "RHIT",
            };
            ApplicationBarIconButton button3 = new ApplicationBarIconButton(new Uri("/Images/search.png", UriKind.Relative)) {
                Text = "Directions",
            };
            button1.Click += new EventHandler(MeButton_Click);
            button2.Click += new EventHandler(RhitButton_Click);
            button3.Click += new EventHandler(SearchButton_Click);

            _appBar.Buttons.Add(button1);
            _appBar.Buttons.Add(button2);
            _appBar.Buttons.Add(button3);

            ApplicationBarMenuItem quickListMenuItem = new ApplicationBarMenuItem("Quick List");
            quickListMenuItem.Click += new EventHandler(POI_Click);
            _appBar.MenuItems.Add(quickListMenuItem);

            ApplicationBarMenuItem settingsMenuItem = new ApplicationBarMenuItem("Settings");
            settingsMenuItem.Click += new EventHandler(Settings_Click);
            _appBar.MenuItems.Add(settingsMenuItem);

            this.ApplicationBar = _appBar;
        }

        void InitGeoCordinateWatcher() {
            _userLocationWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);

            _userLocationWatcher.MovementThreshold = 10.0f;

            _userLocationWatcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(Watcher_StatusChanged);
            _userLocationWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(Watcher_PositionChanged);

            new Thread(bgWatcherUpdate).Start();
        }

        void InitTesting() {
            Content = UnitTestSystem.CreateTestPage();
            IMobileTestPage imtp = Content as IMobileTestPage;

            if(imtp != null) {
                BackKeyPress += (x, xe) => xe.Cancel = imtp.NavigateBack();
            }
        }
        #endregion

        #region Map/Service Event Handlers
        void DebugMode_Changed(object sender, DebugEventArgs e) {
            fillGrid();
        }

        void DebugText_Changed(object sender, DebugEventArgs e) {
            if (RhitMapView.Instance.InDebugMode)
                _debugText.Text = RhitMapView.Instance.DebugText;
        }

        void Map_MapZoom(object sender, MapZoomEventArgs e) {
            RhitMapView.Instance.DebugText = "Zoom: " + RhitMapView.Instance.Map.ZoomLevel.ToString();
        }

        private void Outline_Tapped(object sender, Events.OutlineEventArgs e) {
            if(e.Outline == null) return;
            RhitMapView.Instance.Select(e.Outline);
        }

        private void SelectedPushpin_MouseLeftButtonUp(object sender, PushpinEventArgs e) {
            NavigationService.Navigate(new Uri("/DescriptionPage.xaml", UriKind.Relative));
        }

        private void OnUpdateAvailable(object sender, ServiceEventArgs e) {
            RhitMapView.Instance.Outlines = DataCollector.Instance.GetMapAreas(Dispatcher);
            DataCollector.Instance.GetAllLocations(Dispatcher);
        }
        #endregion

        #region AppBar Button Event Handlers
        void MeButton_Click(object sender, EventArgs e) {
            RhitMapView.Instance.GoToUserLocation();
        }

        void RhitButton_Click(object sender, EventArgs e) {
            RhitMapView.Instance.GoToRhit();
        }

        void SearchButton_Click(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }
        #endregion

        #region AppBar MenuItem Event Handlers
        void Settings_Click(object sender, EventArgs e) {
            GoToSettings();
        }

        void POI_Click(object sender, EventArgs e) {
            GoToQuikList();
        }
        #endregion

        #region GeoCoordinateWatcher Event Handlers
        void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e) {
            //switch(e.Status) {
            //    case GeoPositionStatus.Disabled:
            //        if(watcher.Permission == GeoPositionPermission.Denied)
            //            statusTextBlock.Text = "You have disabled Location Service.";
            //        else
            //            statusTextBlock.Text = "Location Service is not functioning on this device.";
            //        break;
            //    case GeoPositionStatus.Initializing:
            //        statusTextBlock.Text = "Location Service is retrieving data...";
            //        break;
            //    case GeoPositionStatus.NoData:
            //        statusTextBlock.Text = "Location data is not available.";
            //        break;
            //    case GeoPositionStatus.Ready:
            //        statusTextBlock.Text = "Location data is available.";
            //        break;
            //}
        }

        void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e) {
            GeoCoordinate coordinate = e.Position.Location;
            string accuracy = "0.0000000000000";
            //statusTextBlock.Text = "Updated: " + coordinate.Latitude.ToString(accuracy) + ", " + coordinate.Longitude.ToString(accuracy);
            RhitMapView.Instance.User.Location = e.Position.Location;
        }

        void bgWatcherUpdate() {
            _userLocationWatcher.TryStart(true, TimeSpan.FromMilliseconds(60000));
        }
        #endregion

        private void fillGrid() {
            ContentPanel.Children.Clear();
            ContentPanel.RowDefinitions.Clear();
            ContentPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            if (RhitMapView.Instance.InDebugMode)
                ContentPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            ContentPanel.Children.Add(RhitMapView.Instance.Map);
            Grid.SetRow(RhitMapView.Instance.Map, 0);
            if(RhitMapView.Instance.InDebugMode) {
                _debugText = new TextBlock() { Text = RhitMapView.Instance.DebugText };
                ContentPanel.Children.Add(_debugText);
                Grid.SetRow(_debugText, 1);
            }
        }

        public void GoToSettings() {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        public void GoToQuikList() {
            NavigationService.Navigate(new Uri("/QuickListPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            LoadData();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            RhitMapView.Instance.StoreData();
            ContentPanel.Children.Remove(RhitMapView.Instance.Map);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            //SystemTray.IsVisible = false;
            InternetConnection.TestConnection();
        }

        public void LoadData() {
            DataCollector.Instance.UpdateAvailable += new ServiceEventHandler(OnUpdateAvailable);
            //List<RhitLocation> locations = DataCollector.Instance.GetAllLocations(Dispatcher);
            List<RhitLocation> locations = DataCollector.Instance.GetMapAreas(Dispatcher);

            if(!ContentPanel.Children.Contains(RhitMapView.Instance.Map)) LoadMap();
            
            if(locations != null) RhitMapView.Instance.Outlines = locations;
            //TODO: Display message if locations == null; "Map Data is Empty. Trying to download..."

            fillGrid();
        }
    }
}