using System;
using System.Device.Location;
using System.Threading;
using System.Windows;

namespace Rhit.Applications.ViewModels.Controllers {
    public class UserController : DependencyObject {
        private static UserController _instance;

        private UserController() {
            InitGeoCordinateWatcher();
        }

        #region Singleton Instance
        public static UserController Instance {
            get {
                if(_instance == null)
                    _instance = new UserController();
                return _instance;
            }
        }
        #endregion

        private GeoCoordinateWatcher UserTracker { get; set; }

        private Thread TrackerThread { get; set; }

        #region Location
        public GeoCoordinate Location {
            get { return (GeoCoordinate) GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public static readonly DependencyProperty LocationProperty =
           DependencyProperty.Register("Location", typeof(GeoCoordinate), typeof(UserController), new PropertyMetadata(null));
        #endregion

        private void InitGeoCordinateWatcher() {
            UserTracker = new GeoCoordinateWatcher(GeoPositionAccuracy.High);

            UserTracker.MovementThreshold = 10;

            UserTracker.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(Watcher_StatusChanged);
            UserTracker.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(Watcher_PositionChanged);

            TrackerThread = new Thread(bgWatcherUpdate);
            TrackerThread.Start();
        }

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

        private void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e) {
            Location = e.Position.Location;
        }

        private void bgWatcherUpdate() {
            //Update every minute
            UserTracker.TryStart(true, TimeSpan.FromMilliseconds(60000));
        }
        #endregion
    }
}