using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Rhit.Applications.ViewModel.Controllers;
using Rhit.Applications.ViewModel.Models;
using System.Windows.Input;

namespace Rhit.Applications.View.Views {
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

        #region Page Navigation
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e) { }

        // Executes when the user navigates away from this page.
        protected override void OnNavigatedFrom(NavigationEventArgs e) { }
        #endregion

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
