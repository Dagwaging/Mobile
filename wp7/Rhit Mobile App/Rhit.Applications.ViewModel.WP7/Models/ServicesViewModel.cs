using System.Collections.Generic;
using Rhit.Applications.Model;
using Rhit.Applications.ViewModel.Controllers;
using System.ComponentModel;

namespace Rhit.Applications.ViewModel.Models {
    public class ServicesViewModel {
        public ServicesViewModel() {
            if (DesignerProperties.IsInDesignTool) return;
            Services = ServicesController.Instance;
            if(LocationStack == null)
                LocationStack = new Dictionary<int, RhitLocation>();
        }

        public ServicesController Services { get; private set; }

        private static Dictionary<int, RhitLocation> LocationStack { get; set; }

      
    }
}
