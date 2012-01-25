using System.Windows;

namespace Rhit.Applications.ViewModel {
    public class Path : DependencyObject {
        public Path() { }

        #region First
        public PathNode First {
            get { return (PathNode) GetValue(FirstProperty); }
            set { SetValue(FirstProperty, value); }
        }

        public static readonly DependencyProperty FirstProperty =
           DependencyProperty.Register("First", typeof(PathNode), typeof(Path), new PropertyMetadata(null));
        #endregion

        #region Second
        public PathNode Second {
            get { return (PathNode) GetValue(SecondProperty); }
            set { SetValue(SecondProperty, value); }
        }

        public static readonly DependencyProperty SecondProperty =
           DependencyProperty.Register("Second", typeof(PathNode), typeof(Path), new PropertyMetadata(null));
        #endregion
    }
}
