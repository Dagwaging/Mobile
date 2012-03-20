using System.Collections.Generic;
using System.ComponentModel;
using Rhit.Applications.Models;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.ViewModels {
    public class InfoViewModel {
        public InfoViewModel() {
            if(DesignerProperties.IsInDesignTool) return;
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
            LocationStack[index] = Locations.CurrentLocation.OriginalLocation;
        }

        public void ClearStack() {
            LocationStack.Clear();
        }
    }
}
