using System.Windows;

namespace Rhit.Applications.ViewModels.Controllers {
    public class SettingsController : DependencyObject {
        private static SettingsController _instance;

        private SettingsController() {
            Status = "Debug Status";
            LocationsController.Instance.HideBuildings();
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

        // -- Map Settings --
        #region ShowBuildings
        public bool ShowBuildings {
            get { return (bool) GetValue(ShowBuildingsProperty); }
            set { SetValue(ShowBuildingsProperty, value); }
        }

        public static readonly DependencyProperty ShowBuildingsProperty =
           DependencyProperty.Register("ShowBuildings", typeof(bool), typeof(SettingsController),
           new PropertyMetadata(false, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(e.NewValue == e.OldValue) return;
            if((bool) e.NewValue) LocationsController.Instance.ShowBuildings();
            else LocationsController.Instance.HideBuildings();
        }
        #endregion

        #region ShowTopLocations
        public bool ShowTopLocations {
            get { return (bool) GetValue(ShowTopLocationsProperty); }
            set { SetValue(ShowTopLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowTopLocationsProperty =
           DependencyProperty.Register("ShowTopLocations", typeof(bool), typeof(SettingsController), new PropertyMetadata(false));
        #endregion
    }
}
