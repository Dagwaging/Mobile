using System.Windows;
using Microsoft.Maps.MapControl;

namespace Rhit.Applications.ViewModel {
    public class PathNode : DependencyObject {
        public PathNode() { }

        #region Location
        public Location Location {
            get { return (Location) GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public static readonly DependencyProperty LocationProperty =
           DependencyProperty.Register("Location", typeof(Location), typeof(PathNode), new PropertyMetadata(new Location()));
        #endregion
    }
}
