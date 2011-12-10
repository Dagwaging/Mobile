using System.Windows.Media.Imaging;

namespace Rhit.Applications.ViewModel {
    public interface IBitmapProvider {
        BitmapImage GetImage();
    }
}
