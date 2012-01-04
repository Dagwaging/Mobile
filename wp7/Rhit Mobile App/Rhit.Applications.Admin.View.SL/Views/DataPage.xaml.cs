using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Rhit.Applications.ViewModel.Controllers;
using Rhit.Applications.ViewModel.Models;

namespace Rhit.Applications.View.Views {
    public partial class DataPage : Page {
        public DataPage() {
            InitializeComponent();
            ViewModel = new DataViewModel(Dispatcher);
            DataContext = ViewModel;
        }

        private DataViewModel ViewModel { get; set; }

        #region Page Navigation
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e) { }

        // Executes when the user navigates away from this page.
        protected override void OnNavigatedFrom(NavigationEventArgs e) { }
        #endregion

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            ViewModel.SelectLocation((sender as TreeView).SelectedItem);
        }

        private void ManageAltNames_Click(object sender, RoutedEventArgs e) {
            NameManagementWindow window = new NameManagementWindow(ViewModel.Locations.CurrentLocation.AltNames);
            window.Closed += new System.EventHandler(AddNames_Closed);
            window.Show();
        }

        private void AddNames_Closed(object sender, System.EventArgs e) {
            NameManagementWindow window = sender as NameManagementWindow;
            if(window.DialogResult == true) {
                ViewModel.Locations.CurrentLocation.AltNames.Clear();
                foreach(string name in window.AltNames)
                    ViewModel.Locations.CurrentLocation.AltNames.Add(name);
            }
        }

        private void AddLink_Click(object sender, RoutedEventArgs e) {
            LinkManagementWindow window = new LinkManagementWindow(ViewModel.Locations.CurrentLocation.Links);
            window.Closed += new System.EventHandler(AddLink_Closed);
            window.Show();
        }

        private void AddLink_Closed(object sender, System.EventArgs e) {
            LinkManagementWindow window = sender as LinkManagementWindow;
            ViewModel.Locations.CurrentLocation.Links.Clear();
            if(window.DialogResult == false) return;
            foreach(Link link in window.Links)
                ViewModel.Locations.CurrentLocation.Links.Add(link);
        }
    }
}
