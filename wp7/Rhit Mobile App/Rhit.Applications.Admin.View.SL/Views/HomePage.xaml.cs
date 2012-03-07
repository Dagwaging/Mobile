using System.Windows.Controls;
using System.Windows.Navigation;

namespace Rhit.Applications.Views {
    public partial class HomePage : Page {
        public HomePage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e) { }
    }
}
