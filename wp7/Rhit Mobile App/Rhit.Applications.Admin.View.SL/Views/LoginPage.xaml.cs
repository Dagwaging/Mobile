using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Rhit.Applications.ViewModel.Models;
using System;

namespace Rhit.Applications.View.Views {
    public partial class LoginPage : Page {
        public LoginPage() {
            InitializeComponent();

            ViewModel = new LoginViewModel();
            ViewModel.Authenticated += new System.EventHandler(User_Authenticated);
            DataContext = ViewModel;
            UserNameBox.Focus();
        }

        void User_Authenticated(object sender, EventArgs e) {
            
            //NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));

            (App.Current.RootVisual as MainPage).LinksBorder.Visibility = System.Windows.Visibility.Visible;
            NavigationService.Navigate(new Uri("/HomePage", UriKind.Relative));
        }

        private LoginViewModel ViewModel { get; set; }

        private void TextBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) ViewModel.Login();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e) { }
    }
}
