using System.Windows;
using Rhit.Applications.Model;

namespace Rhit.Applications.ViewModel {
    public class Link : DependencyObject, ILink {
        public Link() { }

        #region Name
        public string Name {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Link), new PropertyMetadata(""));
        #endregion

        #region Address
        public string Address {
            get { return (string) GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(Link), new PropertyMetadata(""));
        #endregion
    }
}
