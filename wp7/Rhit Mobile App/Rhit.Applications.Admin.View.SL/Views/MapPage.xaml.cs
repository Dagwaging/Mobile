using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Rhit.Applications.View.Controls;
using Rhit.Applications.ViewModel.Models;
using Microsoft.Maps.MapControl;
using Rhit.Applications.ViewModel.Providers;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Rhit.Applications.View.Views {
    public partial class MapPage : Page, IBuildingCornersProvider {
        public MapPage() {
            InitializeComponent();

            //TODO: Don't use this class to implement IBuildingCornersProvider
            ViewModel = new MainViewModel(MyMap, new LocalImageLoader(), this);
            DataContext = ViewModel;
        }

        private MainViewModel ViewModel { get; set; }

        #region Page Navigation
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e) { }

        // Executes when the user navigates away from this page.
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            //LayoutRoot.Children.Remove(Map.RhitMap);
        }
        #endregion

        private void ImageViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            //ViewModel.ClickImage(e.GetPosition((sender as ImageViewer)));
        }

        private void MapPolygon_Click(object sender, MouseButtonEventArgs e) {
            ViewModel.PolygonClick(sender as MapPolygon, e);
        }

        public void DisplayCorners(ICollection<Location> corners) {
            CornersLayer.Children.Clear();
            foreach(Location corner in corners)
                CornersLayer.Children.Add(new DraggablePushpin() { Location = corner, });
        }

        public ICollection<Location> GetCorners() {
            ICollection<Location> corners = new List<Location>();
            foreach(UIElement element in CornersLayer.Children)
                if(element is Pushpin) corners.Add((element as Pushpin).Location);
            return corners;
        }

        public void RemoveCorners() {
            CornersLayer.Children.Clear();
        }
    }
}
