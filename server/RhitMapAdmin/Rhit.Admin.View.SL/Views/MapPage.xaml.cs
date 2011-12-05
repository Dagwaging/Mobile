using System.Windows.Controls;
using System.Windows.Navigation;
using Rhit.Admin.ViewModel.ViewModels;
using Rhit.Admin.View.Controls;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;

namespace Rhit.Admin.View.Views {
    public partial class MapPage : Page {
        public MapPage() {
            InitializeComponent();

            Map = new MapViewModel(App.MapId) {
                ClickedPin = new DraggablePushpin(),
            };

            ViewModel = new MainViewModel(Map, new LocalImageLoader());
            DataContext = ViewModel;

            LayoutMain.Children.Add(Map.RhitMap);
            Grid.SetRow(Map.RhitMap, 0);
            Grid.SetColumn(Map.RhitMap, 0);
        }

        private MapViewModel Map { get; set; }

        private MainViewModel ViewModel { get; set; }

        #region Page Navigation
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e) { }

        // Executes when the user navigates away from this page.
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            LayoutRoot.Children.Remove(Map.RhitMap);
        }
        #endregion

        private void ImageViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            ViewModel.ClickImage(e.GetPosition((sender as ImageViewer)));
        }
    }
}
