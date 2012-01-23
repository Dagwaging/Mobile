using System.Collections.Generic;
using Rhit.Applications.Model;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.ViewModel.Models {
    public class InfoViewModel {
        public InfoViewModel() {
            Locations = LocationsController.Instance;
            if(LocationStack == null)
                LocationStack = new Dictionary<int, RhitLocation>();
        }

        public LocationsController Locations { get; set; }

        private static Dictionary<int, RhitLocation> LocationStack { get; set; }

        public void SelectLocation(object obj) {
            if(obj is RhitLocation) Locations.SelectLocation(obj as RhitLocation);
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
