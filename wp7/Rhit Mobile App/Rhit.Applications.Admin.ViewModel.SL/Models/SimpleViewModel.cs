using System.Windows;
using Rhit.Applications.Model.Events;
using Rhit.Applications.ViewModel.Controllers;
using Rhit.Applications.Model;

namespace Rhit.Applications.ViewModel.Models {
    public class SimpleViewModel : DependencyObject {
        public SimpleViewModel() {
            Locations = LocationsController.Instance;
        }

        public LocationsController Locations { get; private set; }

        public RhitLocation TempLocation {
            get { return (RhitLocation) GetValue(TempLocationProperty); }
            set { SetValue(TempLocationProperty, value); }
        }

        public static readonly DependencyProperty TempLocationProperty =
           DependencyProperty.Register("TempLocation", typeof(RhitLocation), typeof(SimpleViewModel), new PropertyMetadata(null));

        public void SetTempLocation(object location) {
            if(location is RhitLocation)
                TempLocation = location as RhitLocation;
            else if(location is LocationNode)
                TempLocation = (location as LocationNode).Location;
        }

        public int GetTempId() {
            if(TempLocation == null) return 0;
            return TempLocation.Id;
        }
    }
}
