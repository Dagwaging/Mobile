using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rhit.Applications.ViewModel.Controllers;
using Rhit.Applications.Model;
using System.Collections.Generic;

namespace Rhit.Applications.ViewModel.Models {
    public class InfoViewModel {
        public InfoViewModel() {
            Map = MapController.Instance;
            Locations = LocationsController.Instance;
            if(LocationStack == null)
                LocationStack = new Dictionary<int, RhitLocation>();
        }

        public MapController Map { get; set; }

        public LocationsController Locations { get; set; }

        private static Dictionary<int, RhitLocation> LocationStack { get; set; }

        public void SelectLocation(object obj) {
            if(obj is RhitLocation) Locations.SelectLocation(obj as RhitLocation);
            //try { Locations.SelectLocation(obj as RhitLocation); } catch { }
        }

        public void SetLocation(int index) {
            if(LocationStack.ContainsKey(index)) {
                Locations.SelectLocation(LocationStack[index]);
                List<int> toRemove = new List<int>();
                foreach(int i in LocationStack.Keys)
                    if(i > index) toRemove.Add(i);
                foreach(int i in toRemove)
                    LocationStack.Remove(i);
            }
            LocationStack[index] = Locations.CurrentLocation;
        }

        public void ClearStack() {
            LocationStack.Clear();
        }
    }
}
