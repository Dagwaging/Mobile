using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Navigation;
using Microsoft.Maps.MapControl.Overlays;
using Rhit.Applications.View.Controls;
using Rhit.Applications.ViewModel.Models;
using Rhit.Applications.ViewModel.Providers;
using System.Collections.ObjectModel;
using Rhit.Applications.Mvvm.Commands;

namespace Rhit.Applications.View.Views {
    public partial class MapPage : Page, IBuildingCornersProvider {
        public MapPage() {
            InitializeComponent();

            DraggablePushpin.ParentMap = MyMap;
            DraggableShape.ParentContainer = MyCanvas;

            MyMap.MouseClick += Calibrator.Map_MouseClick;
            MyCanvas.MouseLeftButtonUp += Calibrator.ImageViewer_Click;

            MyMap.MouseClick += new EventHandler<MapMouseEventArgs>(Map_MouseClick);
            MyMap.MapForeground.TemplateApplied += new EventHandler(MapForeground_TemplateApplied);

            //TODO: Don't use this class to implement IBuildingCornersProvider
            ViewModel.Initialize(MyMap, Calibrator, this, new LocalImageLoader());
            DataContext = ViewModel;

        }

        #region Click Event Methods/Properties
        private Point LastEventCoordinate { get; set; }

        void Map_MouseClick(object sender, MapMouseEventArgs e) {
            if(LastEventCoordinate == e.ViewportPoint) return;
            //ViewModel.Locations.UnSelect();
        }

        private void MapPolygon_Click(object sender, MouseButtonEventArgs e) {
            if(Calibrator.Calibrating) return;
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as MapPolygon).Tag, true);
        }

        private void Pushpin_Click(object sender, MouseButtonEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((sender as Pushpin).Location);
        }
        #endregion

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



        private void DraggableShape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as DraggableShape).Tag, false);
        }

        private void DraggablePushpin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as Pushpin).Tag, false);
        }


    }

    public class SampleCommandStore : DependencyObject {
        public SampleCommandStore() {
            SampleCommand = new RelayCommand(p => Sample());
        }

        public ICommand SampleCommand { get; private set; }

        private void Sample() {
            return;
        }
    }

    public class CalibrationHandler : DependencyObject, IBuildingMappingProvider {
        public CalibrationHandler() {
            Locations = new ObservableCollection<Location>();
            Points = new ObservableCollection<Point>();
            Calibrating = false;
        }


        public void Map_MouseClick(object sender, MapMouseEventArgs e) {
            if(Calibrating) {

                if(Locations.Count >= 3)
                    MessageBox.Show("Only three points are needed. Still need more on the image.");
                else {
                    Location location = (sender as Map).ViewportPointToLocation(e.ViewportPoint);
                    Locations.Add(location);
                    if(Points.Count >= 3 && Locations.Count >= 3)
                        EndCalibration();
                }
                e.Handled = true;
            }
        }

        public void ImageViewer_Click(object sender, MouseButtonEventArgs e) {
            if(Calibrating) {
                if(Points.Count >= 3)
                    MessageBox.Show("Only three points are needed. Still need more on the map.");
                else {
                    Point point = e.GetPosition(sender as Canvas);
                    Points.Add(point);
                    if(Points.Count >= 3 && Locations.Count >= 3)
                        EndCalibration();
                }
            }
        }


        public int Floor { get; set; }
        public ObservableCollection<Location> Locations { get; set; }
        public ObservableCollection<Point> Points { get; set; }

        public void QueryMapping() {
            FloorNumberWindow window = new FloorNumberWindow();
            window.Closed += new EventHandler(FloorNumber_Closed);
            window.Show();
        }
        public event FloorMappingEventHandler MappingFinalized;

        private void FloorNumber_Closed(object sender, EventArgs e) {
            FloorNumberWindow window = sender as FloorNumberWindow;
            Floor = window.GetFloorNumber();

            Locations.Clear();
            Points.Clear();

            Calibrating = true;
        }

        private void EndCalibration() {
            Calibrating = false;
            FloorMappingEventArgs args = new FloorMappingEventArgs() {
                FloorNumber = Floor,
                Mapping = new Dictionary<Location, Point>(),
            };
            for(int i = 0; i < 3; i++) {
                args.Mapping[Locations[i]] = Points[i];
            }
            Locations.Clear();
            Points.Clear();

            if(MappingFinalized != null) MappingFinalized(this, args);
        }
    
        public  bool Calibrating { get; set; }
    }
}
