using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.ViewModel.Behaviors {
    public abstract class MapBehavior : DependencyObject {
        public MapBehavior() {
            LocationsController.Instance.CurrentLocationChanged += new LocationEventHandler(CurrentLocationChanged);
            LocationsController.Instance.LocationsChanged += new LocationEventHandler(LocationsChanged);
            Label = "Default Behavior";
            SaveCommand = new RelayCommand(p => Save());
            CancelCommand = new RelayCommand(p => Cancel());
            Initialize();
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        protected abstract void LocationsChanged(object sender, LocationEventArgs e);

        protected abstract void CurrentLocationChanged(object sender, LocationEventArgs e);

        protected abstract void Initialize();

        protected string Label { get; set; }

        #region Dependency Properties
        #region AreBuildingsVisible
        public bool AreBuildingsVisible {
            get { return (bool) GetValue(AreOutlinesVisibleProperty); }
            set { SetValue(AreOutlinesVisibleProperty, value); }
        }

        public static readonly DependencyProperty AreOutlinesVisibleProperty =
           DependencyProperty.Register("AreBuildingsVisible", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
        #endregion

        #region AreLabelsVisible
        public bool AreLabelsVisible {
            get { return (bool) GetValue(AreLabelsVisibleProperty); }
            set { SetValue(AreLabelsVisibleProperty, value); }
        }

        public static readonly DependencyProperty AreLabelsVisibleProperty =
           DependencyProperty.Register("AreLabelsVisible", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
        #endregion

        #region AreLocationsVisible
        public bool AreLocationsVisible {
            get { return (bool) GetValue(AreLocationsVisibleProperty); }
            set { SetValue(AreLocationsVisibleProperty, value); }
        }

        public static readonly DependencyProperty AreLocationsVisibleProperty =
           DependencyProperty.Register("AreLocationsVisible", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
        #endregion

        #region AreSaveCancelVisible
        public bool AreSaveCancelVisible {
            get { return (bool) GetValue(AreSaveCancelVisibleProperty); }
            set { SetValue(AreSaveCancelVisibleProperty, value); }
        }

        public static readonly DependencyProperty AreSaveCancelVisibleProperty =
           DependencyProperty.Register("AreLocationsVisible", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
        #endregion
        #endregion

        protected abstract void Save();

        protected abstract void Cancel();

        public abstract void SaveSettings();

        public abstract void LoadSettings();

        public override string ToString() {
            return Label;
        }

        public abstract void Update();
    }
}
