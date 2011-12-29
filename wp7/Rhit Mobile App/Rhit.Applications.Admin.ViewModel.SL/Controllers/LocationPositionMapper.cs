using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model;
using Rhit.Applications.ViewModel.Utility;
using Rhit.Applications.Model.Services;

namespace Rhit.Applications.ViewModel.Controllers {
    public class LocationPositionMapper {
        private static LocationPositionMapper _instance;

        private LocationPositionMapper() {
            Locations = new ObservableCollection<DualLocation>();
        }

        public static LocationPositionMapper Instance {
            get {
                if(_instance == null)
                    _instance = new LocationPositionMapper();
                return _instance;
            }
        }

        public ObservableCollection<DualLocation> Locations { get; set; }

        private Point[] MapCalibrationPoints { get; set; }
        private Point[] ImageCalibrationPoints { get; set; }

        public int Floor { get; set; }

        public void Clear() {
            Locations.Clear();
        }

        public void Save() {
            foreach (var location in Locations) {
                DataCollector.Instance.ExecuteStoredProcedure(location.Dispatcher, "spMoveLocationCenter", new Dictionary<string, object>() {
                    { "location", location.BaseLocation.Id },
                    { "lat", location.Location.Latitude },
                    { "lon", location.Location.Longitude }
                });
            }
        }

        public void ApplyMapping(Dictionary<Location, Point> mapping, int floor) {
            Clear();
            MapCalibrationPoints = new Point[3];
            ImageCalibrationPoints = new Point[3];
            int i = 0;
            foreach(KeyValuePair<Location, Point> kvp in mapping) {
                MapCalibrationPoints[i] = new Point(kvp.Key.Latitude, kvp.Key.Longitude);
                ImageCalibrationPoints[i] = kvp.Value;
                i++;
            }

            foreach(RhitLocation location in LocationsController.Instance.InnerLocations)
                if(location.Floor == floor) {
                    Locations.Add(new DualLocation() {
                        BaseLocation = location,
                        Label = location.Label,
                        Location = location.Center,
                        Point = ConvertLocationToPosition(location.Center),
                    });
                }

            Floor = floor;
        }

        private static void Convert(double x, double y, Point[] from, Point[] to, out double outX, out double outY) {
            double q = (y - from[1].Y) * (from[2].X - from[1].X) - (x - from[1].X) * (from[2].Y - from[1].Y);
            double d = (from[0].X - x) * (from[2].Y - from[1].Y) - (from[0].Y - y) * (from[2].X - from[1].X);
            
            //if (d == 0)
            //    throw something

            double r = q / d;
            q = (y - from[1].Y) * (from[0].X - x) - (x - from[1].X) * (from[0].Y - y);
            double s = q / d;

            outX = (s * to[1].X + r * to[0].X - s * to[2].X - to[1].X) / (r - 1);
            outY = (s * to[1].Y + r * to[0].Y - s * to[2].Y - to[1].Y) / (r - 1);
        }

        public Location ConvertPositionToLocation(Point p) {
            double lat; double lon;
            Convert(p.X, p.Y, ImageCalibrationPoints, MapCalibrationPoints, out lat, out lon);
            return new Location(lat, lon);
        }

        public Point ConvertLocationToPosition(Location l) {
            double x; double y;
            Convert(l.Latitude, l.Longitude, MapCalibrationPoints, ImageCalibrationPoints, out x, out y);
            return new Point(x, y);
        }
    }
}
