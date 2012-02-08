using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Rhit.Applications.ViewModel.Controllers;
using Rhit.Applications.ViewModel.Models;

namespace Rhit.Applications.View.Views {
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
            if(NavigationService.CanGoBack) NavigationService.GoBack();
            else NavigationService.Navigate(new Uri("/Views/MapPage.xaml", UriKind.Relative));
        }
    }
}