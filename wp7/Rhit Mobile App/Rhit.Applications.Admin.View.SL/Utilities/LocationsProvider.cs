using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Views;
using Rhit.Applications.ViewModels.Providers;

namespace Rhit.Applications.Views.Utilities {
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
}
