using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Rhit.Applications.ViewModels;

namespace Rhit.Applications.Views {
    public partial class LoginPage : Page {
        public LoginPage() {
            InitializeComponent();

            Loaded += new System.Windows.RoutedEventHandler(LoginPage_Loaded);

            ViewModel = new LoginViewModel();
            DataContext = ViewModel;
            ViewModel.Authenticated += new System.EventHandler(User_Authenticated);
        }

        private LoginViewModel ViewModel { get; set; }

        private void LoginPage_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            UserNameBox.Focus();
        }

        private void User_Authenticated(object sender, EventArgs e) {
            (App.Current.RootVisual as MainPage).LinksBorder.Visibility = System.Windows.Visibility.Visible;
            NavigationService.Navigate(new Uri("/HomePage", UriKind.Relative));
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) ViewModel.Login();
        }
    }
}
