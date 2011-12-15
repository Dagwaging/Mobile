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
using Microsoft.Maps.MapControl.Overlays;
using Microsoft.Maps.MapControl.Navigation;

namespace Rhit.Applications.View.Views {
    public partial class MapPage : Page, IBuildingCornersProvider {
        public MapPage() {
            InitializeComponent();

            MyMap.MouseClick += new EventHandler<MapMouseEventArgs>(Map_MouseClick);
            MyMap.MapForeground.TemplateApplied += new EventHandler(MapForeground_TemplateApplied);


            //TODO: Don't use this class to implement IBuildingCornersProvider
            ViewModel = new MainViewModel(MyMap, new LocalImageLoader(), this);
            DataContext = ViewModel;
        }

        void Map_MouseClick(object sender, MapMouseEventArgs e) {
            ViewModel.MapClick(e);
        }

        private MainViewModel ViewModel { get; set; }

        private void ImageViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            //ViewModel.ClickImage(e.GetPosition((sender as ImageViewer)));
        }

        private void MapPolygon_Click(object sender, MouseButtonEventArgs e) {
            ViewModel.PolygonClick(sender as MapPolygon, e);
        }

        #region Implementing IBuildingCornersProvider
        public void DisplayCorners(ICollection<Location> corners) {
            CornersLayer.Children.Clear();
            foreach(Location corner in corners)
                CornersLayer.Children.Add(new DraggablePushpin() { Location = corner, });
        }

        public List<Location> GetCorners() {
            List<Location> corners = new List<Location>();
            foreach(UIElement element in CornersLayer.Children)
                if(element is Pushpin) corners.Add((element as Pushpin).Location);
            return corners;
        }

        public void ClearCorners() {
            MyMap.MouseClick -= CreateCorner;
            CornersLayer.Children.Clear();
            
        }

        public void CreateNewCorners() {
            CornersLayer.Children.Clear();
            MyMap.MouseClick += new EventHandler<MapMouseEventArgs>(CreateCorner);
        }

        private void CreateCorner(object sender, MapMouseEventArgs e) {
            Location corner = MyMap.ViewportPointToLocation(e.ViewportPoint);
            CornersLayer.Children.Add(new DraggablePushpin() { Location = corner, });
        }
        #endregion

        #region NavigationBar Initialization Methods
        void MapForeground_TemplateApplied(object sender, EventArgs e) {
            if(!(sender is MapForeground)) return;
            (sender as MapForeground).NavigationBar.TemplateApplied += new EventHandler(NavigationBar_TemplateApplied);
        }

        void NavigationBar_TemplateApplied(object sender, EventArgs e) {
            if(!(sender is NavigationBar)) return;
            UpdateNaviBar(sender as NavigationBar);
        }

        private void UpdateNaviBar(NavigationBar naviBar) {
            UIElementCollection children = naviBar.HorizontalPanel.Children;
            children.Clear();


            List<UIElement> elements = new List<UIElement>();
            foreach(UIElement element in NavigationBarItems.Children)
                elements.Add(element);
            foreach(UIElement element in elements) {
                NavigationBarItems.Children.Remove(element);
                naviBar.HorizontalPanel.Children.Add(element);
            }
        }
        #endregion

        #region Page Navigation
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e) { }

        // Executes when the user navigates away from this page.
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            //LayoutRoot.Children.Remove(Map.RhitMap);
        }
        #endregion
    }
}
