using System.ComponentModel;
using System.Windows;
using Rhit.Applications.Models;
using Rhit.Applications.ViewModels.Controllers;
using Rhit.Applications.ViewModels.Utilities;

namespace Rhit.Applications.ViewModels {
    public class SimpleViewModel : DependencyObject {
        public SimpleViewModel() {
            if(DesignerProperties.IsInDesignTool) return;
            Locations = LocationsController.Instance;
        }

        public LocationsController Locations { get; private set; }

        public LocationData TempLocation {
            get { return (LocationData) GetValue(TempLocationProperty); }
            set { SetValue(TempLocationProperty, value); }
        }

        public static readonly DependencyProperty TempLocationProperty =
           DependencyProperty.Register("TempLocation", typeof(LocationData), typeof(SimpleViewModel), new PropertyMetadata(null));

        public void SetTempLocation(object location) {
            if(location is LocationData)
                TempLocation = location as LocationData;
            else if(location is LocationNode)
                TempLocation = (location as LocationNode).Location;
        }

        public int GetTempId() {
            if(TempLocation == null) return 0;
            return TempLocation.Id;
        }
    }
}
