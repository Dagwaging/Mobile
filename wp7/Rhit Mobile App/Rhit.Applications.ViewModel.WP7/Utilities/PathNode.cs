using System.Device.Location;
using System.Windows;

namespace Rhit.Applications.ViewModel.Utilities {
    public class PathNode : DependencyObject {
        private static int LastNumber = 0;
        public PathNode(double latitude, double longitude) {
            Number = ++LastNumber;
            Center = new GeoCoordinate(latitude, longitude);
        }

        internal static void Restart() {
            LastNumber = 0;
        }

        public int Number { get; set; }

        public string Action { get; set; }

        internal PathNode Next { get; set; }

        internal PathNode Previous { get; set; }

        public GeoCoordinate Center { get; private set; }

        #region IsSelected
        public bool IsSelected {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
           DependencyProperty.Register("IsSelected", typeof(bool), typeof(PathNode), new PropertyMetadata(false));
        #endregion
    }
}
