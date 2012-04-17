using System.Windows.Controls;
using System.Windows.Navigation;
using Rhit.Applications.ViewModels.Controllers;
using System;

namespace Rhit.Applications.Views {
    public partial class HomePage : Page {
        public HomePage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!LoginController.Instance.IsLoggedIn)
            {
                NavigationService.Navigate(new Uri("/LoginPage", UriKind.Relative));
            }
        }
    }
}
