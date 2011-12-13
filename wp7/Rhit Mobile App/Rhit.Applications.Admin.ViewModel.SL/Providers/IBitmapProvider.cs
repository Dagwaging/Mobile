using System.Windows.Media.Imaging;

namespace Rhit.Applications.ViewModel.Providers {
    public interface IBitmapProvider {
        BitmapImage GetImage();
    }
}
