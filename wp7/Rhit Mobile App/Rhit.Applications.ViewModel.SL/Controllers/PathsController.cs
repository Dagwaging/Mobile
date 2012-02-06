using System.Collections.ObjectModel;
using System.Windows.Input;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Utilities;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using System.Device.Location;
#else
using Microsoft.Maps.MapControl;
#endif

namespace Rhit.Applications.ViewModel.Controllers {
    public class PathsController {
        
        private PathsController() {
            All = new ObservableCollection<Path>();
            Nodes = new LocationCollection();
            DirectionsCommand = new RelayCommand(p => GetDirections());
            DataCollector.Instance.DirectionsReturned += new Model.Events.DirectionsEventHandler(DirectionsReturned);
        }

        private void DirectionsReturned(object sender, Model.Events.DirectionsEventArgs e) {
            Nodes.Clear();
            foreach(DirectionPath_DC path in e.Paths)
                Nodes.Add(new GeoCoordinate(path.Latitude, path.Longitude));
        }

        #region Singleton Instance
        private static PathsController _instance;
        public static PathsController Instance {
            get {
                if(_instance == null)
                    _instance = new PathsController();
                return _instance;
            }
        }
        #endregion

        public LocationCollection Nodes { get; protected set; }

        #region Directions Command
        public ICommand DirectionsCommand { get; private set; }

        private void GetDirections() {
            DataCollector.Instance.GetTestDirections();
        }
        #endregion

        public ObservableCollection<Path> All { get; private set; }
    }
}
