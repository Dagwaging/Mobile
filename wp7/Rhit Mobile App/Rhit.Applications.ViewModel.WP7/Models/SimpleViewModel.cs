using System.Windows;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.ViewModel.Models {
    public class SimpleViewModel : DependencyObject {
        public SimpleViewModel() {
            Settings = SettingsController.Instance;
        }

        public SettingsController Settings { get; protected set; }
    }
}
