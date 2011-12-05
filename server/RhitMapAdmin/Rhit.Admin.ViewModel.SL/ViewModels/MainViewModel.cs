using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Mvvm.Commands;
using System.Collections.Generic;

namespace Rhit.Admin.ViewModel.ViewModels {
    public class MainViewModel : DependencyObject {
        //TODO: Utilize the horizontal and vertical checkbox group to switch the orientation of the image.

        private IBitmapProvider _imageProvider;
        private MapViewModel _map;

        public MainViewModel(MapViewModel mapViewModel, IBitmapProvider imageProvider) {
            _map = mapViewModel;
            _imageProvider = imageProvider;

            ImageColumnWidth = GridLength.Auto;
            ImageLoaded = false;

            LoadImageCommand = new RelayCommand(p => LoadImage());
            CloseImageCommand = new RelayCommand(p => CloseImage());
            GotoRhitCommand = new RelayCommand(p => _map.GotoRhit());
        }

        #region Properties
        public BitmapImage Image {
            get { return (BitmapImage) GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public GridLength ImageColumnWidth {
            get { return (GridLength) GetValue(ImageColumnWidthProperty); }
            set { SetValue(ImageColumnWidthProperty, value); }
        }

        public bool ImageLoaded {
            get { return (bool) GetValue(ImageLoadedProperty); }
            set { SetValue(ImageLoadedProperty, value); }
        }

        public IList<Point> ImagePoints {
            get { return (IList<Point>) GetValue(ImagePointsProperty); }
            set { SetValue(ImagePointsProperty, value); }
        }
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(BitmapImage), typeof(MainViewModel), new PropertyMetadata(new BitmapImage()));

        public static readonly DependencyProperty ImageColumnWidthProperty =
           DependencyProperty.Register("ImageColumnWidth", typeof(GridLength), typeof(MainViewModel), new PropertyMetadata(new GridLength()));

        public static readonly DependencyProperty ImageLoadedProperty =
           DependencyProperty.Register("ImageLoaded", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));

        public static readonly DependencyProperty ImagePointsProperty =
           DependencyProperty.Register("ImagePoints", typeof(IList<Point>), typeof(MainViewModel), new PropertyMetadata(new List<Point>()));
        #endregion

        #region Commands
        public ICommand LoadImageCommand { get; private set; }

        public ICommand CloseImageCommand { get; private set; }

        public ICommand GotoRhitCommand { get; private set; }
        #endregion

        #region Methods
        private void LoadImage() {
            Image = _imageProvider.GetImage();
            if(Image != null) {
                ImageColumnWidth = new GridLength(1, GridUnitType.Star);
                ImageLoaded = true;
            }
        }

        private void CloseImage() {
            Image = null;
            ImageLoaded = false;
            ImageColumnWidth = new GridLength(0, GridUnitType.Pixel);
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
