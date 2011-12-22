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
        }

        private void MappingFinalized(object sender, FloorMappingEventArgs e) {
            LocationPositionMapper.Instance.ApplyMapping(e.Mapping, e.FloorNumber);
            
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

        #region Methods
        public void LoadImage() {
            Bitmap = ImageProvider.GetImage();
            if(Bitmap != null) {
                Loaded = true;
            }
            MappingProvider.QueryMapping();
        }

        public void CloseImage() {
            Bitmap = null;
            Loaded = false;
        }
        #endregion
    }
}
