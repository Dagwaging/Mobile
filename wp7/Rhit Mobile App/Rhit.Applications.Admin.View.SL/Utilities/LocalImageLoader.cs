using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Rhit.Applications.ViewModels.Providers;

namespace Rhit.Applications.Views.Utilities {
    public class LocalImageLoader : IBitmapProvider {
        public LocalImageLoader() { }

        public BitmapImage GetImage() {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.png)|*.png";
            if(dialog.ShowDialog() == true) {
                return ParseInfo(dialog.File);
            }
            return null;
        }

        private static BitmapImage ParseInfo(FileInfo file) {
            BitmapImage bitmap = new BitmapImage();
            try {
                FileStream stream = file.OpenRead();
                bitmap.SetSource(stream);
                stream.Close();

            } catch(Exception) { }
            return bitmap;
        }
    }
}
