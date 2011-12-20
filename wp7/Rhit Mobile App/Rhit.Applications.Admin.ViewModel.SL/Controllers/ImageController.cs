using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Providers;
using Microsoft.Maps.MapControl;
using System.Collections.ObjectModel;

namespace Rhit.Applications.ViewModel.Controllers {
    public class ImageController : DependencyObject {
        //TODO: Utilize the horizontal and vertical checkbox group to switch the orientation of the image.
        private static ImageController _instance;

        private ImageController(IBitmapProvider imageProvider, IBuildingMappingProvider buildingMappingProvider) {
            ImageProvider = imageProvider;
            MappingProvider = buildingMappingProvider;
            MappingProvider.MappingFinalized += new FloorMappingEventHandler(MappingFinalized);
            Loaded = false;

            LoadCommand = new RelayCommand(p => LoadImage());
            CloseCommand = new RelayCommand(p => CloseImage());

            CalibrationPoints = new ObservableCollection<Point>();
            FloorPoints = new ObservableCollection<Point>();
        }

        void MappingFinalized(object sender, FloorMappingEventArgs e) {
            //TODO: Scott - Do something
            MapCalibrationPoints = new Point[3];
            ImageCalibrationPoints = new Point[3];
            int i = 0;
            foreach (var kvp in e.Mapping) {
                MapCalibrationPoints[i] = new Point(kvp.Key.Latitude, kvp.Key.Longitude);
                ImageCalibrationPoints[i] = kvp.Value;
                i++;
            }
        }

        public static void CreateImageController(IBitmapProvider imageProvider, IBuildingMappingProvider buildingMappingProvider) {
            _instance = new ImageController(imageProvider, buildingMappingProvider);
        }

        #region Singleton Instance
        public static ImageController Instance {
            get { return _instance; }
        }
        #endregion

        private IBuildingMappingProvider MappingProvider { get; set; }

        private IBitmapProvider ImageProvider { get; set; }

        public ObservableCollection<Point> CalibrationPoints { get; set; }

        public ObservableCollection<Point> FloorPoints { get; set; }

        private Point[] MapCalibrationPoints { get; set; }

        private Point[] ImageCalibrationPoints { get; set; }

        #region Dependency Properties
        #region Bitmap
        public BitmapImage Bitmap {
            get { return (BitmapImage) GetValue(BitmapProperty); }
            set { SetValue(BitmapProperty, value); }
        }

        public static readonly DependencyProperty BitmapProperty =
            DependencyProperty.Register("Bitmap", typeof(BitmapImage), typeof(ImageController), new PropertyMetadata(new BitmapImage()));
        #endregion

        #region Loaded
        public bool Loaded {
            get { return (bool) GetValue(LoadedProperty); }
            set { SetValue(LoadedProperty, value); }
        }

        public static readonly DependencyProperty LoadedProperty =
           DependencyProperty.Register("Loaded", typeof(bool), typeof(ImageController), new PropertyMetadata(false));
        #endregion
        #endregion

        #region Commands
        public ICommand LoadCommand { get; private set; }

        public ICommand CloseCommand { get; private set; }
        #endregion

        #region Methods
        private void LoadImage() {
            Bitmap = ImageProvider.GetImage();
            if(Bitmap != null) {
                Loaded = true;
            }
            MappingProvider.QueryMapping();
        }

        private void CloseImage() {
            Bitmap = null;
            Loaded = false;
        }

        public void AddPoint(Point point) {
            CalibrationPoints.Add(point);
        }

        private static void ConvertPoint(double x, double y, Point[] from, Point[] to, out double outX, out double outY) {
            double q = (y - from[1].Y) * (from[2].X - from[1].X)
                - (x - from[1].X) * (from[2].Y - from[1].Y);
            double d = (from[0].X - x) * (from[2].Y - from[1].Y)
                - (from[0].Y - y) * (from[2].X - from[1].X);

            //if (d == 0)
            //    throw something

            double r = q / d;
            q = (y - from[1].Y) * (from[0].X - x)
                - (x - from[1].X) * (from[0].Y - y);
            double s = q / d;

            outX = (s * to[1].X + r * to[0].X - s * to[2].X - to[1].X) / (r - 1);
            outY = (s * to[1].Y + r * to[0].Y + s * to[2].Y - to[1].Y) / (r - 1);
        }

        public Location ConvertPointImageToMap(Point p) {
            double lat;
            double lon;
            ConvertPoint(p.X, p.Y, ImageCalibrationPoints, MapCalibrationPoints, out lat, out lon);
            return new Location(lat, lon);
        }

        public Point ConvertPointMapToImage(Location l) {
            double x;
            double y;
            ConvertPoint(l.Latitude, l.Longitude, MapCalibrationPoints, ImageCalibrationPoints, out x, out y);
            return new Point(x, y);
        }
        #endregion
    }
}
