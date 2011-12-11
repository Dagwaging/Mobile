using System.Windows.Input;
using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using Rhit.Applications.ViewModel.Models;

namespace Rhit.Applications.View.Views {
    /// \ingroup pages
    public partial class SearchPage : PhoneApplicationPage {
        public SearchPage() {
            InitializeComponent();

            ViewModel = new SearchViewModel();
            DataContext = ViewModel;
        }

        public SearchViewModel ViewModel { get; set; }

        private void PlacesSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if((sender as ListBox).SelectedItem == null) return;
            ViewModel.SelectLocation((sender as ListBox).SelectedItem);
            NavigationService.Navigate(new Uri("/Views/InfoPage.xaml", UriKind.Relative));
        }

        private void PeopleSelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void Search_KeyUp(object sender, KeyEventArgs e) {
            if(e.Key == Key.Enter) {
                ViewModel.Search((sender as PhoneTextBox).Text);
                this.Focus(); //Closes virtual keyboard
            }
        }
    }
}