using System.Windows.Media.Imaging;

namespace Rhit.Applications.ViewModels.Providers {
    public interface IBitmapProvider {
        BitmapImage GetImage();
    }
}
