using System.Collections.ObjectModel;

namespace Rhit.Applications.ViewModel.Controllers {
    public class PathsController {
        private PathsController() {
            All = new ObservableCollection<Path>();
            //TODO: Bryan - Create dummy paths
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
