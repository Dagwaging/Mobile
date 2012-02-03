using System.Windows;

namespace Rhit.Applications.ViewModel.Utilities {
    public class DynAlternateName : AlternateName {
        #region string Name
        public override string Name {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(DynAlternateName), new PropertyMetadata(""));
        #endregion
    }
}
