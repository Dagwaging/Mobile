using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Rhit.Applications.ViewModel;

namespace Rhit.Applications.View.Controls {
    public class LocalImageLoader : IBitmapProvider {


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

            } catch(Exception exception) { }
            return bitmap;
        }
    }
}
