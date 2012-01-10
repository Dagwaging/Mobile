using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Navigation;
using Microsoft.Maps.MapControl.Overlays;
using Rhit.Applications.View.Controls;
using Rhit.Applications.ViewModel.Providers;

namespace Rhit.Applications.View.Views {
    public partial class MapPage : Page {
        public MapPage() {
            InitializeComponent();

            //TODO: Try not to have to do this
            ViewModel.SetMode(MyMap);

            DraggablePushpin.ParentMap = MyMap;
            DraggableShape.ParentContainer = MyCanvas;

            MyMap.MouseClick += ViewLocations.Map_Click;

            MyMap.MouseClick += Calibrator.Map_Click;
            MyCanvas.MouseLeftButtonUp += Calibrator.ImageViewer_Click;

            MyCanvas.MouseLeftButtonUp += ViewLocations.ImageViewer_Click;

            MyMap.MouseClick += new EventHandler<MapMouseEventArgs>(Map_MouseClick);
            MyMap.MapForeground.TemplateApplied += new EventHandler(MapForeground_TemplateApplied);

            DataContext = ViewModel;
        }

        #region Click Event Methods/Properties
        private Point LastEventCoordinate { get; set; }

        void Map_MouseClick(object sender, MapMouseEventArgs e) {
            if(LastEventCoordinate == e.ViewportPoint) return;
            if(!Calibrator.Calibrating && !ViewLocations.Working) ViewModel.Locations.UnSelect();
        }

        private void MapPolygon_Click(object sender, MouseButtonEventArgs e) {
            if(Calibrator.Calibrating) return;
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as MapPolygon).Tag);
        }

        private void Pushpin_Click(object sender, MouseButtonEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as Pushpin).Tag);
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
            LayoutRoot.Children.Remove(MyMap);
            MyMap.Children.Clear();
        }
        #endregion

        private void DraggableShape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as DraggableShape).Tag);
        }

        private void DraggablePushpin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as Pushpin).Tag);
        }
    }

    public class LocationsProvider : DependencyObject, ILocationsProvider {
        private enum BehaviorState { Default, CreatingCorners, MovingCorners, AddingLocation };

        public LocationsProvider() {
            Locations = new ObservableCollection<LocationWrapper>();
            Points = new ObservableCollection<Point>();
            State = BehaviorState.Default;
        }

        public bool Working { get; private set; }

        #region private BehaviorState State
        private BehaviorState _state;
        private BehaviorState State {
            get { return _state; }
            set {
                _state = value;
                if(_state == BehaviorState.Default) Working = false;
                else Working = true;
            }
        }
        #endregion

        public ObservableCollection<LocationWrapper> Locations { get; set; }

        public ObservableCollection<Point> Points { get; set; }

        public void DisplayCorners(ICollection<Location> corners) {
            Locations.Clear();
            foreach(Location corner in corners)
                Locations.Add(new LocationWrapper(corner));
            State = BehaviorState.MovingCorners;
        }

        public IList<Location> GetLocations() {
            IList<Location> locations = new List<Location>();
            foreach(LocationWrapper location in Locations)
                locations.Add(location.Location);
            return locations;
        }

        public IList<Point> GetPoints() {
            return Points;
        }

        public void Clear() {
            Locations.Clear();
            Points.Clear();
            State = BehaviorState.Default;
        }

        public void CreateNewCorners() {
            Locations.Clear();
            State = BehaviorState.CreatingCorners;
        }

        public void Map_Click(object sender, MapMouseEventArgs e) {
            if(State == BehaviorState.CreatingCorners) {
                Location corner = (sender as Map).ViewportPointToLocation(e.ViewportPoint);
                Locations.Add(new LocationWrapper(corner));
                e.Handled = true;
            }
            if(State == BehaviorState.AddingLocation) {
                if(Locations.Count <= 0) {
                    Location newLocation = (sender as Map).ViewportPointToLocation(e.ViewportPoint);
                    Locations.Add(new LocationWrapper(newLocation));
                    Points.Clear();
                }
                e.Handled = true;
            }
        }

        public void ImageViewer_Click(object sender, MouseButtonEventArgs e) {
            if(State == BehaviorState.AddingLocation) {
                if(Points.Count <= 0) {
                    Point point = e.GetPosition(sender as Canvas);
                    Points.Add(point);
                    Locations.Clear();
                }
                e.Handled = true;
            }
        }

        public void QueryLocation() {
            State = BehaviorState.AddingLocation;
            LocationIdWindow window = new LocationIdWindow();
            window.Closed += new EventHandler(LocationId_Closed);
            window.Show();
        }

        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        private void LocationId_Closed(object sender, EventArgs e) {
            LocationIdWindow window = sender as LocationIdWindow;
            Id = window.GetIdNumber();
            ParentId = window.GetParentId();
            Name = window.NewName;
        }

        public class LocationWrapper : DependencyObject {
            public LocationWrapper(Location location) { Location = location; }
            #region Location
            public Location Location {
                get { return (Location) GetValue(LocationProperty); }
                set { SetValue(LocationProperty, value); }
            }

            public static readonly DependencyProperty LocationProperty =
               DependencyProperty.Register("Location", typeof(Location), typeof(LocationWrapper), new PropertyMetadata(new Location()));
            #endregion
        }
    }

    public class CalibrationHandler : DependencyObject, IBuildingMappingProvider {
        public CalibrationHandler() {
            Locations = new ObservableCollection<Location>();
            Points = new ObservableCollection<Point>();
            Calibrating = false;
        }

        public void Map_Click(object sender, MapMouseEventArgs e) {
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
