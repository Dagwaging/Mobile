using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Threading;
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

namespace RhitMobile {
    public partial class MapPage : PhoneApplicationPage {
        #region Private Fields
        private GeoCoordinateWatcher _userLocationWatcher;
        private ApplicationBar _appBar;
        #endregion

        public MapPage() {
            InitializeComponent();

            LoadMap();

            RhitMapView.Instance.OutlineTapped += new Events.OutlineEventHandler(Outline_Tapped);
            RhitMapView.Instance.PushpinTapped += new PushpinEventHandler(SelectedPushpin_MouseLeftButtonUp);

            InitApplicationBar();
            InitGeoCordinateWatcher();
        }

        #region Initializers
        private void LoadMap() {
            Map map = RhitMapView.Instance.Map;
            map.Margin = new System.Windows.Thickness(0, 0, 0, 36);
            map.Mode = new MercatorMode();
            map.CacheMode = new BitmapCache();
            map.LogoVisibility = System.Windows.Visibility.Collapsed;
            map.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            map.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            LayoutRoot.Children.Add(map);
            Grid.SetRow(map, 0);
            map.MapZoom += new EventHandler<MapZoomEventArgs>(Map_MapZoom);
        }

        void InitGeoCordinateWatcher() {
            _userLocationWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);

            _userLocationWatcher.MovementThreshold = 10.0f;

            _userLocationWatcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(Watcher_StatusChanged);
            _userLocationWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(Watcher_PositionChanged);

            new Thread(bgWatcherUpdate).Start();
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
            ApplicationBarIconButton button3 = new ApplicationBarIconButton(new Uri("/Images/directions.png", UriKind.Relative)) {
                Text = "Directions",
            };
            button1.Click += new EventHandler(MeButton_Click);
            button2.Click += new EventHandler(RhitButton_Click);
            button3.Click += new EventHandler(DirectionsButton_Click);

            _appBar.Buttons.Add(button1);
            _appBar.Buttons.Add(button2);
            _appBar.Buttons.Add(button3);

            ApplicationBarMenuItem pointsOfInterestMenuItem = new ApplicationBarMenuItem("Points of Interest");
            pointsOfInterestMenuItem.Click += new EventHandler(POI_Click);
            _appBar.MenuItems.Add(pointsOfInterestMenuItem);

            ApplicationBarMenuItem settingsMenuItem = new ApplicationBarMenuItem("Settings");
            settingsMenuItem.Click += new EventHandler(Settings_Click);
            _appBar.MenuItems.Add(settingsMenuItem);

            this.ApplicationBar = _appBar;
        }
        #endregion

        #region Map/Service Event Handlers
        void Map_MapZoom(object sender, MapZoomEventArgs e) {
            //TODO: Remove for release
            this.statusTextBlock.Text = "Zoom: " + RhitMapView.Instance.Map.ZoomLevel.ToString();
        }

        private void Outline_Tapped(object sender, Events.OutlineEventArgs e) {
            if(e.Outline == null) return;
            StateManagment.SaveState(null, "SelectedOutline", RhitMapView.Instance.Select(e.Outline));
        }

        private void SelectedPushpin_MouseLeftButtonUp(object sender, PushpinEventArgs e) {
            NavigationService.Navigate(new Uri("/DescriptionPage.xaml", UriKind.Relative));
        }

        private void OnUpdateAvailable(object sender, ServerEventArgs e) {
            RhitMapView.Instance.Outlines = e.ResponseObject.GetLocations();
        }
        #endregion

        #region AppBar Button Event Handlers
        void MeButton_Click(object sender, EventArgs e) {
            RhitMapView.Instance.GoToUserLocation();
        }

        void RhitButton_Click(object sender, EventArgs e) {
            RhitMapView.Instance.GoToRhit();
        }

        void DirectionsButton_Click(object sender, EventArgs e) {
            //TODO: Implement directions
        }
        #endregion

        #region AppBar MenuItem Event Handlers
        void Settings_Click(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        void POI_Click(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/POIPage.xaml", UriKind.Relative));
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

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if(!LayoutRoot.Children.Contains(RhitMapView.Instance.Map)) LoadMap();
            RhitMapView.Instance.LoadData();

            RhitMapView.Instance.GoToRhit();//TODO: Center to full map view instead

            List<RhitLocation> locations = DataCollector.Instance.GetLocations(Dispatcher);
            DataCollector.Instance.UpdateAvailable += new ServerEventHandler(OnUpdateAvailable);
            if(locations != null) RhitMapView.Instance.Outlines = locations;
            //TODO: Display message if locations == null; "Map Data is Empty. Trying to download..."

            RhitLocation outline = this.LoadState<RhitLocation>("SelectedOutline", null);
            if(outline != null) this.SaveState("SelectedOutline", RhitMapView.Instance.Select(outline));

            string mapSourceName = this.LoadState<string>("MapSource");
            RhitMapView.Instance.ChangeTileSource(mapSourceName);

            bool floorPlans = (bool) this.LoadState<object>("TileOverlay", false);
            if(floorPlans) RhitMapView.Instance.AddOverlay("RHIT Floor Plans");
            else RhitMapView.Instance.RemoveOverlay("RHIT Floor Plans");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            RhitMapView.Instance.StoreData();
            LayoutRoot.Children.Remove(RhitMapView.Instance.Map);
        }
    }
}