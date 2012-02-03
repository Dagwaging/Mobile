using System.Windows;

namespace Rhit.Applications.ViewModel.Utilities {
    public class DynLink : Link {
        #region string Name
        public override string Name {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(DynLink), new PropertyMetadata(""));
        #endregion

        #region string Address
        public override string Address {
            get { return (string) GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(DynLink), new PropertyMetadata(""));
        #endregion
    }
}
