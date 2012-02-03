using System.Windows;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
#else
using Microsoft.Maps.MapControl;
#endif

namespace Rhit.Applications.ViewModel.Utilities {
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
