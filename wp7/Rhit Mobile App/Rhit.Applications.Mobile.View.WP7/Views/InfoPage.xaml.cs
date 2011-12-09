using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Rhit.Applications.ViewModel.Models;

namespace Rhit.Applications.View.Views {
    /// \ingroup pages
    public partial class InfoPage : PhoneApplicationPage {
        public InfoPage() {
            InitializeComponent();

            ViewModel = new InfoViewModel();
            DataContext = ViewModel;
        }

        public InfoViewModel ViewModel { get; set; }

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
    }
}