using System;
using System.Windows;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.ViewModel.Utilities {
    public class DualLocation : DependencyObject {
        public DualLocation() {
            DualBinding = false;
            LastUpdate = new Tuple<Location, Point>(new Location(), new Point());
        }

        public RhitLocation BaseLocation { get; set; }

        public bool DualBinding { get; set; }

        private Tuple<Location, Point> LastUpdate { get; set; }

        #region Location
        public Location Location {
            get { return (Location) GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public static readonly DependencyProperty LocationProperty =
           DependencyProperty.Register("Location", typeof(Location), typeof(DualLocation), new PropertyMetadata(new Location(), new PropertyChangedCallback(OnLocationChanged)));

        private static void OnLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            DualLocation instance = (DualLocation) d;
            if(instance.LastUpdate.Item1 == (Location) e.NewValue) return;
            Point point = LocationPositionMapper.Instance.ConvertLocationToPosition((Location) e.NewValue);
            instance.LastUpdate = new Tuple<Location, Point>((Location) e.NewValue, point);
            instance.Point = point;
        }
        #endregion

        #region Point
        public Point Point {
            get { return (Point) GetValue(PointProperty); }
            set { SetValue(PointProperty, value); }
        }

        public static readonly DependencyProperty PointProperty =
           DependencyProperty.Register("Point", typeof(Point), typeof(DualLocation), new PropertyMetadata(new Point(), new PropertyChangedCallback(OnPointChanged)));

        private static void OnPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            DualLocation instance = (DualLocation) d;
            if(instance.LastUpdate.Item2 == (Point) e.NewValue) return;
            Location location = LocationPositionMapper.Instance.ConvertPositionToLocation((Point) e.NewValue);
            instance.LastUpdate = new Tuple<Location, Point>(location, (Point) e.NewValue);
            instance.Location = location;
        }
        #endregion

        #region Label
        public string Label {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
           DependencyProperty.Register("Label", typeof(string), typeof(DualLocation), new PropertyMetadata(""));
        #endregion
    }
}
