using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Models.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModels.Controllers;
using Rhit.Applications.ViewModels.Providers;
using Rhit.Applications.ViewModels.Utilities;

namespace Rhit.Applications.ViewModels {
    public class TaskedViewModel : MapViewModel {
        //NOTE: Requires a call to Initialize and SetMode before class is usable
        //Note: NoArg Constructor so ViewModel can be created in xaml
        public TaskedViewModel() {
        }

        internal void Initialize(IBuildingMappingProvider buildingMappingProvider,
            ILocationsProvider locationsProvider, IBitmapProvider imageProvider) {

            Locations = LocationsController.Instance;

            LocationsProvider = locationsProvider;
            LocationsController.Instance.CurrentLocationChanged += new EventHandler(CurrentLocationChanged);

            InitializeCommands();

            Settings = SettingsController.Instance;
            Settings.ShowBuildings = true;

            ImageController.CreateImageController(imageProvider, buildingMappingProvider);
            Image = ImageController.Instance;

            Mapper = LocationPositionMapper.Instance;
        }

        protected virtual void InitializeCommands() {
            SaveCommand = new RelayCommand(p => Save());
            CancelCommand = new RelayCommand(p => Cancel());
        }

        #region Save Command/Methods
        public ICommand SaveCommand { get; private set; }

        protected virtual void Save() { }
        #endregion

        #region Cancel Command/Method
        public ICommand CancelCommand { get; private set; }

        protected virtual void Cancel() { }
        #endregion

        protected ILocationsProvider LocationsProvider { get; set; }

        public ImageController Image { get; private set; }

        public LocationPositionMapper Mapper { get; set; }

        #region ShowSave
        public bool ShowSave {
            get { return (bool) GetValue(ShowSaveProperty); }
            set { SetValue(ShowSaveProperty, value); }
        }

        public static readonly DependencyProperty ShowSaveProperty =
           DependencyProperty.Register("ShowSave", typeof(bool), typeof(TaskedViewModel), new PropertyMetadata(false));
        #endregion
    }
}
