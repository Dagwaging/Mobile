using System.Windows;
using System.Windows.Input;
using Rhit.Applications.Models.Events;
using Rhit.Applications.Models.Services;
using Rhit.Applications.Mvvm.Commands;
using System.IO;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.ViewModels {
    public class HomeViewModel : DependencyObject {
        public HomeViewModel() {
            IncreaseVersionCommand = new RelayCommand(p => IncreaseVersion());
            Version = DataCollector.Version;

            MapController.Instance.UpdateOverlays(); //TODO: Probably doesn't belong here

            DataCollector.Instance.VersionUpdate += new VersionEventHandler(VersionUpdate);
        }

        private void VersionUpdate(object sender, VersionEventArgs e) {
            if (e.ServerVersion == 0) return;

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

        public void DoSomething() {
            DataCollector.Instance.GetFolders();
        }

        public void DoSomethingElse(FileInfo file) {
            DataCollector.Instance.UploadFile(file);
        }
    }
}
