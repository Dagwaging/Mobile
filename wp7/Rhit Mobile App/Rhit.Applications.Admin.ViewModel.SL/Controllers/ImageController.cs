using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Providers;

namespace Rhit.Applications.ViewModel.Controllers {
    public class ImageController : DependencyObject {
        //TODO: Utilize the horizontal and vertical checkbox group to switch the orientation of the image.
        private static ImageController _instance;

        private ImageController(IBitmapProvider imageProvider) {
            ImageProvider = imageProvider;

            Loaded = false;

            LoadCommand = new RelayCommand(p => LoadImage());
            CloseCommand = new RelayCommand(p => CloseImage());
        }

        public static void CreateImageController(IBitmapProvider imageProvider) {
            _instance = new ImageController(imageProvider);
        }

        #region Singleton Instance
        public static ImageController Instance {
            get { return _instance; }
        }
        #endregion

        private IBitmapProvider ImageProvider { get; set; }

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

        #region ImagePoints
        public IList<Point> ImagePoints {
            get { return (IList<Point>) GetValue(ImagePointsProperty); }
            set { SetValue(ImagePointsProperty, value); }
        }

        public static readonly DependencyProperty ImagePointsProperty =
           DependencyProperty.Register("ImagePoints", typeof(IList<Point>), typeof(ImageController), new PropertyMetadata(new List<Point>()));
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
        }

        private void CloseImage() {
            Bitmap = null;
            Loaded = false;
        }

        public void ClickImage(Point point) {
            if(ImagePoints.Contains(point)) return;
            IList<Point> points = new List<Point>();
            foreach(Point p in ImagePoints)
                points.Add(p);
            points.Add(point);
            ImagePoints = points;

            //TODO: This method should be implemented like below
            // The problem is that the dependency property object doesn't actually change, its children do.
            // Use ObservableCollection instead.

            //IList<Point> points = ImagePoints;
            //points.Add(point);
            //ImagePoints = points;
        }
        #endregion
    }
}
