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
            foreach (var kvp in e.Mapping) {
                
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
        #endregion
    }
}
