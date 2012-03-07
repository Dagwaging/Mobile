using System;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Rhit.Applications.ViewModels;

namespace Rhit.Applications.Views {
    /// \ingroup pages
    public partial class QuickListPage : PhoneApplicationPage {
        public QuickListPage() {
            InitializeComponent();

            ViewModel = new InfoViewModel();
            DataContext = ViewModel;
        }

        public InfoViewModel ViewModel { get; set; }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if((sender as ListBox).SelectedItem == null) return;
            ViewModel.SelectLocation((sender as ListBox).SelectedItem);
            NavigationService.Navigate(new Uri("/Views/MapPage.xaml", UriKind.Relative));
        }
    }
}