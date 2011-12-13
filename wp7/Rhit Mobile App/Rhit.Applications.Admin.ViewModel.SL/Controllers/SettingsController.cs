using System.Windows;

namespace Rhit.Applications.ViewModel.Controllers {
    public class SettingsController : DependencyObject {
        private static SettingsController _instance;

        private SettingsController() {
            Status = "Debug Status";
        }

        #region Singleton Instance
        public static SettingsController Instance {
            get {
                if(_instance == null)
                    _instance = new SettingsController();
                return _instance;
            }
        }
        #endregion

        #region Dependency Properties
        #region DebugMode
        public bool DebugMode {
            get { return (bool) GetValue(DebugModeProperty); }
            set { SetValue(DebugModeProperty, value); }
        }

        public static readonly DependencyProperty DebugModeProperty =
           DependencyProperty.Register("DebugMode", typeof(bool), typeof(SettingsController), new PropertyMetadata(false));
        #endregion

        #region Status
        public string Status {
            get { return (string) GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
           DependencyProperty.Register("Status", typeof(string), typeof(SettingsController), new PropertyMetadata(""));
        #endregion
        #endregion
        
    }
}
