using System.Windows.Media.Imaging;

namespace Rhit.Admin.ViewModel {
    public interface IBitmapProvider {
        BitmapImage GetImage();
    }
}
