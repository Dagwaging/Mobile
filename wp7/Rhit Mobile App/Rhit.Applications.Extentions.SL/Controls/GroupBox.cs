using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rhit.Applications.Extentions.Controls {
    public class GroupBox : ContentControl {
        public GroupBox() {
            DefaultStyleKey = typeof(GroupBox);
            SizeChanged += GroupBox_SizeChanged;
        }

        private RectangleGeometry FullRect { get; set; }

        private RectangleGeometry HeaderRect { get; set; }

        private ContentControl HeaderContainer { get; set; }


        #region public object Header
        public object Header {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(GroupBox), null);
        #endregion

        #region public DataTemplate HeaderTemplate
        public DataTemplate HeaderTemplate {
            get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(GroupBox), null);
        #endregion

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            FullRect = GetTemplateChild("FullRect") as RectangleGeometry;
            HeaderRect = GetTemplateChild("HeaderRect") as RectangleGeometry;
            HeaderContainer = GetTemplateChild("HeaderContainer") as ContentControl;
            HeaderContainer.SizeChanged += HeaderContainer_SizeChanged;
        }
        
        private void GroupBox_SizeChanged(object sender, SizeChangedEventArgs e) {
            FullRect.Rect = new Rect(new Point(), e.NewSize);
        }

        private void HeaderContainer_SizeChanged(object sender, SizeChangedEventArgs e) {
            HeaderRect.Rect = new Rect(new Point(HeaderContainer.Margin.Left, 0), e.NewSize);
        }
    }
}
