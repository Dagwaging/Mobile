using System.Windows;
using System.Windows.Input;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Mvvm.Commands;

namespace Rhit.Applications.ViewModel.Models {
    public class HomeViewModel : DependencyObject {
        public HomeViewModel() {
            IncreaseVersionCommand = new RelayCommand(p => IncreaseVersion());
            Version = DataCollector.Version;

            DataCollector.Instance.VersionUpdate += new Model.Events.VersionEventHandler(VersionUpdate);
        }

        private void VersionUpdate(object sender, Model.Events.VersionEventArgs e) {
            Version = e.ServerVersion;
        }

        public ICommand IncreaseVersionCommand { get; private set; }

        public void IncreaseVersion() {
            DataCollector.Instance.IncreaseServerVersion();
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
