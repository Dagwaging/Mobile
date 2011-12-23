using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Rhit.Applications.ViewModel.Models;

namespace Rhit.Applications.View.Views {
    public partial class LinkManagementWindow : ChildWindow {
        public LinkManagementWindow(IEnumerable<Link> links) {
            InitializeComponent();
            DataContext = this;
            Links = new List<Link>();
            VisibleLinks = new ObservableCollection<Link>();
            if(links == null) return;
            foreach(Link link in links) {
                VisibleLinks.Add(link);
                Links.Add(link);
            }
        }

        public List<Link> Links { get; private set; }

        public ObservableCollection<Link> VisibleLinks { get; private set; }

        private void OKButton_Click(object sender, RoutedEventArgs e) {
            Links = new List<Link>();
            foreach(Link s in VisibleLinks) Links.Add(s);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        private void AddLinkButton_Click(object sender, RoutedEventArgs e) {
            string name = LinkNameBox.Text;
            string address = LinkAddressBox.Text;
            if(name == string.Empty || address == string.Empty) return;
            Link link = ContainsLink(VisibleLinks, name, address);
            if(link == null) {
                link = new Link() { Name = name, Address = address, };
                VisibleLinks.Add(link);
            }
            LinkGrid.SelectedItem = link;
        }

        private Link ContainsLink(IEnumerable<Link> list, string name, string address) {
            foreach(Link link in list)
                if(link.Name == name && link.Address == address)
                    return link;
            return null;
        }

        private void RemoveLinkButton_Click(object sender, RoutedEventArgs e) {
            int tmp = LinkGrid.SelectedIndex;
            VisibleLinks.Remove(LinkGrid.SelectedItem as Link);
            try {
                if(tmp > 0) tmp--;
                LinkGrid.SelectedIndex = tmp;
            } catch { }
        }

        private void AddLink_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
            if(e.Key == Key.Enter) AddLinkButton_Click(this, null);
        }
    }
}

