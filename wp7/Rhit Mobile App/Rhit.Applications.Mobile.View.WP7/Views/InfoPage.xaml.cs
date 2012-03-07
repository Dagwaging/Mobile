using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Rhit.Applications.ViewModels;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.Views {
    /// \ingroup pages
    public partial class InfoPage : PhoneApplicationPage {
        public InfoPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public InfoViewModel ViewModel = new InfoViewModel();

        private void InnerLocationSelected(object sender, SelectionChangedEventArgs e) {
            if((sender as ListBox).SelectedItem == null) return;
            ViewModel.SelectLocation((sender as ListBox).SelectedItem);
            NavigationService.Navigate(new Uri("/Views/InfoPage.xaml?Index=" + (Index + 1).ToString(), UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            string indexString;
            if(NavigationContext.QueryString.TryGetValue("Index", out indexString))
                Index = Convert.ToInt32(indexString);
            else Index = 0;
            ViewModel.SetLocation(Index);
        }

        public int Index { get; set; }

        protected override void OnBackKeyPress(CancelEventArgs e) {
            if(Index == 0) ViewModel.ClearStack();
        }

        private void GetDirections_Click(object sender, RoutedEventArgs e) {
            int id = LocationsController.Instance.CurrentLocation.Id;
            if(id == 25 || id == 101 || id == 102 || id == 1700000 || id == 100 || id == 111 || id == 113 || id == 110)
                NavigationService.Navigate(new Uri("/Views/DirectionsPage.xaml?Id=" + id.ToString(), UriKind.Relative));
            else {
                MessageBox.Show("Currently, you can only get directions to Chauncey's, Cafeteriathe, SRC, Logan Library, Tennis Courts, and IM Fields.", "Sorry...", MessageBoxButton.OK);
            }
        }
    }
}