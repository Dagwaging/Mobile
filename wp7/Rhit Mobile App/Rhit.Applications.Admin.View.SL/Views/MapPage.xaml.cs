using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Rhit.Applications.Extentions.Maps;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.Views.Views {
    public partial class MapPage : Page {
        public MapPage() {
            InitializeComponent();
            DataContext = new MapExtender();
        }

        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e) {
            foreach(UIElement child in LinksStackPanel.Children) {
                HyperlinkButton hb = child as HyperlinkButton;
                if(hb != null && hb.NavigateUri != null) {
                    if(hb.NavigateUri.ToString().Equals(e.Uri.ToString())) {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    } else {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        // If an error occurs during navigation, show an error window
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e) {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }

        private void Delete_Click(object sender, RoutedEventArgs e) {
            MapController.Instance.DeleteCurrentOverlay();
        }

        private void Upload_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Zip Files (*.zip)|*.zip";
            if(dialog.ShowDialog() != true) return;

            MapController.Instance.UploadOverlay(dialog.File);
        }
    }
}
