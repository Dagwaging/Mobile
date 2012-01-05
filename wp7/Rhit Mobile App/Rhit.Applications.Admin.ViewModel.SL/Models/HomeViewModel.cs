using System.Windows;
using System.Windows.Input;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Mvvm.Commands;

namespace Rhit.Applications.ViewModel.Models {
    public class HomeViewModel : DependencyObject {
        public HomeViewModel() {
            IncreaseVersionCommand = new RelayCommand(p => IncreaseVersion());
            Version = DataCollector.Instance.Version;
            DataCollector.Instance.UpdateAvailable += new Model.Events.ServiceEventHandler(UpdateAvailable);
        }

        private void UpdateAvailable(object sender, Model.Events.ServiceEventArgs e) {
            UpdateVersion();
        }

        private void UpdateVersion() {
            Version = DataCollector.Instance.Version;
        }

        public ICommand IncreaseVersionCommand { get; private set; }

        public void IncreaseVersion() {
            DataCollector.Instance.Version += 0.001;
            DataCollector.Instance.UpdateServerVersion(Dispatcher);
        }

        #region Version
        public double Version {
            get { return (double) GetValue(VersionProperty); }
            set { SetValue(VersionProperty, value); }
        }

        public static readonly DependencyProperty VersionProperty =
           DependencyProperty.Register("Version", typeof(double), typeof(HomeViewModel), new PropertyMetadata(0.0));
        #endregion
    }
}
