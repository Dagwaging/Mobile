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
    public partial class MapPage : Page, IBuildingCornersProvider, IBuildingMappingProvider {
        public MapPage() {
            InitializeComponent();
            
            Calibrating = false;

            MyMap.MouseClick += new EventHandler<MapMouseEventArgs>(Map_MouseClick);
            MyMap.MapForeground.TemplateApplied += new EventHandler(MapForeground_TemplateApplied);


            //TODO: Don't use this class to implement IBuildingCornersProvider
            ViewModel = new MainViewModel(MyMap, this, this, new LocalImageLoader());
            DataContext = ViewModel;
        }

        private MainViewModel ViewModel { get; set; }

        #region Click Event Methods/Properties
        private Point LastEventCoordinate { get; set; }

        void Map_MouseClick(object sender, MapMouseEventArgs e) {
            if(Calibrating) {
                if(TempLocations.Count >= 3)
                    MessageBox.Show("Only three points are needed. Still need more on the image.");
                else {
                    Pushpin pin = new DraggablePushpin() { Location = MyMap.ViewportPointToLocation(e.ViewportPoint), };
                    MyMap.Children.Add(pin);
                    TempLocations.Add(pin);
                    if(TempPoints.Count >= 3 && TempLocations.Count >= 3)
                        EndCalibration();
                }
            }
            if(LastEventCoordinate == e.ViewportPoint) return;
            ViewModel.Locations.UnSelect();
        }

        private void MapPolygon_Click(object sender, MouseButtonEventArgs e) {
            if(Calibrating) return;
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation(sender as MapPolygon);
        }

        private void Pushpin_Click(object sender, MouseButtonEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((sender as Pushpin).Location);
        }

        private void ImageViewer_Click(object sender, MouseButtonEventArgs e) {
            if(Calibrating) {
                if(TempPoints.Count >= 3)
                    MessageBox.Show("Only three points are needed. Still need more on the map.");
                else {
                    Point point = e.GetPosition(sender as Canvas);
                    TempPoints.Add(point);
                    ViewModel.Image.AddPoint(point);
                    if(TempPoints.Count >= 3 && TempLocations.Count >= 3)
                        EndCalibration();
                }
            }
        }
        #endregion

        private bool Calibrating { get; set; }

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

        #region Implementing IBuildingMappingProvider
        private FloorMappingEventArgs TempArgs { get; set; }
        private List<Pushpin> TempLocations { get; set; }
        private List<Point> TempPoints { get; set; }

        public void QueryMapping() {
            FloorNumberWindow window = new FloorNumberWindow();
            window.Closed += new EventHandler(FloorNumber_Closed);
            window.Show();
        }
        public event FloorMappingEventHandler MappingFinalized;

        private void FloorNumber_Closed(object sender, EventArgs e) {
            FloorNumberWindow window = sender as FloorNumberWindow;
            TempArgs = new FloorMappingEventArgs() {
                FloorNumber = window.GetFloorNumber(),
                Mapping = new Dictionary<Location,Point>(),
            };
            TempLocations = new List<Pushpin>();
            TempPoints = new List<Point>();

            Calibrating = true;
            MessageBox.Show("Click 3 points on the map and three points on the Image.");
        }

        private void EndCalibration() {
            Calibrating = false;

            for(int i=0; i<3; i++) {
                Pushpin pin = TempLocations[i];
                MyMap.Children.Remove(pin);
                TempArgs.Mapping[pin.Location] = TempPoints[i];
            }
            TempLocations.Clear();
            TempPoints.Clear();
            ViewModel.Image.CalibrationPoints.Clear();

            if(MappingFinalized != null) MappingFinalized(this, TempArgs);
            TempArgs = null;
        }
        #endregion
    }
}
