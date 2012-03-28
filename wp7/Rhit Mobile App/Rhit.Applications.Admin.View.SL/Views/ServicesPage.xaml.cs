using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Rhit.Applications.ViewModels;

namespace Rhit.Applications.Views.Views
{
    public partial class ServicesPage : Page
    {
        private ServicesViewModel ViewModel { get; set; }

        public ServicesPage()
        {
            ViewModel = new ServicesViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectServiceNode((sender as TreeView).SelectedItem);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
