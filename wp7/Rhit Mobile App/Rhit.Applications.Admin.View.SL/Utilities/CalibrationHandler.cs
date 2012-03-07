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

        public bool Calibrating { get; set; }
    }
}
