using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Rhit.Applications.ViewModels;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.Views {
    public partial class DataPage : Page {
        public DataPage() {
            InitializeComponent();

            ViewModel = new DataViewModel();
            DataContext = ViewModel;
        }

        private DataViewModel ViewModel { get; set; }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            ViewModel.SelectLocation((sender as TreeView).SelectedItem);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if(!LoginController.Instance.IsLoggedIn) {
                NavigationService.Navigate(new Uri("/LoginPage", UriKind.Relative));
            }
        }

        private void AltNameGrid_KeyUp(object sender, KeyEventArgs e) {
            if(Keyboard.Modifiers != ModifierKeys.Control || e.Key != Key.Delete) return;
            ViewModel.DeleteAltName((sender as DataGrid).SelectedIndex);
        }

        private void LinksGrid_KeyUp(object sender, KeyEventArgs e) {
            if(Keyboard.Modifiers != ModifierKeys.Control || e.Key != Key.Delete) return;
            ViewModel.DeleteLink((sender as DataGrid).SelectedIndex);
        }
    }
}
