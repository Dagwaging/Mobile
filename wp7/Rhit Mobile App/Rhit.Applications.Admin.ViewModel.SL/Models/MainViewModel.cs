using System.Windows;
using Rhit.Applications.Model;
using Rhit.Applications.ViewModel.Controllers;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Model.Events;
using System.Collections.Generic;
using System.Windows.Input;
using Rhit.Applications.Mvvm.Commands;
using System.Collections.ObjectModel;
using Rhit.Applications.ViewModel.Behaviors;
using Rhit.Applications.ViewModel.Providers;

namespace Rhit.Applications.ViewModel.Models {
    public class MainViewModel : DependencyObject {
        //NOTE: Requires a call to Initialize before class is usable
        //Note: NoArg Constructor so ViewModel can be created in xaml
        public MainViewModel() { }

        public void Initialize(Map map, IBuildingMappingProvider buildingMappingProvider,
            IBuildingCornersProvider cornerProvider, IBitmapProvider imageProvider) {

            Locations = LocationsController.Instance;
            InitializeBehaviors(cornerProvider);
            MapController.CreateMapController(map);
            ImageController.CreateImageController(imageProvider, buildingMappingProvider);
            Image = ImageController.Instance;
            Map = MapController.Instance;
            GotoRhitCommand = new RelayCommand(p => GotoRhit());

            List<RhitLocation> locations = DataCollector.Instance.GetAllLocations(null);
            if(locations == null || locations.Count <= 0)
                DataCollector.Instance.UpdateAvailable += new ServiceEventHandler(OnLocationsRetrieved);
            else OnLocationsRetrieved(this, new ServiceEventArgs());
            Mapper = LocationPositionMapper.Instance;
        }

        public MainViewModel(Map map, IBuildingMappingProvider buildingMappingProvider,
            IBuildingCornersProvider cornerProvider, IBitmapProvider imageProvider) {
                Initialize(map, buildingMappingProvider, cornerProvider, imageProvider);
        }

        private void OnLocationsRetrieved(object sender, ServiceEventArgs e) {
            List<RhitLocation> locations = DataCollector.Instance.GetAllLocations(null);
            if(locations == null || locations.Count <= 0) return;
            Locations.SetLocations(locations);
        }

        private void InitializeBehaviors(IBuildingCornersProvider cornerProvider) {
            Behaviors = new ObservableCollection<MapBehavior>() {
                new BuildingsBehavior(cornerProvider),
                new LocationsBehavior(),
                new PathsBehavior(),
            };
            Behavior = Behaviors[0];
            AreBuildingOptionsVisible = true;
        }

        #region Dependency Properties
        #region Behavior
        public MapBehavior Behavior {
            get { return (MapBehavior) GetValue(BehaviorProperty); }
            set { SetValue(BehaviorProperty, value); }
        }

        public static readonly DependencyProperty BehaviorProperty =
           DependencyProperty.Register("Behavior", typeof(MapBehavior), typeof(MainViewModel),
           new PropertyMetadata(null, new PropertyChangedCallback(OnBehaviorChanged)));

        private static void OnBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            MainViewModel instance = (MainViewModel) d;
            instance.Behavior.Update();
            if(instance.Behavior is BuildingsBehavior)
                instance.AreBuildingOptionsVisible = true;
            else instance.AreBuildingOptionsVisible = false;
        }
        #endregion

        #region AreBuildingOptionsVisible
        public bool AreBuildingOptionsVisible {
            get { return (bool) GetValue(AreBuildingOptionsVisibleProperty); }
            set { SetValue(AreBuildingOptionsVisibleProperty, value); }
        }

        public static readonly DependencyProperty AreBuildingOptionsVisibleProperty =
           DependencyProperty.Register("AreBuildingOptionsVisible", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));
        #endregion
        #endregion

        public ObservableCollection<MapBehavior> Behaviors { get; set; }

        public ICommand GotoRhitCommand { get; private set; }

        public ImageController Image { get; private set; }

        public LocationsController Locations { get; private set; }

        public LocationPositionMapper Mapper { get; set; }

        public MapController Map { get; private set; }

        public void GotoRhit() {
            //TODO: Don't hard code
            Map.MapControl.Center = new GeoCoordinate(39.4820263, -87.3248677);
            Map.MapControl.ZoomLevel = 16;
        }

        public void SelectLocation(MapPolygon polygon) {
            try {
                LocationsController.Instance.SelectLocation((int) polygon.Tag);
            } catch { }
        }

        public void SelectLocation(Location coordinate) {
            Locations.SelectLocation(new GeoCoordinate(coordinate));
        }
    }
}
