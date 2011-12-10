using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Rhit.Applications.View.Controls {
    public class ImageViewer : Canvas  {
        public ImageViewer() : base() {
            Circles = new Dictionary<Point, Ellipse>();
        }

        public BitmapImage CurrentImage {
            get { return (BitmapImage) GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("CurrentImage", typeof(BitmapImage), typeof(ImageViewer), new PropertyMetadata(new BitmapImage(), ImageChanged));

        private static void ImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            (d as ImageViewer).UpdateView();
        }

        public static readonly DependencyProperty PointListProperty =
            DependencyProperty.Register("PointList", typeof(IList<Point>), typeof(ImageViewer), new PropertyMetadata(null, PointListChanged));

        public IList<Point> PointList {
            get { return (IList<Point>) GetValue(PointListProperty); }
            set { SetValue(PointListProperty, value); }
        }

        private static void PointListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            (d as ImageViewer).UpdatePoints((e.OldValue as IList<Point>));
        }

        public void AddPoint(Point point) {
            AddPoint(point.X, point.Y);
        }

        private Dictionary<Point, Ellipse> Circles { get; set; }

        public void AddPoint(double x, double y) {
            Ellipse circle = new Ellipse() {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.Black),
            };
            Children.Add(circle);
            Canvas.SetTop(circle, y - circle.Height / 2.0);
            Canvas.SetLeft(circle, x - circle.Width / 2.0);
            Circles.Add(new Point(x, y), circle);
        }

        public void RemovePoint(Point point) {
            if(Circles.ContainsKey(point)) {
                Children.Remove(Circles[point]);
                Circles.Remove(point);
            }
        }

        private void UpdatePoints(IList<Point> oldList) {
            foreach(Point point in PointList) {
                if(oldList.Contains(point)) continue;
                AddPoint(point);
            }
            if(oldList == null) return;
            foreach(Point point in oldList) {
                if(PointList.Contains(point)) continue;
                RemovePoint(point);
            }
        }

        private void UpdateView() {
            Children.Clear();
            if(CurrentImage == null) return;
            Image image = new Image() {
                Source = CurrentImage,
            };
            Children.Add(image);
            Width = CurrentImage.PixelWidth;
            Height = CurrentImage.PixelHeight;
        }
    }
}
