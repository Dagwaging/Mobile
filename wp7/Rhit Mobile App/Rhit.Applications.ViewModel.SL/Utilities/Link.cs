using System.Windows;
using Rhit.Applications.Models;

namespace Rhit.Applications.ViewModels.Utilities {
    public class Link : DependencyObject, ILink {
        public Link() { }

        public Link_DC ToLightWeightDataContract()
        {
            Link_DC result = new Link_DC();
            result.Name = Name;
            result.Address = Address;

            return result;
        }

        #region string Name
        public string Name {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Link), new PropertyMetadata(""));
        #endregion

        #region string Address
        public string Address {
            get { return (string) GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(Link), new PropertyMetadata(""));
        #endregion
    }
}
