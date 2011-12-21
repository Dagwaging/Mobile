using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Controllers;
using System.Collections.ObjectModel;

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
        #region ShowBuildings
        public bool ShowBuildings {
            get { return (bool) GetValue(ShowBuildingsProperty); }
            set { SetValue(ShowBuildingsProperty, value); }
        }

        public static readonly DependencyProperty ShowBuildingsProperty =
           DependencyProperty.Register("ShowBuildings", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
        #endregion

        #region ShowLabels
        public bool ShowLabels {
            get { return (bool) GetValue(ShowLabelsProperty); }
            set { SetValue(ShowLabelsProperty, value); }
        }

        public static readonly DependencyProperty ShowLabelsProperty =
           DependencyProperty.Register("ShowLabels", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
        #endregion

        #region ShowAllLocations
        public bool ShowAllLocations {
            get { return (bool) GetValue(ShowAllLocationsProperty); }
            set { SetValue(ShowAllLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowAllLocationsProperty =
           DependencyProperty.Register("ShowAllLocations", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
        #endregion

        #region ShowInnerLocations
        public bool ShowInnerLocations {
            get { return (bool) GetValue(ShowInnerLocationsProperty); }
            set { SetValue(ShowInnerLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowInnerLocationsProperty =
           DependencyProperty.Register("ShowInnerLocations", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
        #endregion

        #region ShowTopLocations
        public bool ShowTopLocations {
            get { return (bool) GetValue(ShowTopLocationsProperty); }
            set { SetValue(ShowTopLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowTopLocationsProperty =
           DependencyProperty.Register("ShowTopLocations", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
        #endregion

        #region ShowFloorLocations
        public bool ShowFloorLocations {
            get { return (bool) GetValue(ShowFloorLocationsProperty); }
            set { SetValue(ShowFloorLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowFloorLocationsProperty =
           DependencyProperty.Register("ShowFloorLocations", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
        #endregion

        #region ShowSaveCancel
        public bool ShowSaveCancel {
            get { return (bool) GetValue(ShowSaveCancelProperty); }
            set { SetValue(ShowSaveCancelProperty, value); }
        }

        public static readonly DependencyProperty ShowSaveCancelProperty =
           DependencyProperty.Register("ShowSaveCancel", typeof(bool), typeof(MapBehavior), new PropertyMetadata(false));
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
