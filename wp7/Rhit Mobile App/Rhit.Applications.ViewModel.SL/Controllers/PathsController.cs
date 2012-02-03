using System.Collections.ObjectModel;
using Rhit.Applications.ViewModel.Utilities;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
#else
using Microsoft.Maps.MapControl;
#endif

namespace Rhit.Applications.ViewModel.Controllers {
    public class PathsController {
        private PathsController() {
            All = new ObservableCollection<Path>();


            //TODO: Bryan - Remove Fake Data
            //PathNode node1 = new PathNode() { Location = new Location(39.4821800526708, -87.3222422754326), };
            //PathNode node2 = new PathNode() { Location = new Location(39.4849499103115, -87.3218614017525), };
            //Path path = new Path() { First = node1, Second = node2, };
            //All.Add(path);


            //TODO: Bryan - Hook to UI
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

        public ObservableCollection<Path> All { get; private set; }
    }
}
