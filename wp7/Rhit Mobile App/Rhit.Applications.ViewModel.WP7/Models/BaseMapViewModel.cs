using System.Device.Location;
using System.Windows;

namespace Rhit.Applications.ViewModel.Models {
    public class BaseMapViewModel : DependencyObject {
        public BaseMapViewModel() {
            Initialize();
        }

        protected virtual void Initialize() {
            ZoomLevel = 17;
            Center = new GeoCoordinate(39.483433300823, -87.3257801091232); //TODO: No Hard coding
        }

        #region Center
        public GeoCoordinate Center {
            get { return (GeoCoordinate) GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty CenterProperty =
           DependencyProperty.Register("Center", typeof(GeoCoordinate), typeof(BaseMapViewModel), new PropertyMetadata(new GeoCoordinate()));
        #endregion

        #region ZoomLevel
        public double ZoomLevel {
            get { return (double) GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        public static readonly DependencyProperty ZoomLevelProperty =
           DependencyProperty.Register("ZoomLevel", typeof(double), typeof(BaseMapViewModel), new PropertyMetadata(17.0));
        #endregion
    }
}
