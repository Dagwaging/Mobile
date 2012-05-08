using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.Views {
    public partial class HomePage : Page {
        public HomePage() {
            InitializeComponent();

            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if(!LoginController.Instance.IsLoggedIn) {
                NavigationService.Navigate(new Uri("/LoginPage", UriKind.Relative));
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Zip Files (*.zip)|*.zip";
            if(dialog.ShowDialog() != true) return;

            ViewModel.DoSomethingElse(dialog.File);
            
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e) {
            ViewModel.DoSomething();
        }

    }
}
