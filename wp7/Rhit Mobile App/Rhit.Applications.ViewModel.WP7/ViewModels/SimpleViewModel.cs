using System.Windows;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.ViewModels {
    public class SimpleViewModel : DependencyObject {
        public SimpleViewModel() {
            Settings = SettingsController.Instance;
        }

        public SettingsController Settings { get; protected set; }
    }
}
