using System.Windows;

namespace Rhit.Applications.ViewModels.Utilities {
    public class AlternateName : DependencyObject {
        public AlternateName() { Name = ""; }
        public AlternateName(string name) { Name = name; }
        #region string Name
        public string Name {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(AlternateName), new PropertyMetadata(""));
        #endregion
    }
}
