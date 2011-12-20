using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Model;
using System;
using System.Collections.Generic;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;
using Microsoft.Maps.MapControl;

namespace Rhit.Applications.ViewModel.Controllers {
    public class LocationsController : DependencyObject {
        private static LocationsController _instance;

        private LocationsController() {
            All = new ObservableCollection<RhitLocation>();
            Top = new ObservableCollection<RhitLocation>();
            InnerLocations = new ObservableCollection<RhitLocation>();
            Buildings = new ObservableCollection<RhitLocation>();
            QuickList = new ObservableCollection<RhitLocation>();
            PointsOfInterest = new ObservableCollection<RhitLocation>();
            LocationsOnImage = new ObservableCollection<KeyValuePair<Point, RhitLocation>>();
        }

        #region Singleton Instance
        public static LocationsController Instance {
            get {
                if(_instance == null)
                    _instance = new LocationsController();
                return _instance;
            }
        }
        #endregion

        #region Events
        #region CurrentLocationChanged
        public event LocationEventHandler CurrentLocationChanged;
        protected virtual void OnCurrentLocationChanged(LocationEventArgs e) {
            if(CurrentLocationChanged != null) CurrentLocationChanged(this, e);
        }
        #endregion

        #region LocationsChanged
        public event LocationEventHandler LocationsChanged;
        protected virtual void OnLocationsChanged(LocationEventArgs e) {
            if(LocationsChanged != null) LocationsChanged(this, e);
        }
        #endregion

        #endregion

        private void UpdateCollections() {
            PointsOfInterest.Clear();
            QuickList.Clear();
            foreach(RhitLocation location in All) {
                if(location.Type == LocationType.OnQuickList) {
                    QuickList.Add(location);
                    PointsOfInterest.Add(location);
                } else if(location.Type == LocationType.PointOfInterest)
                    PointsOfInterest.Add(location);
                if(location.ParentId == 0)
                    Top.Add(location);
            }
        }

        #region Collections
        public ObservableCollection<RhitLocation> All { get; set; }

        public ObservableCollection<RhitLocation> Top { get; set; }

        public ObservableCollection<RhitLocation> Buildings { get; set; }

        public ObservableCollection<RhitLocation> InnerLocations { get; set; }

        public ObservableCollection<RhitLocation> QuickList { get; set; }

        public ObservableCollection<RhitLocation> PointsOfInterest { get; set; }

        public ObservableCollection<KeyValuePair<Point, RhitLocation>> LocationsOnImage { get; set; }

        private Point[] MapCalibrationPoints { get; set; }

        private Point[] ImageCalibrationPoints { get; set; }
        #endregion

        public void SetLocations(ICollection<RhitLocation> locations) {
            LocationEventArgs args = new LocationEventArgs();
            args.OldLocations = All;
            All.Clear();
            foreach(RhitLocation location in locations) All.Add(location);
            args.NewLocations = All;
            UpdateCollections();
            OnLocationsChanged(args);
        }

        #region SelectLocation Methods
        public void SelectLocation(int id) {
            foreach(RhitLocation location in Buildings)
                if(location.Id == id) {
                    SelectLocation(location);
                    return;
                }
        }

        public void SelectLocation(GeoCoordinate coordinate) {
            foreach(RhitLocation location in All)
                if(location.Center == coordinate) {
                    CurrentLocation = location;
                    return;
                }
        }

        public void SelectLocation(RhitLocation location) {
            LocationEventArgs args = new LocationEventArgs();
            args.OldLocation = CurrentLocation;
            CurrentLocation = location;
            args.NewLocation = CurrentLocation;

            InnerLocations.Clear();
            List<RhitLocation> locations = DataCollector.Instance.GetChildLocations(null, CurrentLocation.Id);
            if(locations != null) foreach(RhitLocation child in locations) InnerLocations.Add(child);

            OnCurrentLocationChanged(args);
        }
        #endregion

        public void UnSelect() {
            LocationEventArgs args = new LocationEventArgs();
            args.OldLocation = CurrentLocation;
            CurrentLocation = null;
            args.NewLocation = CurrentLocation;
            InnerLocations.Clear();
            OnCurrentLocationChanged(args);
        }

        #region Dependency Properties
        #region CurrentLocation
        public RhitLocation CurrentLocation {
            get { return (RhitLocation) GetValue(CurrentLocationProperty); }
            set { SetValue(CurrentLocationProperty, value); }
        }

        public static readonly DependencyProperty CurrentLocationProperty =
           DependencyProperty.Register("CurrentLocation", typeof(RhitLocation), typeof(LocationsController), new PropertyMetadata(null));
        #endregion
        #endregion

        public void AddBuilding(RhitLocation location) {
            if(!Buildings.Contains(location)) Buildings.Add(location);
        }

        public void RemoveBuilding(RhitLocation location) {
            if(Buildings.Contains(location)) Buildings.Remove(location);
        }

        public void SetCalibrationPoints(Dictionary<Location, Point> mapping, int floor) {
            MapCalibrationPoints = new Point[3];
            ImageCalibrationPoints = new Point[3];
            int i = 0;
            foreach (var kvp in mapping) {
                MapCalibrationPoints[i] = new Point(kvp.Key.Latitude, kvp.Key.Longitude);
                ImageCalibrationPoints[i] = kvp.Value;
                i++;
            }

            LocationsOnImage.Clear();
            foreach (var location in InnerLocations)
                if (location.Floor == floor)
                    LocationsOnImage.Add(new KeyValuePair<Point, RhitLocation>(ConvertPointMapToImage(location.Center), location));
        }

        private static void ConvertPoint(double x, double y, Point[] from, Point[] to, out double outX, out double outY) {
            double q = (y - from[1].Y) * (from[2].X - from[1].X)
                - (x - from[1].X) * (from[2].Y - from[1].Y);
            double d = (from[0].X - x) * (from[2].Y - from[1].Y)
                - (from[0].Y - y) * (from[2].X - from[1].X);

            //if (d == 0)
            //    throw something

            double r = q / d;
            q = (y - from[1].Y) * (from[0].X - x)
                - (x - from[1].X) * (from[0].Y - y);
            double s = q / d;

            outX = (s * to[1].X + r * to[0].X - s * to[2].X - to[1].X) / (r - 1);
            outY = (s * to[1].Y + r * to[0].Y + s * to[2].Y - to[1].Y) / (r - 1);
        }

        public Location ConvertPointImageToMap(Point p) {
            double lat;
            double lon;
            ConvertPoint(p.X, p.Y, ImageCalibrationPoints, MapCalibrationPoints, out lat, out lon);
            return new Location(lat, lon);
        }

        public Point ConvertPointMapToImage(Location l) {
            double x;
            double y;
            ConvertPoint(l.Latitude, l.Longitude, MapCalibrationPoints, ImageCalibrationPoints, out x, out y);
            return new Point(x, y);
        }
    }
}
