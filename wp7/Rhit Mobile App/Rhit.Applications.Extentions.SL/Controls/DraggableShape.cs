using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Rhit.Applications.Extentions.Controls {
    public class DraggableShape : Canvas {
        private bool isDragging = false;
        private MouseButtonEventHandler ShapeMouseDownHandler;
        private MouseEventHandler ParentMouseMoveHandler;
        private MouseButtonEventHandler ParentMouseUpHandler;

        public DraggableShape() { }

        void Parent_MouseMove(object sender, MouseEventArgs e) {
            if(isDragging) {
                Point point = e.GetPosition(this);
                if(point.X >= 0) Canvas.SetLeft(Shape, point.X);
                if(point.Y >= 0) Canvas.SetTop(Shape, point.Y);
            }
        }

        public static UIElement ParentContainer { get; set; }

        private Shape _shape;
        public Shape Shape {
            get { return _shape; }
            set {
                if(_shape != null)
                    Children.Remove(_shape);
                _shape = value;
                Children.Add(_shape);
                AddEventHandlers();
            }
        }

        private void AddEventHandlers() {
            if(Shape == null) return;

            if(ParentContainer != null) {
                if(ParentMouseMoveHandler == null) {
                    ParentMouseMoveHandler = new MouseEventHandler(Parent_MouseMove);
                    ParentContainer.MouseMove += ParentMouseMoveHandler;
                }
                if(ParentMouseUpHandler == null) {
                    ParentMouseUpHandler = new MouseButtonEventHandler(Parent_MouseUp);
                    ParentContainer.MouseLeftButtonUp += ParentMouseUpHandler;
                }
            }

            if(ShapeMouseDownHandler == null) {
                ShapeMouseDownHandler = new MouseButtonEventHandler(Shape_MouseDown);
                Shape.MouseLeftButtonDown += ShapeMouseDownHandler;
            }
        }

        private void Shape_MouseDown(object sender, MouseButtonEventArgs e) {
            AddEventHandlers();
            isDragging = true;
        }

        private void Parent_MouseUp(object sender, MouseButtonEventArgs e) {
            if(isDragging) VirtualLocation = e.GetPosition(this);
            isDragging = false;
        }

        #region VirtualLocation
        public Point VirtualLocation {
            get { return (Point) GetValue(VirtualLocationProperty); }
            set { SetValue(VirtualLocationProperty, value); }
        }

        public static readonly DependencyProperty VirtualLocationProperty =
           DependencyProperty.Register("VirtualLocation", typeof(Point), typeof(DraggableShape), new PropertyMetadata(new Point(), new PropertyChangedCallback(OnVirtualLocationChanged)));

        private static void OnVirtualLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            DraggableShape instance = (DraggableShape) d;
            Canvas.SetLeft(instance.Shape, instance.VirtualLocation.X);
            Canvas.SetTop(instance.Shape, instance.VirtualLocation.Y);

        }
        #endregion
    }
}
