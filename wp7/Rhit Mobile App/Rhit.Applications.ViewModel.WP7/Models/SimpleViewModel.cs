using System.Windows;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.ViewModel.Models {
    public class SimpleViewModel : DependencyObject {
        public SimpleViewModel() {
            Initialize();
        }

        private void Initialize() {
            Map = MapController.Instance;
            Settings = SettingsController.Instance;
        }

        public MapController Map { get; protected set; }

        public SettingsController Settings { get; protected set; }
    }
}
